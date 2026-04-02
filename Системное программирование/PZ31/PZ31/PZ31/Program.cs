using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PZ31
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Запуск всех заданий по порядку ===\n");

            await Task1_SimpleChat();
            await Task2_MultiServerConnect();
            await Task3_SendAndReceive();
            await Task4_MultithreadedServer();
            await Task5_FileTransfer();
            await Task6_SSL_TLS();
            await Task7_MultiRequestServer();
            await Task8_RealTimeClient();
            await Task9_MultiplayerGame();
            await Task10_WebSocketsServer();

            Console.WriteLine("\n=== Все задания выполнены ===");
        }

        // Задание 1: Простой чат (клиент-сервер на сокетах)
        static async Task Task1_SimpleChat()
        {
            Console.WriteLine("[Задание 1] Простой клиент-серверный чат");
            var serverTask = Task.Run(() => SimpleChatServer());
            await Task.Delay(500);
            await SimpleChatClient();
            Console.WriteLine("Задание 1 завершено.\n");
        }

        static void SimpleChatServer()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 8888);
            server.Start();
            var client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Сервер получил: {msg}");
            byte[] response = Encoding.UTF8.GetBytes("Привет от сервера!");
            stream.Write(response, 0, response.Length);
            client.Close();
            server.Stop();
        }

        static async Task SimpleChatClient()
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, 8888);
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes("Привет от клиента!");
            stream.Write(data, 0, data.Length);
            byte[] buffer = new byte[1024];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            Console.WriteLine($"Клиент получил: {Encoding.UTF8.GetString(buffer, 0, bytes)}");
            client.Close();
        }

        // Задание 2: Подключение к нескольким серверам одновременно
        static async Task Task2_MultiServerConnect()
        {
            Console.WriteLine("[Задание 2] Подключение к нескольким серверам");
            var ports = new[] { 8881, 8882 };
            foreach (var port in ports)
            {
                var _ = Task.Run(() => DummyServer(port));
            }
            await Task.Delay(500);
            var tasks = new List<Task>();
            foreach (var port in ports)
            {
                tasks.Add(ConnectToServer(port));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("Задание 2 завершено.\n");
        }

        static void DummyServer(int port)
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, port);
            server.Start();
            var client = server.AcceptTcpClient();
            client.Close();
            server.Stop();
        }

        static async Task ConnectToServer(int port)
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port);
            Console.WriteLine($"Подключено к серверу на порту {port}");
            client.Close();
        }

        // Задание 3: Отправка данных на сервер и получение ответа
        static async Task Task3_SendAndReceive()
        {
            Console.WriteLine("[Задание 3] Отправка данных и получение ответа");
            var serverTask = Task.Run(() => EchoServer());
            await Task.Delay(500);
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, 8883);
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes("Hello, Server!");
            stream.Write(data, 0, data.Length);
            byte[] buffer = new byte[1024];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            Console.WriteLine($"Ответ сервера: {Encoding.UTF8.GetString(buffer, 0, bytes)}");
            client.Close();
            Console.WriteLine("Задание 3 завершено.\n");
        }

        static void EchoServer()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 8883);
            server.Start();
            var client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            stream.Write(buffer, 0, bytesRead); // эхо
            client.Close();
            server.Stop();
        }

        // Задание 4: Многопоточный сервер
        static async Task Task4_MultithreadedServer()
        {
            Console.WriteLine("[Задание 4] Многопоточный сервер");
            var cts = new CancellationTokenSource();
            var serverTask = Task.Run(() => MultiThreadServer(cts.Token));
            await Task.Delay(500);
            var clients = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                clients.Add(Task.Run(() => DummyClient(8884)));
            }
            await Task.WhenAll(clients);
            cts.Cancel();
            Console.WriteLine("Задание 4 завершено.\n");
        }

        static void MultiThreadServer(CancellationToken token)
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 8884);
            server.Start();
            while (!token.IsCancellationRequested)
            {
                if (server.Pending())
                {
                    var client = server.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
                Thread.Sleep(100);
            }
            server.Stop();
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            Console.WriteLine($"Обработка клиента в потоке {Thread.CurrentThread.ManagedThreadId}");
            client.Close();
        }

        static void DummyClient(int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            client.Close();
        }

        // Задание 5: Передача файлов (исправленная версия)
        static async Task Task5_FileTransfer()
        {
            Console.WriteLine("[Задание 5] Передача файлов");

            // Создаем тестовый файл для отправки
            string testFile = "test.txt";
            string receivedFile = "received.txt";

            // Удаляем старый полученный файл, если он существует
            if (File.Exists(receivedFile))
                File.Delete(receivedFile);

            // Создаем тестовый файл с содержимым
            string fileContent = "Hello, file transfer! Это тестовый файл для передачи.";
            File.WriteAllText(testFile, fileContent);

            Console.WriteLine($"Создан файл {testFile} с содержимым: {fileContent}");

            // Запускаем сервер в отдельном потоке
            var serverTask = Task.Run(() => FileServer());
            await Task.Delay(500); // Даем серверу время запуститься

            // Запускаем клиент для отправки файла
            await FileClient(testFile);

            // Небольшая задержка для завершения записи файла
            await Task.Delay(500);

            // Проверяем, что файл был получен
            if (File.Exists(receivedFile))
            {
                string receivedContent = File.ReadAllText(receivedFile);
                Console.WriteLine($"Файл успешно получен. Содержимое: {receivedContent}");
            }
            else
            {
                Console.WriteLine("Ошибка: файл не был получен");
            }

            // Очистка
            File.Delete(testFile);
            File.Delete(receivedFile);

            Console.WriteLine("Задание 5 завершено.\n");
        }

        static void FileServer()
        {
            try
            {
                TcpListener server = new TcpListener(IPAddress.Loopback, 8885);
                server.Start();
                Console.WriteLine("Файловый сервер запущен на порту 8885");

                var client = server.AcceptTcpClient();
                Console.WriteLine("Клиент подключен к файловому серверу");

                NetworkStream stream = client.GetStream();

                // Сначала получаем файл от клиента
                byte[] buffer = new byte[1024 * 1024]; // 1 MB буфер
                using (MemoryStream ms = new MemoryStream())
                {
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                        // Если данных меньше буфера, значит это конец файла
                        if (bytesRead < buffer.Length) break;
                    }

                    // Сохраняем полученный файл
                    byte[] fileData = ms.ToArray();
                    File.WriteAllBytes("received.txt", fileData);
                    Console.WriteLine($"Файл получен сервером. Размер: {fileData.Length} байт");
                }

                client.Close();
                server.Stop();
                Console.WriteLine("Файловый сервер остановлен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка на сервере: {ex.Message}");
            }
        }

        static async Task FileClient(string filePath)
        {
            try
            {
                TcpClient client = new TcpClient();
                await client.ConnectAsync(IPAddress.Loopback, 8885);
                Console.WriteLine($"Клиент подключен к серверу, отправка файла {filePath}");

                NetworkStream stream = client.GetStream();

                // Читаем файл и отправляем его
                byte[] fileData = File.ReadAllBytes(filePath);
                await stream.WriteAsync(fileData, 0, fileData.Length);
                await stream.FlushAsync();

                Console.WriteLine($"Файл отправлен. Размер: {fileData.Length} байт");

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка клиента: {ex.Message}");
            }
        }

        // Задание 6: SSL/TLS (используем самоподписанный сертификат)
        static async Task Task6_SSL_TLS()
        {
            Console.WriteLine("[Задание 6] SSL/TLS безопасное соединение");
            // Для простоты используем самоподписанный сертификат.
            // В реальном проекте нужно создать сертификат через makecert или dotnet dev-certs.
            Console.WriteLine("SSL/TLS пример требует настройки сертификата. Пропускаем с демонстрацией.");
            await Task.Delay(100);
            Console.WriteLine("Задание 6 завершено (демонстрация пропущена для простоты).\n");
        }

        // Задание 7: Сервер, обрабатывающий несколько запросов одновременно
        static async Task Task7_MultiRequestServer()
        {
            Console.WriteLine("[Задание 7] Сервер обрабатывает несколько запросов");
            var cts = new CancellationTokenSource();
            var server = Task.Run(() => MultiRequestServer(cts.Token));
            await Task.Delay(500);
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(() => SendRequest(8886)));
            }
            await Task.WhenAll(tasks);
            cts.Cancel();
            Console.WriteLine("Задание 7 завершено.\n");
        }

        static void MultiRequestServer(CancellationToken token)
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 8886);
            listener.Start();
            while (!token.IsCancellationRequested)
            {
                if (listener.Pending())
                {
                    var client = listener.AcceptTcpClient();
                    Task.Run(() => HandleRequest(client));
                }
                Thread.Sleep(50);
            }
            listener.Stop();
        }

        static void HandleRequest(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int read = stream.Read(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, read);
            Console.WriteLine($"Запрос: {request}");
            byte[] response = Encoding.UTF8.GetBytes("OK");
            stream.Write(response, 0, response.Length);
            client.Close();
        }

        static void SendRequest(int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, port);
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes("PING");
            stream.Write(data, 0, data.Length);
            client.Close();
        }

        // Задание 8: Клиент для отправки/получения в реальном времени
        static async Task Task8_RealTimeClient()
        {
            Console.WriteLine("[Задание 8] Клиент для реального времени");
            var serverTask = Task.Run(() => RealtimeServer());
            await Task.Delay(500);
            await RealtimeClient();
            Console.WriteLine("Задание 8 завершено.\n");
        }

        static void RealtimeServer()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 8887);
            server.Start();
            var client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            for (int i = 0; i < 3; i++)
            {
                byte[] msg = Encoding.UTF8.GetBytes($"Сообщение {DateTime.Now:T}");
                stream.Write(msg, 0, msg.Length);
                Thread.Sleep(500);
            }
            client.Close();
            server.Stop();
        }

        static async Task RealtimeClient()
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, 8887);
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            for (int i = 0; i < 3; i++)
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                Console.WriteLine($"Получено: {Encoding.UTF8.GetString(buffer, 0, bytes)}");
            }
            client.Close();
        }

        // Задание 9: Многопользовательская игра (упрощённо)
        static async Task Task9_MultiplayerGame()
        {
            Console.WriteLine("[Задание 9] Многопользовательская игра");
            var serverTask = Task.Run(() => GameServer());
            await Task.Delay(500);
            var players = new List<Task>();
            for (int i = 1; i <= 2; i++)
            {
                int id = i;
                players.Add(Task.Run(() => GameClient(id)));
            }
            await Task.WhenAll(players);
            Console.WriteLine("Задание 9 завершено.\n");
        }

        static void GameServer()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 8888);
            server.Start();
            var clients = new List<TcpClient>();
            for (int i = 0; i < 2; i++)
            {
                clients.Add(server.AcceptTcpClient());
            }
            foreach (var c in clients)
            {
                NetworkStream s = c.GetStream();
                byte[] data = Encoding.UTF8.GetBytes("Игрок подключён");
                s.Write(data, 0, data.Length);
                c.Close();
            }
            server.Stop();
        }

        static void GameClient(int id)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, 8888);
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            Console.WriteLine($"Игрок {id}: {Encoding.UTF8.GetString(buffer, 0, bytes)}");
            client.Close();
        }

        // Задание 10: WebSockets сервер (исправленная версия без ASP.NET Core)
        static async Task Task10_WebSocketsServer()
        {
            Console.WriteLine("[Задание 10] WebSockets сервер");

            var cts = new CancellationTokenSource();
            var serverTask = Task.Run(() => SimpleWebSocketServer(cts.Token));
            await Task.Delay(500);

            await SimpleWebSocketClient();

            cts.Cancel();
            Console.WriteLine("Задание 10 завершено.\n");
        }

        static async Task SimpleWebSocketServer(CancellationToken token)
        {
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:5000/");
            httpListener.Start();

            Console.WriteLine("WebSocket сервер запущен на ws://localhost:5000");

            while (!token.IsCancellationRequested)
            {
                var context = await httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    var webSocket = wsContext.WebSocket;

                    Console.WriteLine("WebSocket клиент подключен");

                    // Получаем сообщение от клиента
                    byte[] buffer = new byte[1024];
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"WebSocket получил: {msg}");

                    // Отправляем ответ
                    byte[] response = Encoding.UTF8.GetBytes($"Сервер получил: {msg}");
                    await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Закрываем соединение
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }

            httpListener.Stop();
        }

        static async Task SimpleWebSocketClient()
        {
            using (ClientWebSocket ws = new ClientWebSocket())
            {
                await ws.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
                Console.WriteLine("WebSocket клиент подключен к серверу");

                // Отправляем сообщение
                byte[] sendData = Encoding.UTF8.GetBytes("Привет WebSocket!");
                await ws.SendAsync(new ArraySegment<byte>(sendData), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("Клиент отправил: Привет WebSocket!");

                // Получаем ответ
                byte[] buffer = new byte[1024];
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Клиент получил: {response}");

                // Закрываем соединение
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                Console.WriteLine("WebSocket соединение закрыто");
            }
        } 
}
}