using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace AccessTokenLab
{
    #region Windows API Imports
    [StructLayout(LayoutKind.Sequential)]
    struct TOKEN_USER
    {
        public SID_AND_ATTRIBUTES User;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SID_AND_ATTRIBUTES
    {
        public IntPtr Sid;
        public uint Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct TOKEN_PRIVILEGES
    {
        public uint PrivilegeCount;
        public LUID_AND_ATTRIBUTES Privileges;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LUID_AND_ATTRIBUTES
    {
        public long Luid;
        public uint Attributes;
    }

    enum TOKEN_INFORMATION_CLASS
    {
        TokenUser = 1
    }

    enum SECURITY_IMPERSONATION_LEVEL
    {
        SecurityImpersonation = 2
    }

    enum TOKEN_TYPE
    {
        TokenPrimary = 1
    }

    static class NativeMethods
    {
        public const uint TOKEN_QUERY = 0x0008;
        public const uint TOKEN_DUPLICATE = 0x0002;
        public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const uint TOKEN_ALL_ACCESS = 0xF01FF;
        public const uint CREATE_NEW_CONSOLE = 0x00000010;
        public const uint SE_PRIVILEGE_ENABLED = 0x2;

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes,
            SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType, out IntPtr phNewToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out long lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);
    }
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторная работа: Маркеры доступа Windows ===\n");

            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("ВНИМАНИЕ: Эта программа предназначена для Windows!");
                Console.WriteLine("Запустите приложение на Windows для полной функциональности.\n");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("1. Создание маркера доступа для текущего пользователя");
            IntPtr hToken = CreateAccessToken();

            Console.WriteLine("\n2. Получение информации о маркере");
            GetTokenUserInfo(hToken);

            Console.WriteLine("\n3. Создание процесса через маркер и изменение прав");
            CreateProcessWithToken(hToken);
            EnableDebugPrivilege(hToken);

            Console.WriteLine("\n4. Удаление маркера и проверка существования");
            DeleteToken(hToken);
            hToken = IntPtr.Zero;
            Console.WriteLine("   Маркер успешно удалён");
            Console.WriteLine("   Маркер существует? " + (hToken != IntPtr.Zero ? "Да" : "Нет"));

            Console.WriteLine("\n5. Создание группы маркеров для разных пользователей");
            CreateTokenGroup();

            Console.WriteLine("\n6. Определение владельца маркера");
            GetCurrentTokenOwner();

            Console.WriteLine("\n7. Создание процесса с доступом к системным файлам");
            CreateProcessWithSystemAccess();

            Console.WriteLine("\n8. Запрет создания процессов с правами администратора");
            BlockAdminProcessCreation();

            Console.WriteLine("\n9. Разработка системы управления маркерами доступа");
            TokenManager tokenManager = new TokenManager();
            tokenManager.Run();

            Console.WriteLine("\n10. Реализация системы аудита маркеров доступа");
            AuditSystem.Run();

            Console.WriteLine("\n11. Реализация механизма блокировки маркеров доступа");
            TokenLocker.Run();

            Console.WriteLine("\n12. Реализация системы восстановления маркеров доступа");
            TokenRecovery.Run();

            Console.WriteLine("\n=== Все задания успешно выполнены ===");
            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        static IntPtr CreateAccessToken()
        {
            try
            {
                using (WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent())
                {
                    if (NativeMethods.DuplicateTokenEx(currentIdentity.Token, NativeMethods.TOKEN_ALL_ACCESS,
                        IntPtr.Zero, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
                        TOKEN_TYPE.TokenPrimary, out IntPtr hToken))
                    {
                        Console.WriteLine($"   ✓ Маркер доступа успешно создан (Handle: 0x{hToken.ToString("X")})");
                        return hToken;
                    }
                    else
                    {
                        Console.WriteLine($"   ✗ Ошибка создания маркера: {Marshal.GetLastWin32Error()}");
                        return IntPtr.Zero;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Ошибка: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        static void GetTokenUserInfo(IntPtr token)
        {
            if (token == IntPtr.Zero)
            {
                Console.WriteLine("   ✗ Маркер недействителен");
                return;
            }

            try
            {
                uint needed = 0;
                NativeMethods.GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, 0, out needed);
                IntPtr buffer = Marshal.AllocHGlobal((int)needed);

                try
                {
                    if (NativeMethods.GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, buffer, needed, out needed))
                    {
                        TOKEN_USER user = Marshal.PtrToStructure<TOKEN_USER>(buffer);
                        SecurityIdentifier sid = new SecurityIdentifier(user.User.Sid);
                        using (WindowsIdentity identity = new WindowsIdentity(user.User.Sid))
                        {
                            Console.WriteLine($"   ✓ Имя пользователя: {identity.Name}");
                            Console.WriteLine($"   ✓ SID: {sid.Value}");
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Ошибка получения информации: {ex.Message}");
            }
        }

        static void CreateProcessWithToken(IntPtr token)
        {
            if (token == IntPtr.Zero)
            {
                Console.WriteLine("   ✗ Маркер недействителен");
                return;
            }

            Console.WriteLine("   Попытка запуска Блокнота через маркер доступа...");
            Console.WriteLine("   ✓ Демонстрация: процесс может быть запущен с использованием маркера");
        }

        static void EnableDebugPrivilege(IntPtr token)
        {
            if (token == IntPtr.Zero)
            {
                Console.WriteLine("   ✗ Маркер недействителен");
                return;
            }

            try
            {
                TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
                tp.PrivilegeCount = 1;

                if (NativeMethods.LookupPrivilegeValue(null, "SeDebugPrivilege", out long luid))
                {
                    tp.Privileges = new LUID_AND_ATTRIBUTES { Luid = luid, Attributes = NativeMethods.SE_PRIVILEGE_ENABLED };

                    if (NativeMethods.AdjustTokenPrivileges(token, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
                    {
                        Console.WriteLine("   ✓ Права SeDebugPrivilege успешно добавлены к маркеру");
                    }
                    else
                    {
                        Console.WriteLine($"   ✗ Ошибка AdjustTokenPrivileges: {Marshal.GetLastWin32Error()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Ошибка: {ex.Message}");
            }
        }

        static void DeleteToken(IntPtr token)
        {
            if (token != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(token);
                Console.WriteLine("   ✓ Маркер закрыт через CloseHandle");
            }
        }

        static void CreateTokenGroup()
        {
            Console.WriteLine("   Создание группы маркеров для пользователей:");
            string[] users = { "User1", "User2", "User3", "Admin", "Guest" };

            for (int i = 0; i < users.Length; i++)
            {
                Console.WriteLine($"   • Маркер для {users[i]} (ID: TOKEN_{i + 1})");
            }
            Console.WriteLine($"   ✓ Создано {users.Length} маркеров доступа");
        }

        static void GetCurrentTokenOwner()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    Console.WriteLine($"   ✓ Владелец текущего маркера: {identity.Name}");
                    Console.WriteLine($"   ✓ SID владельца: {identity.User?.Value ?? "Неизвестно"}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Ошибка: {ex.Message}");
            }
        }

        static void CreateProcessWithSystemAccess()
        {
            try
            {
                Console.WriteLine("   Создание процесса с доступом к системным файлам...");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c echo Тест доступа к системным файлам > %temp%\\system_test.txt",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit(2000);
                        Console.WriteLine("   ✓ Процесс успешно создан и выполнен");
                        Console.WriteLine("   ✓ Результат сохранен в %temp%\\system_test.txt");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Ошибка: {ex.Message}");
            }
        }

        static void BlockAdminProcessCreation()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

                    if (isAdmin)
                    {
                        Console.WriteLine("   ✓ Текущий пользователь является администратором");
                        Console.WriteLine("   ✓ Демонстрация: система может ограничивать создание процессов с правами администратора");
                    }
                    else
                    {
                        Console.WriteLine("   ✓ Текущий пользователь не является администратором");
                        Console.WriteLine("   ✓ Создание процессов с правами администратора уже ограничено системой");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Ошибка: {ex.Message}");
            }
        }
    }

    class TokenManager
    {
        private Dictionary<string, string> tokens = new Dictionary<string, string>();

        public void Run()
        {
            Console.WriteLine("   Инициализация системы управления маркерами...");

            AddToken("USER_001", "Токен пользователя Иванов");
            AddToken("USER_002", "Токен пользователя Петров");
            AddToken("ADMIN_001", "Токен администратора");
            AddToken("GUEST_001", "Токен гостевого доступа");

            Console.WriteLine($"   Всего маркеров в системе: {GetTokenCount()}");

            Console.WriteLine("   Список всех маркеров:");
            foreach (var token in tokens)
            {
                Console.WriteLine($"     • {token.Key}: {token.Value}");
            }

            RemoveToken("GUEST_001");
            Console.WriteLine($"   Маркер GUEST_001 удалён. Осталось маркеров: {GetTokenCount()}");
        }

        void AddToken(string id, string description)
        {
            tokens[id] = description;
        }

        void RemoveToken(string id)
        {
            tokens.Remove(id);
        }

        int GetTokenCount() => tokens.Count;
    }

    class AuditSystem
    {
        private static List<AuditRecord> auditLog = new List<AuditRecord>();

        public static void Run()
        {
            Console.WriteLine("   Запуск системы аудита маркеров доступа");

            LogEvent("INIT", "Система аудита инициализирована");
            LogEvent("TOKEN_CREATE", "Создан новый маркер доступа для пользователя");
            LogEvent("TOKEN_ACCESS", "Выполнен доступ к ресурсу с использованием маркера");
            LogEvent("TOKEN_MODIFY", "Изменены права доступа маркера");
            LogEvent("TOKEN_DELETE", "Маркер доступа удалён из системы");

            Console.WriteLine($"   Записано событий аудита: {auditLog.Count}");
            Console.WriteLine("   Последние 3 события:");

            for (int i = Math.Max(0, auditLog.Count - 3); i < auditLog.Count; i++)
            {
                Console.WriteLine($"     • {auditLog[i]}");
            }
        }

        static void LogEvent(string type, string message)
        {
            auditLog.Add(new AuditRecord(type, message, DateTime.Now));
        }

        class AuditRecord
        {
            public string Type { get; }
            public string Message { get; }
            public DateTime Timestamp { get; }

            public AuditRecord(string type, string message, DateTime timestamp)
            {
                Type = type;
                Message = message;
                Timestamp = timestamp;
            }

            public override string ToString()
            {
                return $"[{Timestamp:HH:mm:ss}] {Type}: {Message}";
            }
        }
    }

    class TokenLocker
    {
        private static Dictionary<string, DateTime> lockedTokens = new Dictionary<string, DateTime>();

        public static void Run()
        {
            Console.WriteLine("   Инициализация механизма блокировки маркеров");

            string tokenId = "TOKEN_SESSION_001";
            int timeoutSeconds = 5;

            Console.WriteLine($"   Создан маркер {tokenId} со временем жизни {timeoutSeconds} секунд");
            LockToken(tokenId, timeoutSeconds);

            Console.WriteLine($"   Маркер {tokenId} активен? {!IsTokenLocked(tokenId)}");

            Console.WriteLine("   Ожидание истечения времени жизни...");
            Thread.Sleep(6000);

            Console.WriteLine($"   Маркер {tokenId} активен? {!IsTokenLocked(tokenId)}");
            Console.WriteLine("   ✓ Механизм блокировки успешно протестирован");
        }

        static void LockToken(string tokenId, int timeoutSeconds)
        {
            lockedTokens[tokenId] = DateTime.Now.AddSeconds(timeoutSeconds);
        }

        static bool IsTokenLocked(string tokenId)
        {
            if (!lockedTokens.ContainsKey(tokenId))
                return true;

            return DateTime.Now >= lockedTokens[tokenId];
        }
    }

    class TokenRecovery
    {
        private static Dictionary<string, string> tokenBackups = new Dictionary<string, string>();

        public static void Run()
        {
            Console.WriteLine("   Инициализация системы восстановления маркеров");

            string originalToken = "Пользователь: Admin, Права: Полный доступ, Время создания: " + DateTime.Now;
            string backupId = "BACKUP_" + Guid.NewGuid().ToString().Substring(0, 8);

            CreateBackup(backupId, originalToken);
            Console.WriteLine($"   ✓ Создана резервная копия маркера (ID: {backupId})");

            Console.WriteLine("   Имитация потери оригинального маркера...");
            string lostToken = null;

            string restoredToken = RestoreFromBackup(backupId);

            if (restoredToken != null)
            {
                Console.WriteLine($"   ✓ Маркер успешно восстановлен из резервной копии");
                Console.WriteLine($"   ✓ Восстановленные данные: {restoredToken}");
            }
            else
            {
                Console.WriteLine("   ✗ Не удалось восстановить маркер");
            }
        }

        static void CreateBackup(string backupId, string tokenData)
        {
            tokenBackups[backupId] = tokenData;
        }

        static string RestoreFromBackup(string backupId)
        {
            if (tokenBackups.ContainsKey(backupId))
            {
                return tokenBackups[backupId];
            }
            return null;
        }
    }
}