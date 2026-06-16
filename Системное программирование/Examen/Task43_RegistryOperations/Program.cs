using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Task43_RegistryOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Работа с реестром Windows ===\n");

            Console.WriteLine("Выберите операцию:");
            Console.WriteLine("1 - Чтение ключей и значений из раздела");
            Console.WriteLine("2 - Поиск ключа по имени");
            Console.WriteLine("3 - Поиск значения по имени");
            Console.WriteLine("4 - Создание резервной копии раздела");
            Console.WriteLine("5 - Показать дерево реестра");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ReadRegistryKeys();
                    break;
                case "2":
                    SearchKeyByName();
                    break;
                case "3":
                    SearchValueByName();
                    break;
                case "4":
                    BackupRegistryKey();
                    break;
                case "5":
                    ShowRegistryTree();
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }

        static void ReadRegistryKeys()
        {
            Console.WriteLine("Доступные корневые разделы:");
            Console.WriteLine("1 - HKEY_CURRENT_USER");
            Console.WriteLine("2 - HKEY_LOCAL_MACHINE");
            Console.Write("Выберите раздел: ");

            string choice = Console.ReadLine();
            RegistryKey rootKey = choice == "1" ? Registry.CurrentUser : Registry.LocalMachine;

            Console.Write("Введите путь к подразделу (например, Software): ");
            string subPath = Console.ReadLine();

            try
            {
                using (RegistryKey key = rootKey.OpenSubKey(subPath))
                {
                    if (key == null)
                    {
                        Console.WriteLine("Раздел не найден!");
                        return;
                    }

                    Console.WriteLine($"\n=== Раздел: {subPath} ===");
                    Console.WriteLine($"Значений: {key.ValueCount}");
                    Console.WriteLine($"Подразделов: {key.SubKeyCount}");

                    Console.WriteLine("\n--- Значения ---");
                    foreach (string valueName in key.GetValueNames())
                    {
                        object value = key.GetValue(valueName);
                        Console.WriteLine($"  {valueName} = {value} (Тип: {value?.GetType().Name ?? "null"})");
                    }

                    Console.WriteLine("\n--- Подразделы ---");
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        Console.WriteLine($"  {subKeyName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchKeyByName()
        {
            Console.Write("Введите имя ключа для поиска: ");
            string searchName = Console.ReadLine();

            Console.WriteLine("Поиск в HKEY_CURRENT_USER и HKEY_LOCAL_MACHINE...");

            SearchKeyRecursive(Registry.CurrentUser, searchName, "HKEY_CURRENT_USER");
            SearchKeyRecursive(Registry.LocalMachine, searchName, "HKEY_LOCAL_MACHINE");
        }

        static void SearchKeyRecursive(RegistryKey root, string searchName, string path)
        {
            try
            {
                foreach (string subKeyName in root.GetSubKeyNames())
                {
                    string currentPath = path + "\\" + subKeyName;

                    if (subKeyName.ToLower().Contains(searchName.ToLower()))
                    {
                        Console.WriteLine($"Найден ключ: {currentPath}");
                    }

                    using (RegistryKey subKey = root.OpenSubKey(subKeyName))
                    {
                        if (subKey != null)
                        {
                            SearchKeyRecursive(subKey, searchName, currentPath);
                        }
                    }
                }
            }
            catch { }
        }

        static void SearchValueByName()
        {
            Console.Write("Введите имя значения для поиска: ");
            string searchName = Console.ReadLine();

            Console.WriteLine("Поиск в HKEY_CURRENT_USER и HKEY_LOCAL_MACHINE...");

            SearchValueRecursive(Registry.CurrentUser, searchName, "HKEY_CURRENT_USER");
            SearchValueRecursive(Registry.LocalMachine, searchName, "HKEY_LOCAL_MACHINE");
        }

        static void SearchValueRecursive(RegistryKey root, string searchName, string path)
        {
            try
            {
                foreach (string valueName in root.GetValueNames())
                {
                    if (valueName.ToLower().Contains(searchName.ToLower()))
                    {
                        object value = root.GetValue(valueName);
                        Console.WriteLine($"Найдено значение: {path}\\{valueName} = {value}");
                    }
                }

                foreach (string subKeyName in root.GetSubKeyNames())
                {
                    using (RegistryKey subKey = root.OpenSubKey(subKeyName))
                    {
                        if (subKey != null)
                        {
                            SearchValueRecursive(subKey, searchName, path + "\\" + subKeyName);
                        }
                    }
                }
            }
            catch { }
        }

        static void BackupRegistryKey()
        {
            Console.WriteLine("Доступные корневые разделы:");
            Console.WriteLine("1 - HKEY_CURRENT_USER");
            Console.WriteLine("2 - HKEY_LOCAL_MACHINE");
            Console.Write("Выберите раздел: ");

            string choice = Console.ReadLine();
            RegistryKey rootKey = choice == "1" ? Registry.CurrentUser : Registry.LocalMachine;

            Console.Write("Введите путь к разделу для резервного копирования: ");
            string subPath = Console.ReadLine();

            Console.Write("Введите имя файла для сохранения: ");
            string fileName = Console.ReadLine();

            try
            {
                using (RegistryKey key = rootKey.OpenSubKey(subPath))
                {
                    if (key == null)
                    {
                        Console.WriteLine("Раздел не найден!");
                        return;
                    }

                    Dictionary<string, object> backupData = new Dictionary<string, object>();

                    foreach (string valueName in key.GetValueNames())
                    {
                        backupData[valueName] = key.GetValue(valueName);
                    }

                    string filePath = fileName + ".regbackup";
                    System.IO.File.WriteAllText(filePath, "=== Резервная копия реестра ===\n");
                    System.IO.File.AppendAllText(filePath, $"Раздел: {subPath}\n");
                    System.IO.File.AppendAllText(filePath, $"Дата: {DateTime.Now}\n");
                    System.IO.File.AppendAllText(filePath, "=== Значения ===\n");

                    foreach (var item in backupData)
                    {
                        System.IO.File.AppendAllText(filePath, $"{item.Key} = {item.Value}\n");
                    }

                    Console.WriteLine($"Резервная копия сохранена в: {filePath}");
                    Console.WriteLine($"Количество сохраненных значений: {backupData.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ShowRegistryTree()
        {
            Console.WriteLine("Доступные корневые разделы:");
            Console.WriteLine("1 - HKEY_CURRENT_USER");
            Console.WriteLine("2 - HKEY_LOCAL_MACHINE");
            Console.Write("Выберите раздел: ");

            string choice = Console.ReadLine();
            RegistryKey rootKey = choice == "1" ? Registry.CurrentUser : Registry.LocalMachine;

            Console.Write("Введите максимальную глубину (1-3): ");
            int maxDepth = int.Parse(Console.ReadLine());

            Console.WriteLine("\nДерево реестра:");
            Console.WriteLine("=========================");

            string rootName = choice == "1" ? "HKEY_CURRENT_USER" : "HKEY_LOCAL_MACHINE";
            PrintRegistryTree(rootKey, rootName, 0, maxDepth);
        }

        static void PrintRegistryTree(RegistryKey key, string path, int depth, int maxDepth)
        {
            if (depth > maxDepth) return;

            string indent = new string(' ', depth * 2);

            try
            {
                Console.WriteLine($"{indent}├── {path}");

                int count = 0;
                foreach (string subKeyName in key.GetSubKeyNames())
                {
                    if (count >= 5 && depth < maxDepth)
                    {
                        Console.WriteLine($"{indent}│   ... и еще {key.SubKeyCount - 5} подразделов");
                        break;
                    }

                    using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey != null)
                        {
                            PrintRegistryTree(subKey, subKeyName, depth + 1, maxDepth);
                        }
                    }
                    count++;
                }
            }
            catch { }
        }
    }
}