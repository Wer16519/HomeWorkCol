using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Data.SQLite;

namespace AsyncIOPractice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== ПРАКТИЧЕСКОЕ ЗАНЯТИЕ 23 ===");
            Console.WriteLine("Реализация механизма асинхронного ввода/вывода\n");

            await Task1_ReadFileAsync("test_file.txt");
            await Task2_PrintAsync("Асинхронный вывод на экран");
            await Task3_WriteFileAsync("output.txt", "Это данные, записанные асинхронно");
            await Task4_ReadFromDatabaseAsync();        
            await Task5_SendDataAsync("https://jsonplaceholder.typicode.com/posts", "{\"title\":\"test\",\"body\":\"content\",\"userId\":1}");
            await Task6_DownloadFileAsync("https://example.com/sample.pdf", "downloaded_file.pdf");
            await Task7_ParallelIOAsync();
            await Task8_CopyFileAsync("source.txt", "destination.txt");
            await Task9_EncryptDecryptAsync("Секретные данные для шифрования");
            await Task10_ProcessImageAsync("input.jpg", "output.jpg");
            await Task11_StreamOperationsAsync();
            await Task12_AccelerateWithAsyncIOAsync();

            Console.WriteLine("\nВсе задания выполнены!");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static async Task Task1_ReadFileAsync(string filename)
        {
            Console.WriteLine("\n--- Задание 1: Асинхронное чтение файла ---");
            try
            {
                if (!File.Exists(filename))
                    await File.WriteAllTextAsync(filename, "Пример содержимого файла для асинхронного чтения.");

                string content = await File.ReadAllTextAsync(filename);
                Console.WriteLine($"Файл '{filename}' прочитан успешно!");
                Console.WriteLine($"Содержимое: {content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static async Task Task2_PrintAsync(string message)
        {
            Console.WriteLine("\n--- Задание 2: Асинхронный вывод на экран ---");
            await Task.Run(() => Console.WriteLine($"Вывод: {message}"));
        }

        static async Task Task3_WriteFileAsync(string filename, string content)
        {
            Console.WriteLine("\n--- Задание 3: Асинхронная запись в файл ---");
            await File.WriteAllTextAsync(filename, content);
            Console.WriteLine($"Данные записаны в файл '{filename}'");

            string readContent = await File.ReadAllTextAsync(filename);
            Console.WriteLine($"Прочитано из файла: {readContent}");
        }

        static async Task Task4_ReadFromDatabaseAsync()
        {
            Console.WriteLine("\n--- Задание 4: Асинхронное чтение из БД (SQLite) ---");

            string dbPath = "users.db";
            string connectionString = $"Data Source={dbPath};Version=3;";

            await Task.Run(async () =>
            {
                try
                {
                    if (!File.Exists(dbPath))
                    {
                        Console.WriteLine("Создание файла базы данных users.db...");
                        SQLiteConnection.CreateFile(dbPath);
                    }

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        string createTableQuery = @"
                            CREATE TABLE IF NOT EXISTS Users (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Name TEXT NOT NULL,
                                Age INTEGER NOT NULL
                            )";

                        using (var command = new SQLiteCommand(createTableQuery, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }

                        string checkQuery = "SELECT COUNT(*) FROM Users";
                        using (var checkCmd = new SQLiteCommand(checkQuery, connection))
                        {
                            long count = (long)await checkCmd.ExecuteScalarAsync();

                            if (count == 0)
                            {
                                string insertQuery = @"
                                    INSERT INTO Users (Name, Age) VALUES 
                                    ('Иван', 25),
                                    ('Мария', 30),
                                    ('Петр', 28),
                                    ('Анна', 22),
                                    ('Сергей', 35)";

                                using (var insertCmd = new SQLiteCommand(insertQuery, connection))
                                {
                                    await insertCmd.ExecuteNonQueryAsync();
                                }
                                Console.WriteLine("Добавлены тестовые данные в БД");
                            }
                        }

                        string selectQuery = "SELECT Id, Name, Age FROM Users";
                        using (var command = new SQLiteCommand(selectQuery, connection))
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            Console.WriteLine("\nДанные из базы данных users.db:");
                            Console.WriteLine("--------------------------------");

                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                int age = reader.GetInt32(2);

                                Console.WriteLine($"ID: {id}, Имя: {name}, Возраст: {age}");
                            }
                            Console.WriteLine("--------------------------------");
                        }

                        string avgQuery = "SELECT AVG(Age) FROM Users";
                        using (var avgCmd = new SQLiteCommand(avgQuery, connection))
                        {
                            double avgAge = Convert.ToDouble(await avgCmd.ExecuteScalarAsync());
                            Console.WriteLine($"\nСредний возраст пользователей: {avgAge:F1} лет");
                        }
                    }

                    Console.WriteLine($"\nЧтение из БД завершено. Файл БД: {Path.GetFullPath(dbPath)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при работе с БД: {ex.Message}");
                }
            });
        }

        static async Task Task5_SendDataAsync(string url, string jsonData)
        {
            Console.WriteLine("\n--- Задание 5: Асинхронная отправка данных по сети ---");
            try
            {
                using HttpClient client = new HttpClient();
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Данные отправлены. Ответ сервера: {responseBody.Substring(0, Math.Min(100, responseBody.Length))}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке: {ex.Message}");
            }
        }

        static async Task Task6_DownloadFileAsync(string url, string outputPath)
        {
            Console.WriteLine("\n--- Задание 6: Асинхронное скачивание файла ---");
            try
            {
                using HttpClient client = new HttpClient();
                byte[] fileData = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(outputPath, fileData);
                Console.WriteLine($"Файл скачан: {outputPath} (размер: {fileData.Length} байт)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка скачивания: {ex.Message}");
                await File.WriteAllTextAsync(outputPath, "Тестовое содержимое");
                Console.WriteLine($"Создан тестовый файл: {outputPath}");
            }
        }

        static async Task Task7_ParallelIOAsync()
        {
            Console.WriteLine("\n--- Задание 7: Параллельные асинхронные операции ---");

            Task task1 = Task.Run(async () => { await Task.Delay(500); Console.WriteLine("  Операция 1 завершена"); });
            Task task2 = Task.Run(async () => { await Task.Delay(300); Console.WriteLine("  Операция 2 завершена"); });
            Task task3 = Task.Run(async () => { await Task.Delay(700); Console.WriteLine("  Операция 3 завершена"); });

            await Task.WhenAll(task1, task2, task3);
            Console.WriteLine("Все параллельные операции завершены");
        }

        static async Task Task8_CopyFileAsync(string sourcePath, string destPath)
        {
            Console.WriteLine("\n--- Задание 8: Асинхронное копирование файла ---");
            try
            {
                if (!File.Exists(sourcePath))
                    await File.WriteAllTextAsync(sourcePath, "Исходное содержимое для копирования");

                using FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
                using FileStream destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write);

                await sourceStream.CopyToAsync(destStream);
                Console.WriteLine($"Файл скопирован из '{sourcePath}' в '{destPath}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка копирования: {ex.Message}");
            }
        }

        static async Task Task9_EncryptDecryptAsync(string plainText)
        {
            Console.WriteLine("\n--- Задание 9: Асинхронное шифрование/дешифрование ---");

            await Task.Run(() =>
            {
                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
                aes.IV = Encoding.UTF8.GetBytes("1234567890123456");

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                string encryptedText = Convert.ToBase64String(encryptedBytes);

                Console.WriteLine($"Исходный текст: {plainText}");
                Console.WriteLine($"Зашифрованный текст: {encryptedText}");

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] encryptedData = Convert.FromBase64String(encryptedText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

                Console.WriteLine($"Дешифрованный текст: {decryptedText}");
            });
        }

        static async Task Task10_ProcessImageAsync(string inputPath, string outputPath)
        {
            Console.WriteLine("\n--- Задание 10: Асинхронная обработка изображений ---");

            await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(inputPath))
                    {
                        using Bitmap testBitmap = new Bitmap(100, 100);
                        using Graphics g = Graphics.FromImage(testBitmap);
                        g.Clear(Color.Blue);
                        g.DrawString("Test", new Font("Arial", 12), Brushes.White, 10, 40);
                        testBitmap.Save(inputPath, ImageFormat.Jpeg);
                        Console.WriteLine($"Создано тестовое изображение '{inputPath}'");
                    }

                    using Bitmap image = new Bitmap(inputPath);
                    Console.WriteLine($"Исходное изображение: {image.Width}x{image.Height}");

                    using Bitmap resized = new Bitmap(image, new Size(image.Width / 2, image.Height / 2));
                    resized.Save(outputPath, ImageFormat.Jpeg);
                    Console.WriteLine($"Обработанное изображение: '{outputPath}' ({resized.Width}x{resized.Height})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            });
        }

        static async Task Task11_StreamOperationsAsync()
        {
            Console.WriteLine("\n--- Задание 11: Асинхронная работа с потоками ---");

            string testFile = "stream_test.txt";

            using (StreamWriter writer = new StreamWriter(testFile))
            {
                await writer.WriteAsync("Первая строка\n");
                await writer.WriteAsync("Вторая строка\n");
                await writer.WriteAsync("Третья строка\n");
                Console.WriteLine("Данные записаны в поток");
            }

            using (StreamReader reader = new StreamReader(testFile))
            {
                string line;
                int lineNumber = 1;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine($"Строка {lineNumber}: {line}");
                    lineNumber++;
                }
            }
        }

        static async Task Task12_AccelerateWithAsyncIOAsync()
        {
            Console.WriteLine("\n--- Задание 12: Ускорение работы через асинхронный I/O ---");

            Console.WriteLine("Сравнение синхронного и асинхронного подходов:");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 5; i++)
            {
                File.WriteAllText($"sync_file_{i}.txt", $"Содержимое файла {i}");
                string content = File.ReadAllText($"sync_file_{i}.txt");
            }
            stopwatch.Stop();
            Console.WriteLine($"Синхронный подход (5 операций): {stopwatch.ElapsedMilliseconds} мс");

            stopwatch.Restart();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    await File.WriteAllTextAsync($"async_file_{index}.txt", $"Содержимое файла {index}");
                    await File.ReadAllTextAsync($"async_file_{index}.txt");
                }));
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Асинхронный подход (5 операций): {stopwatch.ElapsedMilliseconds} мс");

            for (int i = 0; i < 5; i++)
            {
                if (File.Exists($"sync_file_{i}.txt")) File.Delete($"sync_file_{i}.txt");
                if (File.Exists($"async_file_{i}.txt")) File.Delete($"async_file_{i}.txt");
            }

            Console.WriteLine("Асинхронный подход ускоряет выполнение за счёт параллелизации I/O операций");
        }
    }
}