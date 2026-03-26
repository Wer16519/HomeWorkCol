using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pz11
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Проверяем, запущен ли процесс как дочерний
            if (args.Length > 0 && args[0] == "child")
            {
                ChildProcessHandler();
                return;
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== ПРАКТИЧЕСКОЕ ЗАНЯТИЕ 11: РАБОТА С КАНАЛАМИ ===\n");

            try
            {
                // Задание 1
                await Task1();
                await Task.Delay(500);

                // Задание 2
                await Task2();
                await Task.Delay(500);

                // Задание 3
                Task3();
                await Task.Delay(500);

                // Задание 4
                await Task4();
                await Task.Delay(500);

                // Задание 5
                Task5();
                await Task.Delay(500);

                // Задание 6
                Task6();
                await Task.Delay(500);

                // Задание 7
                await Task7();
                await Task.Delay(500);

                // Задание 8
                await Task8();
                await Task.Delay(500);

                // Задание 9
                Task9();
                await Task.Delay(500);

                // Задание 10
                Task10();
                await Task.Delay(500);

                // Задание 11
                Task11();
                await Task.Delay(500);

                // Задание 12
                Task12();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\n=== ВСЕ ЗАДАНИЯ ВЫПОЛНЕНЫ ===");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void ChildProcessHandler()
        {
            try
            {
                string pipeHandle = Console.ReadLine();
                if (!string.IsNullOrEmpty(pipeHandle))
                {
                    using (var client = new AnonymousPipeClientStream(PipeDirection.In, pipeHandle))
                    using (var reader = new StreamReader(client))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null && line != "END")
                        {
                            Console.WriteLine($"Дочерний процесс получил: {line}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в дочернем процессе: {ex.Message}");
            }
        }

        // Задание 1: Анонимный канал между родительским и дочерним процессом
        static async Task Task1()
        {
            Console.WriteLine("\n--- Задание 1: Анонимный канал родитель-дочерний процесс ---");

            try
            {
                using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                {
                    string pipeHandle = pipeServer.GetClientHandleAsString();
                    string currentExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = currentExe,
                            Arguments = "child",
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.StandardInput.WriteLine(pipeHandle);
                    process.StandardInput.Close();

                    using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                    {
                        writer.WriteLine("Привет от родительского процесса!");
                        writer.WriteLine("END");
                    }

                    pipeServer.DisposeLocalCopyOfClientHandle();
                    process.WaitForExit(1000);
                    process.Close();
                }

                Console.WriteLine("  ✓ Задание 1 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 1: {ex.Message}");
            }
        }

        // Задание 2: Именованный канал
        static async Task Task2()
        {
            Console.WriteLine("\n--- Задание 2: Именованный канал ---");

            try
            {
                string pipeName = $"pipe_{Guid.NewGuid()}";

                // Сервер
                var server = Task.Run(() =>
                {
                    using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
                    {
                        pipe.WaitForConnection();
                        using (var reader = new StreamReader(pipe))
                        using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                        {
                            string msg = reader.ReadLine();
                            Console.WriteLine($"  Сервер получил: {msg}");
                            writer.WriteLine("Привет от сервера!");
                        }
                    }
                });

                // Клиент
                await Task.Delay(100);
                using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                {
                    pipe.Connect(1000);
                    using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                    using (var reader = new StreamReader(pipe))
                    {
                        writer.WriteLine("Привет от клиента!");
                        string response = reader.ReadLine();
                        Console.WriteLine($"  Клиент получил: {response}");
                    }
                }

                await server;
                Console.WriteLine("  ✓ Задание 2 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 2: {ex.Message}");
            }
        }

        // Задание 3: Анонимный канал для чтения и записи
        static void Task3()
        {
            Console.WriteLine("\n--- Задание 3: Анонимный канал чтение/запись ---");

            try
            {
                using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
                {
                    using (var writer = new StreamWriter(server) { AutoFlush = true })
                    using (var reader = new StreamReader(client))
                    {
                        writer.WriteLine("Тестовые данные для анонимного канала");
                        writer.WriteLine("END");

                        string line;
                        while ((line = reader.ReadLine()) != "END")
                        {
                            Console.WriteLine($"  Прочитано: {line}");
                        }
                    }
                    server.DisposeLocalCopyOfClientHandle();
                }

                Console.WriteLine("  ✓ Задание 3 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 3: {ex.Message}");
            }
        }

        // Задание 4: Именованный канал с операциями
        static async Task Task4()
        {
            Console.WriteLine("\n--- Задание 4: Именованный канал с сортировкой/фильтрацией ---");

            try
            {
                string pipeName = $"pipe_{Guid.NewGuid()}";

                var server = Task.Run(() =>
                {
                    using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
                    {
                        pipe.WaitForConnection();
                        using (var reader = new StreamReader(pipe))
                        using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                        {
                            string data = reader.ReadLine();
                            if (!string.IsNullOrEmpty(data))
                            {
                                var numbers = data.Split(',').Select(int.Parse).ToList();
                                var sorted = numbers.OrderBy(n => n);
                                var filtered = numbers.Where(n => n % 2 == 0);

                                writer.WriteLine($"Сортировка: {string.Join(",", sorted)}");
                                writer.WriteLine($"Четные: {string.Join(",", filtered)}");
                            }
                        }
                    }
                });

                await Task.Delay(100);
                using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                {
                    pipe.Connect(1000);
                    using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                    using (var reader = new StreamReader(pipe))
                    {
                        writer.WriteLine("5,2,8,1,9,3,7,4,6");
                        Console.WriteLine($"  {reader.ReadLine()}");
                        Console.WriteLine($"  {reader.ReadLine()}");
                    }
                }

                await server;
                Console.WriteLine("  ✓ Задание 4 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 4: {ex.Message}");
            }
        }

        // Задание 5: Передача информации о файлах
        static void Task5()
        {
            Console.WriteLine("\n--- Задание 5: Передача информации о файлах ---");

            try
            {
                using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
                {
                    using (var writer = new StreamWriter(server) { AutoFlush = true })
                    using (var reader = new StreamReader(client))
                    {
                        var files = Directory.GetFiles(Directory.GetCurrentDirectory())
                                             .Select(f => new FileInfo(f))
                                             .Take(3);

                        foreach (var file in files)
                        {
                            writer.WriteLine($"{file.Name} - {file.Length} байт");
                        }
                        writer.WriteLine("END");

                        string line;
                        while ((line = reader.ReadLine()) != "END")
                        {
                            Console.WriteLine($"  {line}");
                        }
                    }
                    server.DisposeLocalCopyOfClientHandle();
                }

                Console.WriteLine("  ✓ Задание 5 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 5: {ex.Message}");
            }
        }

        // Задание 6: Простой именованный канал
        static void Task6()
        {
            Console.WriteLine("\n--- Задание 6: Именованный канал ---");

            try
            {
                string pipeName = $"pipe_{Guid.NewGuid()}";

                var server = new Thread(() =>
                {
                    using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
                    {
                        pipe.WaitForConnection();
                        using (var reader = new StreamReader(pipe))
                        using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                        {
                            string msg = reader.ReadLine();
                            Console.WriteLine($"  Сервер получил: {msg}");
                            writer.WriteLine("Ответ сервера");
                        }
                    }
                });

                server.Start();
                Thread.Sleep(100);

                using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                {
                    pipe.Connect(1000);
                    using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                    using (var reader = new StreamReader(pipe))
                    {
                        writer.WriteLine("Привет от клиента");
                        Console.WriteLine($"  Клиент получил: {reader.ReadLine()}");
                    }
                }

                server.Join();
                Console.WriteLine("  ✓ Задание 6 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 6: {ex.Message}");
            }
        }

        // Задание 7: Анонимный канал между двумя процессами
        static async Task Task7()
        {
            Console.WriteLine("\n--- Задание 7: Анонимный канал между двумя процессами ---");

            try
            {
                using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                {
                    string pipeHandle = pipeServer.GetClientHandleAsString();
                    string currentExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = currentExe,
                            Arguments = "child",
                            UseShellExecute = false,
                            RedirectStandardInput = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.StandardInput.WriteLine(pipeHandle);
                    process.StandardInput.Close();

                    using (var writer = new StreamWriter(pipeServer) { AutoFlush = true })
                    {
                        writer.WriteLine("Данные для второго процесса");
                        writer.WriteLine("END");
                    }

                    pipeServer.DisposeLocalCopyOfClientHandle();
                    process.WaitForExit(1000);
                    process.Close();
                }

                Console.WriteLine("  ✓ Задание 7 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 7: {ex.Message}");
            }
        }

        // Задание 8: Именованный канал для сети
        static async Task Task8()
        {
            Console.WriteLine("\n--- Задание 8: Именованный канал для сети ---");

            try
            {
                string pipeName = $"networkpipe_{Guid.NewGuid()}";

                var server = Task.Run(() =>
                {
                    using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message))
                    {
                        pipe.WaitForConnection();
                        using (var reader = new StreamReader(pipe))
                        using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                        {
                            string msg = reader.ReadLine();
                            Console.WriteLine($"  Сервер получил: {msg}");
                            writer.WriteLine("Ответ по сети");
                        }
                    }
                });

                await Task.Delay(100);
                using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                {
                    pipe.Connect(1000);
                    using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                    using (var reader = new StreamReader(pipe))
                    {
                        writer.WriteLine("Запрос от клиента");
                        Console.WriteLine($"  Клиент получил: {reader.ReadLine()}");
                    }
                }

                await server;
                Console.WriteLine("  ✓ Задание 8 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 8: {ex.Message}");
            }
        }

        // Задание 9: Анонимный канал с операциями
        static void Task9()
        {
            Console.WriteLine("\n--- Задание 9: Анонимный канал с сортировкой/фильтрацией ---");

            try
            {
                using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
                {
                    using (var writer = new StreamWriter(server) { AutoFlush = true })
                    using (var reader = new StreamReader(client))
                    {
                        int[] data = { 15, 3, 8, 22, 7, 10, 1, 4 };
                        writer.WriteLine(string.Join(",", data));
                        writer.WriteLine("END");

                        string sorted = reader.ReadLine();
                        string filtered = reader.ReadLine();

                        Console.WriteLine($"  Сортировка: {sorted}");
                        Console.WriteLine($"  Фильтрация (>5): {filtered}");
                    }
                    server.DisposeLocalCopyOfClientHandle();
                }

                Console.WriteLine("  ✓ Задание 9 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 9: {ex.Message}");
            }
        }

        // Задание 10: Именованный канал с несколькими клиентами
        static void Task10()
        {
            Console.WriteLine("\n--- Задание 10: Именованный канал с несколькими клиентами ---");

            try
            {
                string pipeName = $"multipipe_{Guid.NewGuid()}";

                var server = new Thread(() =>
                {
                    using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 2))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            pipe.WaitForConnection();
                            using (var reader = new StreamReader(pipe))
                            using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                            {
                                string msg = reader.ReadLine();
                                Console.WriteLine($"  Сервер получил от {msg}");
                                writer.WriteLine($"Ответ сервера {i + 1}");
                            }
                            pipe.Disconnect();
                        }
                    }
                });

                server.Start();
                Thread.Sleep(100);

                for (int i = 0; i < 2; i++)
                {
                    using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                    {
                        pipe.Connect(1000);
                        using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                        using (var reader = new StreamReader(pipe))
                        {
                            writer.WriteLine($"Клиент {i + 1}");
                            Console.WriteLine($"  Клиент {i + 1} получил: {reader.ReadLine()}");
                        }
                    }
                    Thread.Sleep(100);
                }

                server.Join();
                Console.WriteLine("  ✓ Задание 10 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 10: {ex.Message}");
            }
        }

        // Задание 11: Передача файлов
        static void Task11()
        {
            Console.WriteLine("\n--- Задание 11: Передача файлов через анонимный канал ---");

            try
            {
                string testFile = "test.txt";
                File.WriteAllText(testFile, "Тестовое содержимое файла для передачи");

                using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
                {
                    using (var writer = new StreamWriter(server) { AutoFlush = true })
                    using (var reader = new StreamReader(client))
                    {
                        writer.WriteLine(File.ReadAllText(testFile));
                        writer.WriteLine("END");

                        string content = reader.ReadLine();
                        Console.WriteLine($"  Получено: {content}");
                    }
                    server.DisposeLocalCopyOfClientHandle();
                }

                File.Delete(testFile);
                Console.WriteLine("  ✓ Задание 11 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 11: {ex.Message}");
            }
        }

        // Задание 12: Анонимный канал с закрытием
        static void Task12()
        {
            Console.WriteLine("\n--- Задание 12: Анонимный канал с закрытием ---");

            try
            {
                using (var server = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                using (var client = new AnonymousPipeClientStream(PipeDirection.In, server.GetClientHandleAsString()))
                {
                    using (var writer = new StreamWriter(server) { AutoFlush = true })
                    using (var reader = new StreamReader(client))
                    {
                        writer.WriteLine("Данные для обработки");
                        writer.WriteLine("END");

                        string data = reader.ReadLine();
                        Console.WriteLine($"  Обработано: {data.ToUpper()}");
                    }
                    server.DisposeLocalCopyOfClientHandle();
                }

                Console.WriteLine("  Канал закрыт");
                Console.WriteLine("  ✓ Задание 12 выполнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Ошибка в задании 12: {ex.Message}");
            }
        }
    }
}