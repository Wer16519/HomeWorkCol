using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task36_FileSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Поиск файлов в директории ===\n");

            Console.Write("Введите путь к директории: ");
            string directory = Console.ReadLine();

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Директория не найдена!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nВыберите тип поиска:");
            Console.WriteLine("1 - Поиск по маске");
            Console.WriteLine("2 - Поиск скрытых файлов");
            Console.WriteLine("3 - Поиск по размеру");
            Console.WriteLine("4 - Поиск по дате изменения");
            Console.WriteLine("5 - Все файлы с фильтрацией");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    SearchByMask(directory);
                    break;
                case "2":
                    SearchHiddenFiles(directory);
                    break;
                case "3":
                    SearchBySize(directory);
                    break;
                case "4":
                    SearchByDate(directory);
                    break;
                case "5":
                    SearchAllWithFilters(directory);
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }

        static void SearchByMask(string directory)
        {
            Console.Write("Введите маску поиска (например, *.txt, *.exe): ");
            string mask = Console.ReadLine();

            try
            {
                string[] files = Directory.GetFiles(directory, mask, SearchOption.AllDirectories);
                Console.WriteLine($"\nНайдено файлов: {files.Length}");
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

        static void SearchHiddenFiles(string directory)
        {
            try
            {
                string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                List<string> hiddenFiles = new List<string>();

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    if ((info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        hiddenFiles.Add(file);
                    }
                }

                Console.WriteLine($"\nНайдено скрытых файлов: {hiddenFiles.Count}");
                foreach (string file in hiddenFiles)
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

        static void SearchBySize(string directory)
        {
            Console.Write("Минимальный размер (байт): ");
            long minSize = long.Parse(Console.ReadLine());
            Console.Write("Максимальный размер (байт): ");
            long maxSize = long.Parse(Console.ReadLine());

            try
            {
                string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                List<string> filteredFiles = new List<string>();

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    if (info.Length >= minSize && info.Length <= maxSize)
                    {
                        filteredFiles.Add(file);
                    }
                }

                Console.WriteLine($"\nНайдено файлов: {filteredFiles.Count}");
                foreach (string file in filteredFiles)
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

        static void SearchByDate(string directory)
        {
            Console.Write("Дата изменения (дд.мм.гггг): ");
            DateTime date = DateTime.Parse(Console.ReadLine());

            try
            {
                string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                List<string> filteredFiles = new List<string>();

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    if (info.LastWriteTime.Date == date.Date)
                    {
                        filteredFiles.Add(file);
                    }
                }

                Console.WriteLine($"\nНайдено файлов: {filteredFiles.Count}");
                foreach (string file in filteredFiles)
                {
                    FileInfo info = new FileInfo(file);
                    Console.WriteLine($"  {Path.GetFileName(file)} ({info.LastWriteTime:dd.MM.yyyy HH:mm})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SearchAllWithFilters(string directory)
        {
            Console.WriteLine("Фильтры для поиска:");

            Console.Write("Маска (например, *.txt, или Enter для пропуска): ");
            string mask = Console.ReadLine();
            if (string.IsNullOrEmpty(mask)) mask = "*.*";

            Console.Write("Искать скрытые файлы? (да/нет): ");
            bool includeHidden = Console.ReadLine().ToLower() == "да";

            Console.Write("Минимальный размер (байт, Enter для пропуска): ");
            string minSizeInput = Console.ReadLine();
            long minSize = string.IsNullOrEmpty(minSizeInput) ? 0 : long.Parse(minSizeInput);

            Console.Write("Максимальный размер (байт, Enter для пропуска): ");
            string maxSizeInput = Console.ReadLine();
            long maxSize = string.IsNullOrEmpty(maxSizeInput) ? long.MaxValue : long.Parse(maxSizeInput);

            try
            {
                string[] files = Directory.GetFiles(directory, mask, SearchOption.AllDirectories);
                List<string> filteredFiles = new List<string>();

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);

                    bool isHidden = (info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                    if (!includeHidden && isHidden) continue;

                    if (info.Length < minSize || info.Length > maxSize) continue;

                    filteredFiles.Add(file);
                }

                Console.WriteLine($"\nНайдено файлов: {filteredFiles.Count}");
                foreach (string file in filteredFiles)
                {
                    FileInfo info = new FileInfo(file);
                    string attributes = "";
                    if ((info.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        attributes += "[Скрытый] ";
                    if ((info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        attributes += "[Только чтение] ";

                    Console.WriteLine($"  {Path.GetFileName(file)} ({info.Length} байт) {attributes}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}