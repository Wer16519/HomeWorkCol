using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task38_AdvancedFileSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Расширенный поиск файлов ===\n");

            Console.Write("Введите путь к директории: ");
            string directory = Console.ReadLine();

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Директория не найдена!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nВыберите тип поиска:");
            Console.WriteLine("1 - Поиск дубликатов файлов");
            Console.WriteLine("2 - Поиск пустых файлов");
            Console.WriteLine("3 - Поиск файлов определенного размера");
            Console.WriteLine("4 - Поиск файлов, не измененных заданное время");
            Console.WriteLine("5 - Поиск исполняемых файлов");
            Console.WriteLine("6 - Поиск всех файлов");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    SearchDuplicates(directory);
                    break;
                case "2":
                    SearchEmptyFiles(directory);
                    break;
                case "3":
                    SearchBySize(directory);
                    break;
                case "4":
                    SearchByLastWriteTime(directory);
                    break;
                case "5":
                    SearchExecutableFiles(directory);
                    break;
                case "6":
                    SearchAllFiles(directory);
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }

        static List<string> GetAllFilesSafe(string directory)
        {
            List<string> allFiles = new List<string>();
            try
            {
                allFiles.AddRange(Directory.GetFiles(directory));
                foreach (string subDir in Directory.GetDirectories(directory))
                {
                    try
                    {
                        allFiles.AddRange(GetAllFilesSafe(subDir));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"[Пропущено] Нет доступа к папке: {subDir}");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"[Пропущено] Нет доступа к папке: {directory}");
            }
            return allFiles;
        }

        static void SearchDuplicates(string directory)
        {
            Console.WriteLine("Поиск дубликатов файлов...");

            try
            {
                List<string> files = GetAllFilesSafe(directory);
                Dictionary<long, List<string>> sizeGroups = new Dictionary<long, List<string>>();

                foreach (string file in files)
                {
                    try
                    {
                        FileInfo info = new FileInfo(file);
                        if (!sizeGroups.ContainsKey(info.Length))
                        {
                            sizeGroups[info.Length] = new List<string>();
                        }
                        sizeGroups[info.Length].Add(file);
                    }
                    catch { }
                }

                int duplicateCount = 0;
                foreach (var group in sizeGroups.Where(g => g.Value.Count > 1))
                {
                    Console.WriteLine($"\nРазмер: {group.Key} байт, файлов: {group.Value.Count}");
                    foreach (string file in group.Value)
                    {
                        Console.WriteLine($"  {file}");
                    }
                    duplicateCount += group.Value.Count;
                }

                Console.WriteLine($"\nНайдено дубликатов: {duplicateCount} файлов");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchEmptyFiles(string directory)
        {
            Console.WriteLine("Поиск пустых файлов...");

            try
            {
                List<string> files = GetAllFilesSafe(directory);
                List<string> emptyFiles = new List<string>();

                foreach (string file in files)
                {
                    try
                    {
                        FileInfo info = new FileInfo(file);
                        if (info.Length == 0)
                        {
                            emptyFiles.Add(file);
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"\nНайдено пустых файлов: {emptyFiles.Count}");
                foreach (string file in emptyFiles)
                {
                    Console.WriteLine($"  {file}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchBySize(string directory)
        {
            Console.Write("Размер файла (байт): ");
            long size = long.Parse(Console.ReadLine());

            try
            {
                List<string> files = GetAllFilesSafe(directory);
                List<string> filteredFiles = new List<string>();

                foreach (string file in files)
                {
                    try
                    {
                        FileInfo info = new FileInfo(file);
                        if (info.Length == size)
                        {
                            filteredFiles.Add(file);
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"\nНайдено файлов размером {size} байт: {filteredFiles.Count}");
                foreach (string file in filteredFiles)
                {
                    Console.WriteLine($"  {file}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchByLastWriteTime(string directory)
        {
            Console.Write("Количество дней (файлы не изменялись более N дней): ");
            int days = int.Parse(Console.ReadLine());

            DateTime threshold = DateTime.Now.AddDays(-days);

            try
            {
                List<string> files = GetAllFilesSafe(directory);
                List<string> filteredFiles = new List<string>();

                foreach (string file in files)
                {
                    try
                    {
                        FileInfo info = new FileInfo(file);
                        if (info.LastWriteTime < threshold)
                        {
                            filteredFiles.Add(file);
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"\nНайдено файлов, не изменявшихся более {days} дней: {filteredFiles.Count}");
                foreach (string file in filteredFiles)
                {
                    FileInfo info = new FileInfo(file);
                    Console.WriteLine($"  {Path.GetFileName(file)} ({info.LastWriteTime:dd.MM.yyyy})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchExecutableFiles(string directory)
        {
            Console.WriteLine("Поиск исполняемых файлов...");

            try
            {
                string[] exeExtensions = { ".exe", ".bat", ".cmd", ".com", ".msi", ".dll" };
                List<string> files = GetAllFilesSafe(directory);
                List<string> executableFiles = new List<string>();

                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (exeExtensions.Contains(ext))
                    {
                        executableFiles.Add(file);
                    }
                }

                Console.WriteLine($"\nНайдено исполняемых файлов: {executableFiles.Count}");
                foreach (string file in executableFiles)
                {
                    FileInfo info = new FileInfo(file);
                    Console.WriteLine($"  {Path.GetFileName(file)} ({info.Length} байт)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchAllFiles(string directory)
        {
            Console.WriteLine("Поиск всех файлов...");

            try
            {
                List<string> files = GetAllFilesSafe(directory);

                Console.WriteLine($"\nНайдено файлов: {files.Count}");
                Console.WriteLine($"Папка: {directory}");

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    Console.WriteLine($"  {Path.GetFileName(file)} ({info.Length} байт)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}