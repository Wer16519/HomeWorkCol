using System;
using System.IO;

namespace Task32_FileSystemHelper
{
    public class FileSystemHelper
    {
        public void CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Console.WriteLine($"[Создана папка] {path}");
                }
                else
                {
                    Console.WriteLine($"[Папка уже существует] {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка создания папки] {ex.Message}");
            }
        }

        public void CreateFile(string path, string content = "")
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, content);
                    Console.WriteLine($"[Создан файл] {path}");
                }
                else
                {
                    Console.WriteLine($"[Файл уже существует] {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка создания файла] {ex.Message}");
            }
        }

        public void CopyFile(string sourcePath, string destPath)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destPath, true);
                    Console.WriteLine($"[Копирован файл] {sourcePath} -> {destPath}");
                }
                else
                {
                    Console.WriteLine($"[Исходный файл не найден] {sourcePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка копирования] {ex.Message}");
            }
        }

        public void MoveFile(string sourcePath, string destPath)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    File.Move(sourcePath, destPath);
                    Console.WriteLine($"[Перемещен файл] {sourcePath} -> {destPath}");
                }
                else
                {
                    Console.WriteLine($"[Исходный файл не найден] {sourcePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка перемещения] {ex.Message}");
            }
        }

        public void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine($"[Удален файл] {path}");
                }
                else
                {
                    Console.WriteLine($"[Файл не найден] {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка удаления] {ex.Message}");
            }
        }

        public void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"[Удалена папка] {path}");
                }
                else
                {
                    Console.WriteLine($"[Папка не найдена] {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка удаления папки] {ex.Message}");
            }
        }

        public void GetFileInfo(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    Console.WriteLine($"\n[Информация о файле] {path}");
                    Console.WriteLine($"  Имя: {info.Name}");
                    Console.WriteLine($"  Размер: {info.Length} байт");
                    Console.WriteLine($"  Создан: {info.CreationTime}");
                    Console.WriteLine($"  Изменен: {info.LastWriteTime}");
                    Console.WriteLine($"  Доступ: {info.LastAccessTime}");
                    Console.WriteLine($"  Атрибуты: {info.Attributes}");
                }
                else
                {
                    Console.WriteLine($"[Файл не найден] {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка получения информации] {ex.Message}");
            }
        }

        public void GetDirectoryInfo(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    DirectoryInfo info = new DirectoryInfo(path);
                    Console.WriteLine($"\n[Информация о папке] {path}");
                    Console.WriteLine($"  Имя: {info.Name}");
                    Console.WriteLine($"  Создана: {info.CreationTime}");
                    Console.WriteLine($"  Изменена: {info.LastWriteTime}");
                    Console.WriteLine($"  Подпапок: {info.GetDirectories().Length}");
                    Console.WriteLine($"  Файлов: {info.GetFiles().Length}");
                }
                else
                {
                    Console.WriteLine($"[Папка не найдена] {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка получения информации] {ex.Message}");
            }
        }

        public void ListDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"\n[Содержимое папки] {path}");
                    Console.WriteLine("Папки:");
                    foreach (string dir in Directory.GetDirectories(path))
                    {
                        Console.WriteLine($"  {Path.GetFileName(dir)}/");
                    }
                    Console.WriteLine("Файлы:");
                    foreach (string file in Directory.GetFiles(path))
                    {
                        FileInfo info = new FileInfo(file);
                        Console.WriteLine($"  {Path.GetFileName(file)} ({info.Length} байт)");
                    }
                }
                else
                {
                    Console.WriteLine($"[Папка не найдена] {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка получения списка] {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Работа с файловой системой ===\n");

            FileSystemHelper helper = new FileSystemHelper();

            string testDir = "TestFolder";
            string testFile = Path.Combine(testDir, "test.txt");
            string copyFile = Path.Combine(testDir, "copy.txt");
            string moveFile = Path.Combine(testDir, "moved.txt");

            helper.CreateDirectory(testDir);

            helper.CreateFile(testFile, "Это тестовый файл для демонстрации работы с файловой системой.");

            helper.GetFileInfo(testFile);

            helper.CopyFile(testFile, copyFile);

            helper.GetFileInfo(copyFile);

            helper.MoveFile(copyFile, moveFile);

            helper.ListDirectory(testDir);

            helper.DeleteFile(moveFile);

            helper.DeleteFile(testFile);

            helper.ListDirectory(testDir);

            helper.DeleteDirectory(testDir);

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}