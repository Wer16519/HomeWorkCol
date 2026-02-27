using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pz22
{
    class Program
    {
        static string directory = @"C:\Test";    

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Папка {directory} не существует. Создайте её и добавьте файлы.");
                return;
            }

            Console.WriteLine("=== ПРАКТИЧЕСКОЕ ЗАНЯТИЕ 22: ПОИСК ФАЙЛОВ ===\n");

            Task1();    
            Task2();    
            Task3();    
            Task4();    
            Task5();     
            Task6();    
            Task7();   
            Task8();     
            Task9();   
            Task10();    
            Task11();    
            Task12();    
            Task13();     
            Task14();    
            Task15();    

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static string[] GetAllFiles()
        {
            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        }

        static void Task1()
        {
            Console.WriteLine("1. Поиск по шаблону *.txt:");
            string[] files = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
            foreach (string f in files) Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task2()
        {
            Console.WriteLine("2. Поиск файла test.txt:");
            foreach (string f in GetAllFiles())
                if (Path.GetFileName(f).ToLower() == "test.txt")
                    Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task3()
        {
            Console.WriteLine("3. Файлы с текстом 'Hello':");
            foreach (string f in GetAllFiles())
            {
                try
                {
                    if (File.ReadAllText(f).Contains("Hello"))
                        Console.WriteLine("   " + f);
                }
                catch { }
            }
            Console.WriteLine();
        }

        static void Task4()
        {
            Console.WriteLine($"4. Все файлы в {directory}:");
            foreach (string f in GetAllFiles())
                Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task5()
        {
            Console.WriteLine("5. Файлы .txt и .log:");
            string[] masks = { "*.txt", "*.log" };
            foreach (string mask in masks)
                foreach (string f in Directory.GetFiles(directory, mask, SearchOption.AllDirectories))
                    Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task6()
        {
            Console.WriteLine("6. Скрытые файлы:");
            foreach (string f in GetAllFiles())
                if ((File.GetAttributes(f) & FileAttributes.Hidden) == FileAttributes.Hidden)
                    Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task7()
        {
            Console.WriteLine("7. Только .exe файлы:");
            foreach (string f in GetAllFiles())
                if (Path.GetExtension(f).ToLower() == ".exe")
                    Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task8()
        {
            Console.WriteLine("8. Файлы созданные сегодня:");
            foreach (string f in GetAllFiles())
                if (File.GetCreationTime(f).Date == DateTime.Today)
                    Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task9()
        {
            Console.WriteLine("9. Дубликаты файлов (одинаковый размер):");
            var files = GetAllFiles();
            for (int i = 0; i < files.Length; i++)
            {
                for (int j = i + 1; j < files.Length; j++)
                {
                    if (new FileInfo(files[i]).Length == new FileInfo(files[j]).Length)
                        Console.WriteLine($"   {files[i]} = {files[j]}");
                }
            }
            Console.WriteLine();
        }

        static void Task10()
        {
            Console.WriteLine("10. Пустые файлы (0 байт):");
            foreach (string f in GetAllFiles())
                if (new FileInfo(f).Length == 0)
                    Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task11()
        {
            Console.WriteLine("11. Поврежденные файлы (пустые или ошибки):");
            foreach (string f in GetAllFiles())
            {
                try
                {
                    if (new FileInfo(f).Length == 0)
                        Console.WriteLine("   " + f + " (пустой)");
                }
                catch
                {
                    Console.WriteLine("   " + f + " (ошибка доступа)");
                }
            }
            Console.WriteLine();
        }

        static void Task12()
        {
            Console.WriteLine("12. Файлы больше 1 КБ:");
            foreach (string f in GetAllFiles())
            {
                long size = new FileInfo(f).Length;
                if (size > 1024)
                    Console.WriteLine($"   {f} - {size} байт");
            }
            Console.WriteLine();
        }

        static void Task13()
        {
            Console.WriteLine("13. Файлы не измененные 30+ дней:");
            DateTime oldDate = DateTime.Now.AddDays(-30);
            foreach (string f in GetAllFiles())
                if (File.GetLastWriteTime(f) < oldDate)
                    Console.WriteLine("   " + f);
            Console.WriteLine();
        }

        static void Task14()
        {
            Console.WriteLine("14. Исполняемые файлы (.exe, .bat, .cmd):");
            string[] exts = { ".exe", ".bat", ".cmd" };
            foreach (string f in GetAllFiles())
            {
                string ext = Path.GetExtension(f).ToLower();
                if (exts.Contains(ext))
                    Console.WriteLine("   " + f);
            }
            Console.WriteLine();
        }

        static void Task15()
        {
            Console.WriteLine("15. Все найденные файлы (итог):");
            string[] all = GetAllFiles();
            foreach (string f in all)
                Console.WriteLine("   " + f);
            Console.WriteLine($"Всего файлов: {all.Length}");
        }
    }
}