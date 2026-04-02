using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO;

namespace PZ26
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int choice;
            do
            {
                Console.WriteLine("\n===== Работа с реестром Windows =====");
                Console.WriteLine("1.  Прочитать и вывести все ключи и значения из раздела");
                Console.WriteLine("2.  Добавить новый ключ и значение");
                Console.WriteLine("3.  Изменить существующее значение");
                Console.WriteLine("4.  Удалить ключ или значение");
                Console.WriteLine("5.  Переименовать ключ");
                Console.WriteLine("6.  Создать новый раздел");
                Console.WriteLine("7.  Поиск ключа или значения по критерию");
                Console.WriteLine("8.  Получить информацию о ключе (тип, размер, дата изменения)");
                Console.WriteLine("9.  Обработать ошибки при работе с реестром");
                Console.WriteLine("10. Создать резервную копию и восстановить раздел");
                Console.WriteLine("11. Работа с подразделами (создание, удаление, чтение, запись)");
                Console.WriteLine("12. Система прав доступа к разделам");
                Console.WriteLine("13. Отобразить дерево разделов в виде иерархического списка");
                Console.WriteLine("14. Использовать реестр для хранения настроек приложения");
                Console.WriteLine("15. Интеграция с Active Directory и групповыми политиками");
                Console.WriteLine("0.  Выход");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out choice)) continue;

                switch (choice)
                {
                    case 1: ReadRegistryKey(); break;
                    case 2: AddKeyAndValue(); break;
                    case 3: ModifyValue(); break;
                    case 4: DeleteKeyOrValue(); break;
                    case 5: RenameKey(); break;
                    case 6: CreateSubKey(); break;
                    case 7: SearchRegistry(); break;
                    case 8: GetKeyInfo(); break;
                    case 9: DemonstrateErrorHandling(); break;
                    case 10: BackupAndRestore(); break;
                    case 11: WorkWithSubKeys(); break;
                    case 12: SetKeySecurity(); break;
                    case 13: ShowRegistryTree(); break;
                    case 14: StoreAppSettings(); break;
                    case 15: ActiveDirectoryIntegration(); break;
                }
            } while (choice != 0);
        }

        // Задание 1: Чтение и вывод всех ключей и значений из раздела
        static void ReadRegistryKey()
        {
            Console.Write("Введите путь к разделу (например: Software\\Microsoft): ");
            string path = Console.ReadLine();

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
            {
                if (key == null)
                {
                    Console.WriteLine("Раздел не найден!");
                    return;
                }

                Console.WriteLine("\nПодразделы:");
                foreach (string subKeyName in key.GetSubKeyNames())
                {
                    Console.WriteLine($"  {subKeyName}");
                }

                Console.WriteLine("\nЗначения:");
                foreach (string valueName in key.GetValueNames())
                {
                    object value = key.GetValue(valueName);
                    Console.WriteLine($"  {valueName} = {value}");
                }
            }
        }

        // Задание 2: Добавить новый ключ и значение
        static void AddKeyAndValue()
        {
            Console.Write("Введите путь к ключу (например: Software\\MyApp): ");
            string path = Console.ReadLine();
            Console.Write("Введите имя значения: ");
            string valueName = Console.ReadLine();
            Console.Write("Введите данные значения: ");
            string valueData = Console.ReadLine();

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(path))
            {
                if (key != null)
                {
                    key.SetValue(valueName, valueData);
                    Console.WriteLine("Ключ и значение успешно добавлены.");
                }
            }
        }

        // Задание 3: Изменить существующее значение
        static void ModifyValue()
        {
            Console.Write("Введите путь к ключу: ");
            string path = Console.ReadLine();
            Console.Write("Введите имя значения для изменения: ");
            string valueName = Console.ReadLine();
            Console.Write("Введите новое значение: ");
            string newValue = Console.ReadLine();

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                if (key != null && key.GetValue(valueName) != null)
                {
                    key.SetValue(valueName, newValue);
                    Console.WriteLine("Значение изменено.");
                }
                else
                {
                    Console.WriteLine("Ключ или значение не найдены.");
                }
            }
        }

        // Задание 4: Удалить ключ или значение
        static void DeleteKeyOrValue()
        {
            Console.Write("Удалить (1 - значение, 2 - ключ): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введите путь к ключу: ");
                string path = Console.ReadLine();
                Console.Write("Введите имя значения: ");
                string valueName = Console.ReadLine();

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                {
                    if (key != null && key.GetValue(valueName) != null)
                    {
                        key.DeleteValue(valueName);
                        Console.WriteLine("Значение удалено.");
                    }
                    else Console.WriteLine("Значение не найдено.");
                }
            }
            else if (choice == "2")
            {
                Console.Write("Введите путь к удаляемому ключу: ");
                string path = Console.ReadLine();
                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(path);
                    Console.WriteLine("Ключ удалён.");
                }
                catch { Console.WriteLine("Ошибка удаления ключа."); }
            }
        }

        // Задание 5: Переименовать ключ (копирование + удаление)
        static void RenameKey()
        {
            Console.Write("Введите старый путь: ");
            string oldPath = Console.ReadLine();
            Console.Write("Введите новый путь: ");
            string newPath = Console.ReadLine();

            using (RegistryKey oldKey = Registry.CurrentUser.OpenSubKey(oldPath))
            {
                if (oldKey != null)
                {
                    using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(newPath))
                    {
                        foreach (string valueName in oldKey.GetValueNames())
                        {
                            newKey.SetValue(valueName, oldKey.GetValue(valueName), oldKey.GetValueKind(valueName));
                        }
                    }
                    Registry.CurrentUser.DeleteSubKeyTree(oldPath);
                    Console.WriteLine("Ключ переименован.");
                }
                else Console.WriteLine("Ошибка переименования.");
            }
        }

        // Задание 6: Создать новый раздел
        static void CreateSubKey()
        {
            Console.Write("Введите путь нового раздела: ");
            string path = Console.ReadLine();
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(path))
            {
                if (key != null) Console.WriteLine("Раздел создан.");
            }
        }

        // Задание 7: Поиск ключа или значения по критерию
        static void SearchRegistry()
        {
            Console.Write("Введите строку для поиска: ");
            string searchStr = Console.ReadLine();
            SearchInKey(Registry.CurrentUser, "Software", searchStr);
        }

        static void SearchInKey(RegistryKey root, string currentPath, string searchStr)
        {
            try
            {
                using (RegistryKey key = root.OpenSubKey(currentPath))
                {
                    if (key == null) return;

                    foreach (string valueName in key.GetValueNames())
                    {
                        object value = key.GetValue(valueName);
                        if (value != null && value.ToString().Contains(searchStr))
                            Console.WriteLine($"Найдено: {currentPath}\\{valueName} = {value}");

                        if (valueName.Contains(searchStr))
                            Console.WriteLine($"Найдено имя значения: {currentPath}\\{valueName}");
                    }

                    foreach (string subKey in key.GetSubKeyNames())
                    {
                        SearchInKey(root, currentPath + "\\" + subKey, searchStr);
                    }
                }
            }
            catch { }
        }

        // Задание 8: Получить информацию о ключе
        static void GetKeyInfo()
        {
            Console.Write("Введите путь к ключу: ");
            string path = Console.ReadLine();

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
            {
                if (key == null)
                {
                    Console.WriteLine("Ключ не найден.");
                    return;
                }

                int subKeyCount = key.SubKeyCount;
                int valueCount = key.ValueCount;

                Console.WriteLine($"Подразделов: {subKeyCount}");
                Console.WriteLine($"Значений: {valueCount}");

                foreach (string valueName in key.GetValueNames())
                {
                    RegistryValueKind kind = key.GetValueKind(valueName);
                    object value = key.GetValue(valueName);
                    int size = value != null ? value.ToString().Length : 0;
                    Console.WriteLine($"  {valueName} : тип={kind}, размер≈{size} байт");
                }
            }
        }

        // Задание 9: Обработка ошибок
        static void DemonstrateErrorHandling()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("NonExistentKey\\123"))
                {
                    if (key == null)
                        throw new Exception("Ключ не существует");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка: Нет доступа к разделу реестра.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Задание 10: Резервная копия и восстановление (экспорт через reg.exe)
        static void BackupAndRestore()
        {
            Console.Write("1 - Создать резервную копию, 2 - Восстановить: ");
            string choice = Console.ReadLine();
            Console.Write("Введите путь к разделу (например: HKEY_CURRENT_USER\\Software\\MyApp): ");
            string keyPath = Console.ReadLine();
            Console.Write("Имя файла (.reg): ");
            string fileName = Console.ReadLine();

            if (choice == "1")
            {
                System.Diagnostics.Process.Start("reg", $"export \"{keyPath}\" {fileName} /y");
                Console.WriteLine("Резервная копия создана.");
            }
            else if (choice == "2")
            {
                System.Diagnostics.Process.Start("reg", $"import {fileName}");
                Console.WriteLine("Восстановление выполнено.");
            }
        }

        // Задание 11: Работа с подразделами
        static void WorkWithSubKeys()
        {
            string basePath = "Software\\PracticeSub";

            // Создание подраздела
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(basePath))
            {
                key.SetValue("TestValue", "Hello");
                Console.WriteLine("Создан раздел и значение.");
            }

            // Чтение
            Console.WriteLine("\nЧтение созданного раздела:");
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(basePath))
            {
                if (key != null)
                {
                    foreach (string valueName in key.GetValueNames())
                    {
                        Console.WriteLine($"  {valueName} = {key.GetValue(valueName)}");
                    }
                }
            }

            // Удаление
            Console.Write("\nУдалить подраздел? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Registry.CurrentUser.DeleteSubKeyTree(basePath);
                Console.WriteLine("Подраздел удалён.");
            }
        }

        // Задание 12: Система прав доступа
        static void SetKeySecurity()
        {
            Console.Write("Введите путь к разделу: ");
            string path = Console.ReadLine();

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(path))
            {
                if (key != null)
                {
                    RegistrySecurity security = new RegistrySecurity();
                    string user = WindowsIdentity.GetCurrent().Name;
                    security.AddAccessRule(new RegistryAccessRule(user, RegistryRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                    key.SetAccessControl(security);
                    Console.WriteLine("Права доступа установлены.");
                }
            }
        }

        // Задание 13: Отображение дерева разделов
        static void ShowRegistryTree()
        {
            Console.WriteLine("Дерево разделов HKEY_CURRENT_USER\\Software:");
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software"))
            {
                PrintTree(key, "Software", 0);
            }
        }

        static void PrintTree(RegistryKey key, string name, int level)
        {
            if (key == null) return;
            Console.WriteLine(new string(' ', level * 2) + "+-- " + name);

            foreach (string subName in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(subName))
                {
                    PrintTree(subKey, subName, level + 1);
                }
            }
        }

        // Задание 14: Хранение настроек приложения
        static void StoreAppSettings()
        {
            string appPath = "Software\\MyAppSettings";
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(appPath))
            {
                key.SetValue("Theme", "Dark");
                key.SetValue("Language", "Russian");
                key.SetValue("LastRun", DateTime.Now.ToString());
                key.SetValue("WindowSize", 800);
                key.SetValue("IsMaximized", false);
            }
            Console.WriteLine("Настройки сохранены.");

            Console.WriteLine("\nЗагруженные настройки:");
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(appPath))
            {
                if (key != null)
                {
                    foreach (string valueName in key.GetValueNames())
                    {
                        object value = key.GetValue(valueName);
                        Console.WriteLine($"  {valueName} = {value} (тип: {key.GetValueKind(valueName)})");
                    }
                }
            }
        }

        // Задание 15: Интеграция с Active Directory (имитация)
        static void ActiveDirectoryIntegration()
        {
            Console.WriteLine("Интеграция с Active Directory:");
            Console.WriteLine("Чтение групповых политик из HKLM\\Software\\Policies...");

            using (RegistryKey policies = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\CurrentVersion"))
            {
                if (policies != null)
                {
                    Console.WriteLine("\nНайденные политики:");
                    foreach (string policyName in policies.GetValueNames())
                    {
                        Console.WriteLine($"  {policyName} = {policies.GetValue(policyName)}");
                    }
                }
                else
                {
                    Console.WriteLine("Раздел политик не найден.");
                    Console.WriteLine("\nВ реальной среде здесь можно использовать:");
                    Console.WriteLine("  - System.DirectoryServices для работы с AD");
                    Console.WriteLine("  - GroupPolicyObject для работы с GPO");
                    Console.WriteLine("  - Чтение реестровых ключей групповых политик");
                }
            }

            // Пример работы с политиками через реестр
            Console.WriteLine("\nПример чтения политик IE из реестра:");
            using (RegistryKey iePolicies = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Internet Explorer\\Main"))
            {
                if (iePolicies != null)
                {
                    foreach (string policyName in iePolicies.GetValueNames())
                    {
                        Console.WriteLine($"  {policyName} = {iePolicies.GetValue(policyName)}");
                    }
                }
                else
                {
                    Console.WriteLine("  Политики Internet Explorer не найдены.");
                }
            }
        }
    }
}