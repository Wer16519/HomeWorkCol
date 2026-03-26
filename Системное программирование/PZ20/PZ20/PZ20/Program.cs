using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.IO.Compression;

namespace FileSystemTasks
{
    // Задание 12: Класс для работы с файловой системой Windows
    public class FileSystemManager : IDisposable
    {
        private string basePath;
        private bool disposed = false;

        // Windows API импорты для задания 9 и 10
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFileW(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool DeleteFileW(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveFileExW(string lpExistingFileName, string lpNewFileName, uint dwFlags);

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        private const uint CREATE_ALWAYS = 2;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private const uint MOVEFILE_REPLACE_EXISTING = 0x1;

        // Статические методы для вызова из других методов
        public static IntPtr ApiCreateFile(string filename, uint desiredAccess, uint creationDisposition)
        {
            return CreateFileW(filename, desiredAccess, 0, IntPtr.Zero, creationDisposition, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
        }

        public static bool ApiWriteFile(IntPtr hFile, byte[] buffer, out uint bytesWritten)
        {
            return WriteFile(hFile, buffer, (uint)buffer.Length, out bytesWritten, IntPtr.Zero);
        }

        public static bool ApiReadFile(IntPtr hFile, byte[] buffer, out uint bytesRead)
        {
            return ReadFile(hFile, buffer, (uint)buffer.Length, out bytesRead, IntPtr.Zero);
        }

        public static bool ApiCloseHandle(IntPtr hFile)
        {
            return CloseHandle(hFile);
        }

        public static bool ApiDeleteFile(string filename)
        {
            return DeleteFileW(filename);
        }

        public static bool ApiMoveFileEx(string source, string dest, uint flags)
        {
            return MoveFileExW(source, dest, flags);
        }

        public FileSystemManager(string path)
        {
            basePath = path;
            CreateWorkspace();
        }

        private void CreateWorkspace()
        {
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
                Console.WriteLine($"[Задание 12] Создана рабочая директория: {basePath}");
            }
        }

        public string GetBasePath() => basePath;

        public bool CreateFile(string filename, string content)
        {
            try
            {
                string fullPath = Path.Combine(basePath, filename);
                File.WriteAllText(fullPath, content, Encoding.UTF8);
                Console.WriteLine($"  Файл создан: {fullPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка создания файла: {ex.Message}");
                return false;
            }
        }

        public string? ReadFile(string filename)
        {
            try
            {
                string fullPath = Path.Combine(basePath, filename);
                return File.ReadAllText(fullPath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка чтения файла: {ex.Message}");
                return null;
            }
        }

        public bool RemoveFile(string filename)
        {
            try
            {
                string fullPath = Path.Combine(basePath, filename);
                File.Delete(fullPath);
                Console.WriteLine($"  Файл удалён: {fullPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка удаления файла: {ex.Message}");
                return false;
            }
        }

        public bool MoveFile(string sourceFilename, string destFilename)
        {
            try
            {
                string sourcePath = Path.Combine(basePath, sourceFilename);
                string destPath = Path.Combine(basePath, destFilename);
                File.Move(sourcePath, destPath);
                Console.WriteLine($"  Файл перемещён: {sourcePath} -> {destPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка перемещения файла: {ex.Message}");
                return false;
            }
        }

        public string[] GetFiles() => Directory.GetFiles(basePath);

        public void Dispose()
        {
            if (!disposed)
            {
                try
                {
                    if (Directory.Exists(basePath))
                    {
                        // Не удаляем директорию на рабочем столе автоматически
                        // чтобы пользователь мог просмотреть результаты
                        Console.WriteLine($"[Задание 12] Рабочая директория: {basePath}");
                    }
                }
                catch { }
                disposed = true;
            }
        }
    }

    // Класс для сжатия/распаковки (Задание 5)
    public static class FileCompressor
    {
        public static bool CompressFile(string inputPath, string outputPath)
        {
            try
            {
                using (FileStream inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (GZipStream compressionStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    inputStream.CopyTo(compressionStream);
                }
                Console.WriteLine($"  Файл сжат: {outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка сжатия: {ex.Message}");
                return false;
            }
        }

        public static bool DecompressFile(string inputPath, string outputPath)
        {
            try
            {
                using (FileStream inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (GZipStream decompressionStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(outputStream);
                }
                Console.WriteLine($"  Файл распакован: {outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка распаковки: {ex.Message}");
                return false;
            }
        }
    }

    // Класс для шифрования/дешифрования (Задание 14)
    public static class FileEncryptor
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF"); // 32 байта
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("0123456789ABCDEF"); // 16 байт

        public static bool EncryptFile(string inputPath, string outputPath)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (FileStream inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                    using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        inputStream.CopyTo(cryptoStream);
                    }
                }
                Console.WriteLine($"  Файл зашифрован: {outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка шифрования: {ex.Message}");
                return false;
            }
        }

        public static bool DecryptFile(string inputPath, string outputPath)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (FileStream inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                    using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(outputStream);
                    }
                }
                Console.WriteLine($"  Файл расшифрован: {outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Ошибка дешифрования: {ex.Message}");
                return false;
            }
        }
    }

    class Program
    {
        private static FileSystemManager? fsManager;
        private static string desktopPath;

        static void Main(string[] args)
        {
            // Получаем путь к рабочему столу текущего пользователя
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string workDir = Path.Combine(desktopPath, "PZ20_FileSystem_Workspace");

            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("ПРАКТИЧЕСКОЕ ЗАНЯТИЕ 20: РАБОТА С ФАЙЛОВОЙ СИСТЕМОЙ WINDOWS");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Все файлы будут сохранены в: {workDir}");
            Console.WriteLine();

            try
            {
                using (fsManager = new FileSystemManager(workDir))
                {
                    // Задание 1: Создать программу которая создаёт файл и записывает в него данные
                    Task1_CreateAndWriteFile();

                    // Задание 2: Создать программу которая удаляет файл после использования
                    Task2_DeleteAfterUse();

                    // Задание 3: Работа с файлами на удаленном сервере (UNC путь)
                    Task3_RemoteFileWork();

                    // Задание 4: Управление доступом к файлам между процессами
                    Task4_FileLockingBetweenProcesses();

                    // Задание 5: Алгоритм сжатия и распаковки данных в файле
                    Task5_CompressionDecompression();

                    // Задание 6: Разработать библиотеку для работы с файловой системой (используем FileSystemManager)
                    Task6_FileSystemLibrary();

                    // Задание 7: Система управления памятью для работы с файловой системой
                    Task7_MemoryManagement();

                    // Задание 8: Работа с большими файлами
                    Task8_LargeFiles();

                    // Задание 9: Использование API функций для работы с файловой системой
                    Task9_WindowsAPI();

                    // Задание 10: Перенос файлов на удаленный сервер с использованием API
                    Task10_RemoteFileTransfer();

                    // Задание 11: Удаление файлов после их использования
                    Task11_AutoDeleteAfterUse();

                    // Задание 12: Класс для работы с файловой системой (уже реализован)
                    Task12_FileSystemClass();

                    // Задание 13: Проверка целостности файлов
                    Task13_FileIntegrityCheck();

                    // Задание 14: Шифрование и дешифрование данных в файлах
                    Task14_EncryptionDecryption();
                }

                Console.WriteLine();
                Console.WriteLine("=".PadRight(80, '='));
                Console.WriteLine("ВСЕ ЗАДАНИЯ УСПЕШНО ВЫПОЛНЕНЫ!");
                Console.WriteLine($"Все файлы сохранены в папке: {workDir}");
                Console.WriteLine("=".PadRight(80, '='));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // Задание 1: Создание файла и запись данных
        static void Task1_CreateAndWriteFile()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 1: Создание файла и запись данных ---");

            string content = "Это тестовый файл, созданный программой.\n";
            content += "Дата создания: " + DateTime.Now.ToString() + "\n";
            content += "Содержит несколько строк для проверки.\n";
            content += "Строка 1\nСтрока 2\nСтрока 3\n";

            fsManager!.CreateFile("task1_test.txt", content);

            string? readContent = fsManager.ReadFile("task1_test.txt");
            Console.WriteLine($"  Содержимое файла:\n{readContent}");
        }

        // Задание 2: Удаление файла после использования
        static void Task2_DeleteAfterUse()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 2: Удаление файла после использования ---");

            string filename = "task2_temp.txt";
            fsManager!.CreateFile(filename, "Временные данные для удаления");

            string? content = fsManager.ReadFile(filename);
            Console.WriteLine($"  Прочитано: {content}");

            fsManager.RemoveFile(filename);

            // Проверка, что файл удалён
            if (!File.Exists(Path.Combine(fsManager.GetBasePath(), filename)))
                Console.WriteLine("  Файл успешно удалён после использования");
        }

        // Задание 3: Работа с файлами на удаленном сервере (эмуляция)
        static void Task3_RemoteFileWork()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 3: Работа с файлами на удаленном сервере ---");
            Console.WriteLine("  (Эмуляция работы с UNC-путями)");

            // Эмуляция UNC пути (в реальных условиях нужно указать существующий сетевой путь)
            string remotePath = Path.Combine(fsManager!.GetBasePath(), "remote_emulation");

            if (!Directory.Exists(remotePath))
                Directory.CreateDirectory(remotePath);

            string remoteFile = Path.Combine(remotePath, "remote_test.txt");
            File.WriteAllText(remoteFile, "Данные на удаленном ресурсе (эмуляция)");

            Console.WriteLine($"  Файл создан по пути: {remoteFile}");

            string readContent = File.ReadAllText(remoteFile);
            Console.WriteLine($"  Прочитано: {readContent}");

            // Очистка
            File.Delete(remoteFile);
            Directory.Delete(remotePath);
            Console.WriteLine("  Удалённые данные очищены");
        }

        // Задание 4: Управление доступом к файлам между процессами
        static void Task4_FileLockingBetweenProcesses()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 4: Управление доступом к файлам между процессами ---");

            string filename = Path.Combine(fsManager!.GetBasePath(), "task4_locked.txt");

            // Создаём файл и блокируем его
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] data = Encoding.UTF8.GetBytes("Заблокированные данные");
                fs.Write(data, 0, data.Length);
                Console.WriteLine("  Файл открыт с эксклюзивной блокировкой (FileShare.None)");

                // Попытка открыть файл из другого потока (эмуляция другого процесса)
                bool canOpen = false;
                try
                {
                    using (FileStream fs2 = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        canOpen = true;
                    }
                }
                catch (IOException)
                {
                    canOpen = false;
                }

                Console.WriteLine($"  Другой процесс может открыть файл: {canOpen} (ожидаемо: False)");

                // Ждём немного, пока файл заблокирован
                Thread.Sleep(1000);
            }

            // После освобождения блокировки
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Console.WriteLine("  После снятия блокировки файл доступен для чтения");
            }

            fsManager.RemoveFile("task4_locked.txt");
        }

        // Задание 5: Сжатие и распаковка данных
        static void Task5_CompressionDecompression()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 5: Сжатие и распаковка данных ---");

            string originalFile = Path.Combine(fsManager!.GetBasePath(), "task5_original.txt");
            string compressedFile = Path.Combine(fsManager.GetBasePath(), "task5_compressed.gz");
            string decompressedFile = Path.Combine(fsManager.GetBasePath(), "task5_decompressed.txt");

            // Создаём тестовый файл с повторяющимися данными для хорошего сжатия
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                sb.AppendLine($"Строка номер {i} с повторяющимся текстом для тестирования сжатия данных.");
            }
            File.WriteAllText(originalFile, sb.ToString(), Encoding.UTF8);

            long originalSize = new FileInfo(originalFile).Length;
            Console.WriteLine($"  Исходный файл: {originalSize} байт");

            // Сжатие
            FileCompressor.CompressFile(originalFile, compressedFile);
            long compressedSize = new FileInfo(compressedFile).Length;
            Console.WriteLine($"  Сжатый файл: {compressedSize} байт (экономия: {(1 - (double)compressedSize / originalSize) * 100:F1}%)");

            // Распаковка
            FileCompressor.DecompressFile(compressedFile, decompressedFile);
            long decompressedSize = new FileInfo(decompressedFile).Length;
            Console.WriteLine($"  Распакованный файл: {decompressedSize} байт");

            // Очистка
            fsManager.RemoveFile("task5_original.txt");
            fsManager.RemoveFile("task5_compressed.gz");
            fsManager.RemoveFile("task5_decompressed.txt");
        }

        // Задание 6: Библиотека для работы с файловой системой
        static void Task6_FileSystemLibrary()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 6: Библиотека для работы с файловой системой ---");
            Console.WriteLine("  Библиотека FileSystemManager уже реализована и используется во всех заданиях.");
            Console.WriteLine("  Функционал библиотеки:");
            Console.WriteLine("    - CreateFile() - создание файла");
            Console.WriteLine("    - ReadFile() - чтение файла");
            Console.WriteLine("    - RemoveFile() - удаление файла");
            Console.WriteLine("    - MoveFile() - перемещение файла");
            Console.WriteLine("    - GetFiles() - получение списка файлов");

            fsManager!.CreateFile("task6_lib_test.txt", "Тест работы библиотеки");
            string[] files = fsManager.GetFiles();
            Console.WriteLine($"  Файлы в рабочей директории: {files.Length}");
            fsManager.RemoveFile("task6_lib_test.txt");
        }

        // Задание 7: Система управления памятью для работы с файловой системой
        static void Task7_MemoryManagement()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 7: Система управления памятью для работы с файловой системой ---");
            Console.WriteLine("  Использование буферизации и потоковой обработки для экономии памяти");

            string largeFile = Path.Combine(fsManager!.GetBasePath(), "task7_large.txt");

            // Создаём файл с помощью буферизированной записи
            using (StreamWriter sw = new StreamWriter(largeFile, false, Encoding.UTF8, 8192))
            {
                for (int i = 0; i < 10000; i++)
                {
                    sw.WriteLine($"Буферизированная запись строки номер {i}");
                }
            }
            Console.WriteLine("  Файл создан с использованием буфера 8KB");

            // Чтение с использованием буфера
            using (StreamReader sr = new StreamReader(largeFile, Encoding.UTF8, true, 8192))
            {
                int lineCount = 0;
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    lineCount++;
                    if (lineCount == 1)
                        Console.WriteLine($"  Первая строка: {(line.Length > 50 ? line.Substring(0, 50) + "..." : line)}");
                    if (lineCount == 10000)
                        Console.WriteLine($"  Последняя строка: {(line.Length > 50 ? line.Substring(0, 50) + "..." : line)}");
                }
                Console.WriteLine($"  Всего прочитано строк: {lineCount}");
            }

            fsManager.RemoveFile("task7_large.txt");
        }

        // Задание 8: Работа с большими файлами
        static void Task8_LargeFiles()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 8: Работа с большими файлами ---");
            Console.WriteLine("  Создание и обработка файла большого размера (потоковая обработка)");

            string largeFile = Path.Combine(fsManager!.GetBasePath(), "task8_large.bin");

            // Создаём файл размером ~10MB
            const long targetSize = 10 * 1024 * 1024; // 10 MB
            const int bufferSize = 64 * 1024; // 64 KB

            using (FileStream fs = new FileStream(largeFile, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[bufferSize];
                long written = 0;

                while (written < targetSize)
                {
                    int toWrite = (int)Math.Min(bufferSize, targetSize - written);
                    fs.Write(buffer, 0, toWrite);
                    written += toWrite;
                }
            }

            long fileSize = new FileInfo(largeFile).Length;
            Console.WriteLine($"  Создан файл размером: {fileSize / (1024 * 1024)} MB");

            // Чтение с буферизацией
            using (FileStream fs = new FileStream(largeFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
            {
                byte[] buffer = new byte[bufferSize];
                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    totalRead += bytesRead;
                }

                Console.WriteLine($"  Прочитано байт: {totalRead / (1024 * 1024)} MB");
            }

            fsManager.RemoveFile("task8_large.bin");
        }

        // Задание 9: Использование Windows API функций
        static void Task9_WindowsAPI()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 9: Использование Windows API функций ---");

            string filename = Path.Combine(fsManager!.GetBasePath(), "task9_api_test.txt");

            // Используем CreateFile через P/Invoke
            IntPtr hFile = FileSystemManager.ApiCreateFile(filename, 0x40000000, 2); // GENERIC_WRITE, CREATE_ALWAYS

            if (hFile.ToInt64() != -1 && hFile.ToInt64() != 0)
            {
                byte[] data = Encoding.UTF8.GetBytes("Данные, записанные через Windows API");
                uint bytesWritten;

                bool success = FileSystemManager.ApiWriteFile(hFile, data, out bytesWritten);

                if (success)
                {
                    Console.WriteLine($"  Записано через API: {bytesWritten} байт");
                }

                FileSystemManager.ApiCloseHandle(hFile);
            }

            // Чтение через API
            hFile = FileSystemManager.ApiCreateFile(filename, 0x80000000, 3); // GENERIC_READ, OPEN_EXISTING

            if (hFile.ToInt64() != -1 && hFile.ToInt64() != 0)
            {
                byte[] buffer = new byte[256];
                uint bytesRead;

                bool success = FileSystemManager.ApiReadFile(hFile, buffer, out bytesRead);

                if (success && bytesRead > 0)
                {
                    string content = Encoding.UTF8.GetString(buffer, 0, (int)bytesRead);
                    Console.WriteLine($"  Прочитано через API: {content}");
                }

                FileSystemManager.ApiCloseHandle(hFile);
            }

            // Удаление через API
            FileSystemManager.ApiDeleteFile(filename);
            Console.WriteLine("  Файл удалён через API");
        }

        // Задание 10: Перенос файлов на удаленный сервер
        static void Task10_RemoteFileTransfer()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 10: Перенос файлов на удаленный сервер с использованием API ---");
            Console.WriteLine("  (Эмуляция переноса на удаленный сервер)");

            string localFile = Path.Combine(fsManager!.GetBasePath(), "task10_local.txt");
            string remoteDir = Path.Combine(fsManager.GetBasePath(), "remote_transfer");
            string remoteFile = Path.Combine(remoteDir, "task10_remote.txt");

            // Создаём локальный файл
            File.WriteAllText(localFile, "Данные для переноса на удаленный сервер");

            // Создаём директорию "удаленного сервера"
            if (!Directory.Exists(remoteDir))
                Directory.CreateDirectory(remoteDir);

            // Используем MoveFileEx для переноса
            bool success = FileSystemManager.ApiMoveFileEx(localFile, remoteFile, 0x1); // MOVEFILE_REPLACE_EXISTING

            if (success)
            {
                Console.WriteLine($"  Файл перенесён: {localFile} -> {remoteFile}");

                // Проверяем, что файл действительно перенесён
                if (File.Exists(remoteFile))
                {
                    string content = File.ReadAllText(remoteFile);
                    Console.WriteLine($"  Содержимое перенесённого файла: {content}");
                }
            }
            else
            {
                Console.WriteLine("  Ошибка переноса файла");
            }

            // Очистка
            if (File.Exists(remoteFile))
                File.Delete(remoteFile);
            if (Directory.Exists(remoteDir))
                Directory.Delete(remoteDir);
        }

        // Задание 11: Удаление файлов после использования
        static void Task11_AutoDeleteAfterUse()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 11: Удаление файлов после их использования ---");
            Console.WriteLine("  Использование using для автоматического удаления");

            string tempFile = Path.Combine(fsManager!.GetBasePath(), "task11_temp.txt");

            // Используем обёртку для автоматического удаления
            using (var tempFileWrapper = new TempFileWrapper(tempFile))
            {
                File.WriteAllText(tempFile, "Временные данные");
                Console.WriteLine($"  Файл создан: {tempFile}");

                string content = File.ReadAllText(tempFile);
                Console.WriteLine($"  Прочитано: {content}");

                // При выходе из using файл будет удалён
            }

            // Проверяем, что файл удалён
            if (!File.Exists(tempFile))
                Console.WriteLine("  Файл автоматически удалён после использования");
        }

        // Задание 12: Класс для работы с файловой системой
        static void Task12_FileSystemClass()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 12: Класс для работы с файловой системой Windows ---");
            Console.WriteLine("  Класс FileSystemManager уже реализован и включает:");
            Console.WriteLine("    - Автоматическое создание рабочей директории");
            Console.WriteLine("    - Методы для создания, чтения, удаления, перемещения файлов");
            Console.WriteLine("    - Реализация IDisposable для очистки ресурсов");

            string testDir = Path.Combine(desktopPath, "PZ20_Test_Class");
            using (var tempManager = new FileSystemManager(testDir))
            {
                tempManager.CreateFile("class_test.txt", "Тест работы класса");
                Console.WriteLine($"  Файл создан в: {tempManager.GetBasePath()}");
                string? content = tempManager.ReadFile("class_test.txt");
                Console.WriteLine($"  Содержимое: {content}");
            }
            Console.WriteLine("  Рабочая директория сохранена на рабочем столе");
        }

        // Задание 13: Проверка целостности файлов
        static void Task13_FileIntegrityCheck()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 13: Проверка целостности файлов ---");

            string originalFile = Path.Combine(fsManager!.GetBasePath(), "task13_original.txt");
            string copyFile = Path.Combine(fsManager.GetBasePath(), "task13_copy.txt");

            // Создаём исходный файл
            string content = "Тестовые данные для проверки целостности файлов";
            File.WriteAllText(originalFile, content);

            // Копируем
            File.Copy(originalFile, copyFile);

            // Вычисляем хеши
            string originalHash = ComputeFileHash(originalFile);
            string copyHash = ComputeFileHash(copyFile);

            Console.WriteLine($"  Хеш исходного файла: {originalHash}");
            Console.WriteLine($"  Хеш копии файла: {copyHash}");
            Console.WriteLine($"  Целостность подтверждена: {originalHash == copyHash}");

            // Модифицируем копию
            File.AppendAllText(copyFile, "\nМодифицированные данные");
            string modifiedHash = ComputeFileHash(copyFile);
            Console.WriteLine($"  Хеш модифицированного файла: {modifiedHash}");
            Console.WriteLine($"  Целостность нарушена: {originalHash != modifiedHash}");

            fsManager.RemoveFile("task13_original.txt");
            fsManager.RemoveFile("task13_copy.txt");
        }

        // Задание 14: Шифрование и дешифрование
        static void Task14_EncryptionDecryption()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 14: Шифрование и дешифрование данных в файлах ---");

            string originalFile = Path.Combine(fsManager!.GetBasePath(), "task14_original.txt");
            string encryptedFile = Path.Combine(fsManager.GetBasePath(), "task14_encrypted.bin");
            string decryptedFile = Path.Combine(fsManager.GetBasePath(), "task14_decrypted.txt");

            // Создаём исходный файл
            string originalContent = "Секретные данные, которые необходимо зашифровать.\n";
            originalContent += "Это тестовое сообщение для проверки шифрования AES.\n";
            originalContent += $"Время шифрования: {DateTime.Now}";
            File.WriteAllText(originalFile, originalContent);

            Console.WriteLine($"  Исходное содержимое: {originalContent}");

            // Шифрование
            FileEncryptor.EncryptFile(originalFile, encryptedFile);

            // Показываем зашифрованные данные (бинарные)
            byte[] encryptedData = File.ReadAllBytes(encryptedFile);
            string encryptedPreview = BitConverter.ToString(encryptedData.Take(50).ToArray());
            Console.WriteLine($"  Зашифрованные данные (первые 50 байт): {encryptedPreview}");

            // Дешифрование
            FileEncryptor.DecryptFile(encryptedFile, decryptedFile);

            // Проверяем расшифрованные данные
            string decryptedContent = File.ReadAllText(decryptedFile);
            Console.WriteLine($"  Расшифрованное содержимое: {decryptedContent}");
            Console.WriteLine($"  ");
            Console.WriteLine($"  Шифрование/дешифрование успешно: {originalContent == decryptedContent}");

            // Очистка
            fsManager.RemoveFile("task14_original.txt");
            fsManager.RemoveFile("task14_encrypted.bin");
            fsManager.RemoveFile("task14_decrypted.txt");
        }

        // Вспомогательный метод для вычисления хеша файла
        static string ComputeFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    // Вспомогательный класс для автоматического удаления файлов
    // sisa
    
    public class TempFileWrapper : IDisposable
    {
        private string filePath;
        private bool disposed = false;

        public TempFileWrapper(string path)
        {
            filePath = path;
        }

        public void Dispose()
        {
            if (!disposed && File.Exists(filePath))
            {
                File.Delete(filePath);
                disposed = true;
            }
        }
    }
}