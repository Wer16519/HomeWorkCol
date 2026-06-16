using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Task46_SimpleChat
{
    class Program
    {
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static readonly object _lock = new object();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Простой чат ===\n");
            Console.WriteLine("Выберите режим:");
            Console.WriteLine("1 - Сервер");
            Console.WriteLine("2 - Клиент");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                await RunServer();
            }
            else if (choice == "2")
            {
                await RunClient();
            }
            else
            {
                Console.WriteLine("Неверный выбор");
            }

            Console.ReadKey();
        }

        static async Task RunServer()
        {
            try
            {
                TcpListener server = new TcpListener(IPAddress.Any, 8888);
                server.Start();
                Console.WriteLine("Сервер запущен на порту 8888");
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    lock (_lock)
                    {
                        _clients.Add(client);
                    }

                    Console.WriteLine($"[Клиент подключен] Всего: {_clients.Count}");

                    ThreadPool.QueueUserWorkItem(async _ =>
                    {
                        await HandleClient(client);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сервера: {ex.Message}");
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];

                    while (client.Connected)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Получено: {message}");

                        await BroadcastMessage(message, client);
                    }
                }
            }
            catch
            {
                // Клиент отключился
            }
            finally
            {
                lock (_lock)
                {
                    _clients.Remove(client);
                }
                Console.WriteLine($"[Клиент отключился] Осталось: {_clients.Count}");
            }
        }

        static async Task BroadcastMessage(string message, TcpClient sender)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message + "\n");

            lock (_lock)
            {
                foreach (TcpClient client in _clients)
                {
                    if (client != sender && client.Connected)
                    {
                        try
                        {
                            client.GetStream().Write(data, 0, data.Length);
                        }
                        catch { }
                    }
                }
            }

            await Task.CompletedTask;
        }

        static async Task RunClient()
        {
            try
            {
                Console.Write("Введите IP сервера (localhost): ");
                string ip = Console.ReadLine();
                if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";

                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ip, 8888);
                    Console.WriteLine("Подключено к серверу!");
                    Console.WriteLine("Введите сообщение (exit для выхода)\n");

                    using (NetworkStream stream = client.GetStream())
                    {
                        Thread receiveThread = new Thread(() => ReceiveMessages(stream));
                        receiveThread.IsBackground = true;
                        receiveThread.Start();

                        while (true)
                        {
                            string message = Console.ReadLine();
                            if (message?.ToLower() == "exit") break;

                            if (!string.IsNullOrEmpty(message))
                            {
                                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                                await stream.WriteAsync(data, 0, data.Length);
                            }
                        }
                    }
                }

                Console.WriteLine("Отключено от сервера");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка клиента: {ex.Message}");
            }
        }

        static void ReceiveMessages(NetworkStream stream)
        {
            try
            {
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(message.TrimEnd('\n'));
                }
            }
            catch { }
        }
    }
}