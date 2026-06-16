using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Task37_AsyncFileCopy
{
    class Program
    {
        private static int _totalFiles = 0;
        private static int _copiedFiles = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Асинхронное копирование файлов ===\n");

            Console.Write("Введите путь к исходной папке: ");
            string sourceDir = Console.ReadLine();

            if (!Directory.Exists(sourceDir))
            {
                Console.WriteLine("Исходная папка не найдена!");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите путь к целевой папке: ");
            string destDir = Console.ReadLine();

            try
            {
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                    Console.WriteLine($"[Создана] Папка назначения: {destDir}");
                }

                string[] files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
                _totalFiles = files.Length;
                Console.WriteLine($"\nНайдено файлов для копирования: {_totalFiles}");

                List<Task> tasks = new List<Task>();
                int maxParallel = Math.Min(10, _totalFiles);

                using (SemaphoreSlim semaphore = new SemaphoreSlim(maxParallel))
                {
                    foreach (string file in files)
                    {
                        await semaphore.WaitAsync();
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await CopyFileAsync(file, sourceDir, destDir);
                                int copied = Interlocked.Increment(ref _copiedFiles);
                                Console.WriteLine($"[Прогресс] {copied}/{_totalFiles} - {Path.GetFileName(file)}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Ошибка] {Path.GetFileName(file)}: {ex.Message}");
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                    }

                    await Task.WhenAll(tasks);
                }

                Console.WriteLine($"\n[Завершено] Скопировано {_copiedFiles} из {_totalFiles} файлов");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] {ex.Message}");
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }

        static async Task CopyFileAsync(string sourceFile, string sourceDir, string destDir)
        {
            string relativePath = GetRelativePath(sourceDir, sourceFile);
            string destFile = Path.Combine(destDir, relativePath);

            string destFolder = Path.GetDirectoryName(destFile);
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            int maxRetries = 3;
            int retryDelay = 500;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (FileStream destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await sourceStream.CopyToAsync(destStream);
                    }
                    return;
                }
                catch (IOException) when (attempt < maxRetries)
                {
                    await Task.Delay(retryDelay * attempt);
                }
            }

            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await sourceStream.CopyToAsync(destStream);
            }
        }

        static string GetRelativePath(string basePath, string fullPath)
        {
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            Uri baseUri = new Uri(basePath);
            Uri fullUri = new Uri(fullPath);

            string relativePath = Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString());
            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}