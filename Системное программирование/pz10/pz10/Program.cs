using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pz10
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Сетевое программирование сокетов - Лабораторная работа";
            
            Console.WriteLine("     ПРАКТИЧЕСКОЕ ЗАНЯТИЕ 10: СЕТЕВОЕ ПРОГРАММИРОВАНИЕ");
            
            Console.WriteLine();

            Task1_Basics();

            Task2_ClientServerApp();

            Task3_MultithreadingSockets();

            Task4_UDPvsTCP();

            Task5_ErrorHandling();

            Task6_Multicast();

            
            Console.WriteLine("Все задания выполнены! Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void Task1_Basics()
        {
            
            Console.WriteLine("ЗАДАНИЕ 1: ОСНОВЫ СОКЕТОВ");
            

            Console.WriteLine("\n1.1. Что такое сокет?");
            Console.WriteLine("   Сокет - это программный интерфейс для обмена данными между");
            Console.WriteLine("   процессами, который позволяет осуществлять связь между");
            Console.WriteLine("   компьютерами через сеть.");

            Console.WriteLine("\n1.2. IP-адрес и порт:");
            Console.WriteLine("   • IP-адрес - уникальный идентификатор устройства в сети");
            Console.WriteLine("   • Порт - числовой идентификатор процесса/приложения (0-65535)");
            Console.WriteLine("   • Вместе IP+PORT образуют уникальную точку подключения");

            Console.WriteLine("\n1.3. Примеры IP-адресов в C#:");
            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            IPAddress any = IPAddress.Any;
            IPAddress broadcast = IPAddress.Broadcast;

            Console.WriteLine($"   • Localhost (127.0.0.1): {localhost}");
            Console.WriteLine($"   • Any (0.0.0.0): {any}");
            Console.WriteLine($"   • Broadcast (255.255.255.255): {broadcast}");

            Console.WriteLine("\n1.4. Типы сокетов:");
            Console.WriteLine("   • Stream (TCP) - надежная передача с установкой соединения");
            Console.WriteLine("   • Dgram (UDP) - ненадежная передача без установки соединения");

            Console.WriteLine("\n1.5. Создание сокета в C#:");
            Console.WriteLine("   Socket tcpSocket = new Socket(AddressFamily.InterNetwork,");
            Console.WriteLine("                         SocketType.Stream, ProtocolType.Tcp);");
            Console.WriteLine("   Socket udpSocket = new Socket(AddressFamily.InterNetwork,");
            Console.WriteLine("                         SocketType.Dgram, ProtocolType.Udp);");

            Console.WriteLine("\n✓ Задание 1 завершено");
            
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }

        static void Task2_ClientServerApp()
        {
            
            Console.WriteLine("ЗАДАНИЕ 2: КЛИЕНТ-СЕРВЕРНОЕ ПРИЛОЖЕНИЕ (TCP)");
            

            int port = 8888;
            bool serverStarted = false;
            Task serverTask = null;

            try
            {
                Console.WriteLine("\n[СЕРВЕР] Запуск TCP сервера на порту " + port + "...");
                serverTask = Task.Run(() => RunTcpServer(port));
                Thread.Sleep(500);      
                serverStarted = true;

                Console.WriteLine("\n[КЛИЕНТ] Запуск TCP клиента...");
                Thread.Sleep(500);
                RunTcpClient(port);

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\n✓ Задание 2 завершено");
            
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }

        static void RunTcpServer(int port)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine("[СЕРВЕР] Ожидание подключения клиента...");

                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("[СЕРВЕР] Клиент подключен!");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[СЕРВЕР] Получено от клиента: {receivedData}");

                string response = $"Сервер получил: {receivedData}";
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);
                Console.WriteLine($"[СЕРВЕР] Отправлен ответ: {response}");

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[СЕРВЕР] Ошибка: {ex.Message}");
            }
            finally
            {
                server?.Stop();
                Console.WriteLine("[СЕРВЕР] Остановлен");
            }
        }

        static void RunTcpClient(int port)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback, port);
                    Console.WriteLine("[КЛИЕНТ] Подключен к серверу");

                    NetworkStream stream = client.GetStream();
                    string message = "Привет, сервер! Это клиентское сообщение.";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"[КЛИЕНТ] Отправлено: {message}");

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[КЛИЕНТ] Получен ответ: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КЛИЕНТ] Ошибка: {ex.Message}");
            }
        }

        static void Task3_MultithreadingSockets()
        {
            
            Console.WriteLine("ЗАДАНИЕ 3: МНОГОПОТОЧНОСТЬ И СОКЕТЫ");
            

            int port = 8889;
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.WriteLine("\n[МНОГОПОТОЧНЫЙ СЕРВЕР] Запуск с поддержкой нескольких клиентов...");

            Task serverTask = Task.Run(() => RunMultithreadedServer(port, cts.Token));

            Thread.Sleep(500);

            Console.WriteLine("\n[КЛИЕНТЫ] Запуск 3 клиентов одновременно...");
            List<Task> clientTasks = new List<Task>();

            for (int i = 1; i <= 3; i++)
            {
                int clientId = i;
                clientTasks.Add(Task.Run(() => RunMultithreadedClient(port, clientId)));
                Thread.Sleep(100);
            }

            Task.WaitAll(clientTasks.ToArray(), 5000);
            Console.WriteLine("\n[СЕРВЕР] Все клиенты обработаны, завершение работы...");
            cts.Cancel();

            Thread.Sleep(1000);

            Console.WriteLine("\n✓ Задание 3 завершено");
            
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }

        static void RunMultithreadedServer(int port, CancellationToken token)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine($"[МНОГОПОТОЧНЫЙ СЕРВЕР] Слушаем порт {port} (Thread: {Thread.CurrentThread.ManagedThreadId})");

                int clientCounter = 0;

                while (!token.IsCancellationRequested)
                {
                    if (server.Pending())
                    {
                        TcpClient client = server.AcceptTcpClient();
                        clientCounter++;
                        int currentClient = clientCounter;

                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            HandleClient(client, currentClient);
                        });
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                    Console.WriteLine($"[МНОГОПОТОЧНЫЙ СЕРВЕР] Ошибка: {ex.Message}");
            }
            finally
            {
                server?.Stop();
                Console.WriteLine("[МНОГОПОТОЧНЫЙ СЕРВЕР] Остановлен");
            }
        }

        static void HandleClient(TcpClient client, int clientId)
        {
            try
            {
                Console.WriteLine($"[СЕРВЕР] Обработка клиента #{clientId} в потоке {Thread.CurrentThread.ManagedThreadId}");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[СЕРВЕР] Клиент #{clientId} прислал: {receivedData}");

                Thread.Sleep(500);   

                string response = $"Клиент #{clientId}: сообщение получено. Ваш запрос: {receivedData}";
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);

                Console.WriteLine($"[СЕРВЕР] Клиенту #{clientId} отправлен ответ");
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[СЕРВЕР] Ошибка при обработке клиента #{clientId}: {ex.Message}");
            }
        }

        static void RunMultithreadedClient(int port, int clientId)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback, port);
                    Console.WriteLine($"[КЛИЕНТ #{clientId}] Подключен (поток: {Thread.CurrentThread.ManagedThreadId})");

                    NetworkStream stream = client.GetStream();
                    string message = $"Привет от клиента #{clientId}! Время: {DateTime.Now:HH:mm:ss}";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"[КЛИЕНТ #{clientId}] Отправлено: {message}");

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[КЛИЕНТ #{clientId}] Получен ответ: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КЛИЕНТ #{clientId}] Ошибка: {ex.Message}");
            }
        }

        static void Task4_UDPvsTCP()
        {
            
            Console.WriteLine("ЗАДАНИЕ 4: UDP И TCP - СРАВНЕНИЕ ПРОТОКОЛОВ");
            

            Console.WriteLine("\n4.1. TCP (Transmission Control Protocol):");
            Console.WriteLine("   • С установкой соединения (3-way handshake)");
            Console.WriteLine("   • Гарантированная доставка данных");
            Console.WriteLine("   • Контроль целостности и порядка пакетов");
            Console.WriteLine("   • Управление потоком и перегрузками");
            Console.WriteLine("   • Примеры: HTTP, HTTPS, FTP, Email");

            Console.WriteLine("\n4.2. UDP (User Datagram Protocol):");
            Console.WriteLine("   • Без установки соединения");
            Console.WriteLine("   • Нет гарантии доставки");
            Console.WriteLine("   • Пакеты могут приходить в любом порядке");
            Console.WriteLine("   • Быстрее и легче TCP");
            Console.WriteLine("   • Примеры: DNS, VoIP, видеозвонки, стриминг");

            Console.WriteLine("\n4.3. Демонстрация UDP (быстрая передача без подтверждения):");

            int udpPort = 8890;
            Task.Run(() => RunUdpServer(udpPort));
            Thread.Sleep(500);
            RunUdpClient(udpPort);
            Thread.Sleep(1000);

            Console.WriteLine("\n4.4. Демонстрация TCP (надежная передача с подтверждением):");
            int tcpPort = 8891;
            Task.Run(() => RunTcpDemoServer(tcpPort));
            Thread.Sleep(500);
            RunTcpDemoClient(tcpPort);
            Thread.Sleep(1000);

            Console.WriteLine("\n✓ Задание 4 завершено");
            
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }

        static void RunUdpServer(int port)
        {
            try
            {
                using (UdpClient udpServer = new UdpClient(port))
                {
                    Console.WriteLine($"[UDP СЕРВЕР] Ожидание данных на порту {port}...");
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpServer.Receive(ref remoteEP);
                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine($"[UDP СЕРВЕР] Получено от {remoteEP.Address}:{remoteEP.Port}: {message}");

                    string response = "UDP: Сообщение получено (без гарантии доставки)";
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    udpServer.Send(responseData, responseData.Length, remoteEP);
                    Console.WriteLine($"[UDP СЕРВЕР] Отправлен ответ (может быть потерян)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UDP СЕРВЕР] Ошибка: {ex.Message}");
            }
        }

        static void RunUdpClient(int port)
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    string message = "UDP тестовое сообщение (быстрая передача)";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    udpClient.Send(data, data.Length, "127.0.0.1", port);
                    Console.WriteLine($"[UDP КЛИЕНТ] Отправлено: {message}");

                    udpClient.Client.ReceiveTimeout = 2000;
                    try
                    {
                        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        byte[] response = udpClient.Receive(ref remoteEP);
                        string responseMessage = Encoding.UTF8.GetString(response);
                        Console.WriteLine($"[UDP КЛИЕНТ] Получен ответ: {responseMessage}");
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("[UDP КЛИЕНТ] Таймаут - ответ не получен (UDP не гарантирует доставку)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UDP КЛИЕНТ] Ошибка: {ex.Message}");
            }
        }

        static void RunTcpDemoServer(int port)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine($"[TCP ДЕМО СЕРВЕР] Ожидание подключения на порту {port}...");

                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("[TCP ДЕМО СЕРВЕР] Клиент подключен");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[TCP ДЕМО СЕРВЕР] Получено: {message}");

                string response = "TCP: Сообщение получено и подтверждено (гарантированная доставка)";
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);
                Console.WriteLine("[TCP ДЕМО СЕРВЕР] Отправлено подтверждение");

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCP ДЕМО СЕРВЕР] Ошибка: {ex.Message}");
            }
            finally
            {
                server?.Stop();
            }
        }

        static void RunTcpDemoClient(int port)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback, port);
                    Console.WriteLine("[TCP ДЕМО КЛИЕНТ] Подключен к серверу");

                    NetworkStream stream = client.GetStream();
                    string message = "TCP тестовое сообщение (надежная передача)";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"[TCP ДЕМО КЛИЕНТ] Отправлено: {message}");

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[TCP ДЕМО КЛИЕНТ] Получен ответ: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCP ДЕМО КЛИЕНТ] Ошибка: {ex.Message}");
            }
        }

        static void Task5_ErrorHandling()
        {
            
            Console.WriteLine("ЗАДАНИЕ 5: ОБРАБОТКА ОШИБОК И ИСКЛЮЧЕНИЙ");
            

            Console.WriteLine("\n5.1. Основные типы исключений при работе с сокетами:");
            Console.WriteLine("   • SocketException - ошибки сокетов (недоступный порт, таймаут и т.д.)");
            Console.WriteLine("   • ObjectDisposedException - попытка использовать закрытый сокет");
            Console.WriteLine("   • ArgumentNullException - передан пустой аргумент");
            Console.WriteLine("   • InvalidOperationException - неверное состояние сокета");

            Console.WriteLine("\n5.2. Демонстрация обработки ошибок:");

            Console.WriteLine("\n--- Попытка подключения к несуществующему серверу ---");
            TestConnectionError();

            Console.WriteLine("\n--- Попытка подключения с неверным портом ---");
            TestInvalidPortError();

            Console.WriteLine("\n--- Использование таймаутов ---");
            TestTimeoutHandling();

            Console.WriteLine("\n5.3. Лучшие практики обработки ошибок:");
            Console.WriteLine("   • Всегда использовать try-catch-finally блоки");
            Console.WriteLine("   • Закрывать сокеты в блоке finally");
            Console.WriteLine("   • Устанавливать таймауты для операций");
            Console.WriteLine("   • Логировать ошибки для отладки");
            Console.WriteLine("   • Использовать using для IDisposable объектов");

            Console.WriteLine("\n✓ Задание 5 завершено");
            
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }

        static void TestConnectionError()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Console.WriteLine("[ТЕСТ] Попытка подключения к 192.168.99.99:9999...");
                    client.Connect("192.168.99.99", 9999);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[ОШИБКА] SocketException: {ex.Message}");
                Console.WriteLine($"   Код ошибки: {ex.ErrorCode}");
                Console.WriteLine($"   Тип ошибки: {ex.SocketErrorCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.GetType().Name}: {ex.Message}");
            }
        }

        static void TestInvalidPortError()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Console.WriteLine("[ТЕСТ] Попытка подключения с неверным портом (70000)...");
                    client.Connect("127.0.0.1", 70000);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[ОШИБКА] ArgumentOutOfRangeException: {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[ОШИБКА] SocketException: {ex.Message} (порт недоступен)");
            }
        }

        static void TestTimeoutHandling()
        {
            int port = 8892;
            Task.Run(() => RunSlowServer(port));
            Thread.Sleep(500);

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.ReceiveTimeout = 2000;
                    Console.WriteLine($"[ТЕСТ] Подключение к порту {port} с таймаутом 2 сек...");
                    client.Connect("127.0.0.1", port);

                    NetworkStream stream = client.GetStream();
                    string message = "Тестовое сообщение";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("[ТЕСТ] Сообщение отправлено, ожидание ответа...");

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[ТЕСТ] Получен ответ: {response}");
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                Console.WriteLine($"[ОШИБКА] Таймаут операции: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
            }
        }

        static void RunSlowServer(int port)
        {
            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine($"[МЕДЛЕННЫЙ СЕРВЕР] Запущен на порту {port}");

                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("[МЕДЛЕННЫЙ СЕРВЕР] Клиент подключен");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                stream.Read(buffer, 0, buffer.Length);

                Console.WriteLine("[МЕДЛЕННЫЙ СЕРВЕР] Обработка запроса (задержка 5 сек)...");
                Thread.Sleep(5000);

                string response = "Ответ после задержки";
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[МЕДЛЕННЫЙ СЕРВЕР] Ошибка: {ex.Message}");
            }
            finally
            {
                server?.Stop();
            }
        }

        static void Task6_Multicast()
        {
            
            Console.WriteLine("ЗАДАНИЕ 6: МНОГОАДРЕСНАЯ РАССЫЛКА (MULTICAST)");
            

            Console.WriteLine("\n6.1. Что такое Multicast?");
            Console.WriteLine("   • Отправка данных группе получателей одновременно");
            Console.WriteLine("   • Использует специальные IP-адреса (224.0.0.0 - 239.255.255.255)");
            Console.WriteLine("   • Эффективнее, чем отправка каждому клиенту отдельно");
            Console.WriteLine("   • Примеры: видеоконференции, потоковое вещание, игры");

            Console.WriteLine("\n6.2. Демонстрация Multicast рассылки:");

            string multicastGroup = "224.5.6.7";
            int multicastPort = 8893;

            List<Thread> listeners = new List<Thread>();
            for (int i = 1; i <= 3; i++)
            {
                int listenerId = i;
                Thread listener = new Thread(() => RunMulticastListener(multicastGroup, multicastPort, listenerId));
                listener.Start();
                listeners.Add(listener);
                Thread.Sleep(200);
            }

            Thread.Sleep(1000);

            RunMulticastSender(multicastGroup, multicastPort);

            Thread.Sleep(2000);

            Console.WriteLine("\n6.3. Особенности Multicast:");
            Console.WriteLine("   • Требуется поддержка маршрутизаторами (IGMP)");
            Console.WriteLine("   • Получатели должны подписаться на группу");
            Console.WriteLine("   • TTL (Time To Live) ограничивает распространение");
            Console.WriteLine("   • Не гарантирует доставку (использует UDP)");

            Console.WriteLine("\n✓ Задание 6 завершено");
            
        }

        static void RunMulticastListener(string groupIp, int port, int listenerId)
        {
            try
            {
                IPAddress multicastAddress = IPAddress.Parse(groupIp);
                using (UdpClient listener = new UdpClient())
                {
                    listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    listener.Client.Bind(new IPEndPoint(IPAddress.Any, port));
                    listener.JoinMulticastGroup(multicastAddress);

                    Console.WriteLine($"[ПОЛУЧАТЕЛЬ #{listenerId}] Подписан на группу {groupIp}:{port}");

                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    listener.Client.ReceiveTimeout = 3000;

                    for (int i = 0; i < 2; i++)    
                    {
                        try
                        {
                            byte[] data = listener.Receive(ref remoteEP);
                            string message = Encoding.UTF8.GetString(data);
                            Console.WriteLine($"[ПОЛУЧАТЕЛЬ #{listenerId}] Получено: {message} (от {remoteEP.Address})");
                        }
                        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                        {
                            Console.WriteLine($"[ПОЛУЧАТЕЛЬ #{listenerId}] Таймаут ожидания");
                            break;
                        }
                    }

                    listener.DropMulticastGroup(multicastAddress);
                    Console.WriteLine($"[ПОЛУЧАТЕЛЬ #{listenerId}] Отписан от группы");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ПОЛУЧАТЕЛЬ #{listenerId}] Ошибка: {ex.Message}");
            }
        }

        static void RunMulticastSender(string groupIp, int port)
        {
            try
            {
                IPAddress multicastAddress = IPAddress.Parse(groupIp);
                using (UdpClient sender = new UdpClient())
                {
                    sender.JoinMulticastGroup(multicastAddress);
                    sender.Ttl = 2;   

                    Console.WriteLine($"\n[ОТПРАВИТЕЛЬ] Начало Multicast рассылки на {groupIp}:{port}");

                    for (int i = 1; i <= 3; i++)
                    {
                        string message = $"Multicast сообщение #{i}: Привет всем получателям! Время: {DateTime.Now:HH:mm:ss}";
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        sender.Send(data, data.Length, groupIp, port);
                        Console.WriteLine($"[ОТПРАВИТЕЛЬ] Отправлено сообщение #{i}");
                        Thread.Sleep(500);
                    }

                    Console.WriteLine("[ОТПРАВИТЕЛЬ] Рассылка завершена");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОТПРАВИТЕЛЬ] Ошибка: {ex.Message}");
            }
        }
    }
}