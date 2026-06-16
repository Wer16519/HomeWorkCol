using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Task34_FileLocking
{
    public class FileLockManager
    {
        private static readonly object _lock = new object();
        private static readonly Dictionary<string, bool> _lockedFiles = new Dictionary<string, bool>();

        public async Task<bool> TryLockFileAsync(string filePath, int timeoutMs = 5000)
        {
            try
            {
                bool locked = false;
                lock (_lock)
                {
                    if (!_lockedFiles.ContainsKey(filePath) || !_lockedFiles[filePath])
                    {
                        _lockedFiles[filePath] = true;
                        locked = true;
                    }
                }

                if (locked)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                        {
                            Console.WriteLine($"[Блокировка] Файл {Path.GetFileName(filePath)} заблокирован процессом {Thread.CurrentThread.ManagedThreadId}");
                            await Task.Delay(100);
                            return true;
                        }
                    }
                    catch (IOException)
                    {
                        lock (_lock)
                        {
                            _lockedFiles[filePath] = false;
                        }
                        Console.WriteLine($"[Блокировка] Не удалось заблокировать файл {Path.GetFileName(filePath)}");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"[Блокировка] Файл {Path.GetFileName(filePath)} уже заблокирован");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка блокировки] {ex.Message}");
                return false;
            }
        }

        public void UnlockFile(string filePath)
        {
            lock (_lock)
            {
                if (_lockedFiles.ContainsKey(filePath))
                {
                    _lockedFiles[filePath] = false;
                    Console.WriteLine($"[Разблокировка] Файл {Path.GetFileName(filePath)} разблокирован");
                }
            }
        }

        public async Task WriteToFileAsync(string filePath, string content, int processId)
        {
            try
            {
                bool locked = await TryLockFileAsync(filePath);
                if (locked)
                {
                    await Task.Delay(2000);

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        await writer.WriteLineAsync($"[Процесс {processId}] {content} - {DateTime.Now:HH:mm:ss}");
                        await writer.FlushAsync();
                    }

                    Console.WriteLine($"[Запись] Процесс {processId} записал данные");
                    UnlockFile(filePath);
                }
                else
                {
                    Console.WriteLine($"[Отказ] Процесс {processId} не получил доступ к файлу");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка записи] {ex.Message}");
                UnlockFile(filePath);
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Блокировка файлов между процессами ===\n");

            string filePath = "shared_data.txt";

            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine("=== Лог операций ===");
                }
            }

            Console.WriteLine($"Файл: {filePath}\n");

            FileLockManager lockManager = new FileLockManager();

            Console.WriteLine("Запуск 5 потоков для записи в файл...\n");

            Task[] tasks = new Task[5];
            for (int i = 1; i <= 5; i++)
            {
                int processId = i;
                tasks[i - 1] = Task.Run(async () =>
                {
                    await lockManager.WriteToFileAsync(filePath, $"Сообщение от процесса {processId}", processId);
                });
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("\n[Содержимое файла после всех операций]:");
            Console.WriteLine(new string('-', 50));
            string content = File.ReadAllText(filePath);
            Console.WriteLine(content);
            Console.WriteLine(new string('-', 50));

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}