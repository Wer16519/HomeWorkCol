using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Task28_NamedPipes
{
    class Program
    {
        private static string pipeName = "mypipe";

        static void Main(string[] args)
        {
            Console.WriteLine("=== Именованные каналы ===\n");
            Console.WriteLine("Выберите режим:");
            Console.WriteLine("1 - Сервер");
            Console.WriteLine("2 - Клиент");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                RunServer();
            }
            else if (choice == "2")
            {
                RunClient();
            }
            else
            {
                Console.WriteLine("Неверный выбор");
            }

            Console.ReadKey();
        }

        static void RunServer()
        {
            try
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut))
                {
                    Console.WriteLine("[Сервер] Ожидание подключения клиента...");
                    pipeServer.WaitForConnection();
                    Console.WriteLine("[Сервер] Клиент подключен");

                    using (StreamReader reader = new StreamReader(pipeServer))
                    using (StreamWriter writer = new StreamWriter(pipeServer))
                    {
                        while (true)
                        {
                            string message = reader.ReadLine();

                            if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                            {
                                Console.WriteLine("[Сервер] Клиент отключился");
                                break;
                            }

                            Console.WriteLine($"[Сервер] Получено: {message}");
                            string response = $"Эхо: {message} (время: {DateTime.Now:HH:mm:ss})";
                            writer.WriteLine(response);
                            writer.Flush();
                            Console.WriteLine($"[Сервер] Отправлено: {response}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Сервер] Ошибка: {ex.Message}");
            }
        }

        static void RunClient()
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                {
                    Console.WriteLine("[Клиент] Подключение к серверу...");
                    pipeClient.Connect(5000);
                    Console.WriteLine("[Клиент] Подключен к серверу");

                    using (StreamReader reader = new StreamReader(pipeClient))
                    using (StreamWriter writer = new StreamWriter(pipeClient))
                    {
                        while (true)
                        {
                            Console.Write("[Клиент] Введите сообщение (exit для выхода): ");
                            string message = Console.ReadLine();

                            writer.WriteLine(message);
                            writer.Flush();

                            if (message.ToLower() == "exit")
                            {
                                Console.WriteLine("[Клиент] Завершение работы");
                                break;
                            }

                            string response = reader.ReadLine();
                            Console.WriteLine($"[Клиент] Ответ сервера: {response}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Клиент] Ошибка: {ex.Message}");
            }
        }
    }
}