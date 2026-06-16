using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Task26_DeadlockSimulation
{
    public class Resource
    {
        public int Id { get; }
        public string Name { get; }
        private readonly object _lock = new object();

        public Resource(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void Lock()
        {
            Monitor.Enter(_lock);
        }

        public void Unlock()
        {
            Monitor.Exit(_lock);
        }
    }

    public static class Logger
    {
        private static readonly object _lock = new object();

        public static void Log(string message)
        {
            lock (_lock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }

    public static class DeadlockDetector
    {
        private static readonly HashSet<int> _activeThreads = new HashSet<int>();
        private static readonly object _lock = new object();

        public static void RegisterThread(int threadId)
        {
            lock (_lock)
            {
                _activeThreads.Add(threadId);
            }
        }

        public static void UnregisterThread(int threadId)
        {
            lock (_lock)
            {
                _activeThreads.Remove(threadId);
            }
        }

        public static async Task CheckForDeadlocksAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(5000, token);
                lock (_lock)
                {
                    if (_activeThreads.Count > 0)
                    {
                        Logger.Log($"Обнаружено потенциальных заблокированных потоков: {_activeThreads.Count}");
                    }
                }
            }
        }
    }

    class Program
    {
        private static Resource _resourceA;
        private static Resource _resourceB;
        private static CancellationTokenSource _cts;
        private static bool _usePrevention;

        static void Main(string[] args)
        {
            Console.WriteLine("Выберите режим:");
            Console.WriteLine("1 - Симуляция тупика");
            Console.WriteLine("2 - С предотвращением тупиков");
            Console.Write("Ваш выбор: ");
            var choice = Console.ReadLine();

            _usePrevention = choice == "2";
            _cts = new CancellationTokenSource();

            _resourceA = new Resource(1, "Ресурс A");
            _resourceB = new Resource(2, "Ресурс B");

            Logger.Log("Ресурсы созданы");

            var detectorTask = DeadlockDetector.CheckForDeadlocksAsync(_cts.Token);

            var thread1 = new Thread(() => ThreadWork(1, _resourceA, _resourceB, "Thread-1"));
            var thread2 = new Thread(() => ThreadWork(2, _resourceB, _resourceA, "Thread-2"));
            var thread3 = new Thread(() => ThreadWork(3, _resourceA, _resourceB, "Thread-3"));

            thread1.Start();
            thread2.Start();
            thread3.Start();

            Logger.Log("Все потоки запущены. Нажмите Enter для завершения...");
            Console.ReadLine();

            _cts.Cancel();
            thread1.Interrupt();
            thread2.Interrupt();
            thread3.Interrupt();

            Logger.Log("Программа завершена");
        }

        static void ThreadWork(int threadId, Resource first, Resource second, string name)
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    if (_usePrevention)
                    {
                        Resource firstLock, secondLock;
                        if (first.Id < second.Id)
                        {
                            firstLock = first;
                            secondLock = second;
                        }
                        else
                        {
                            firstLock = second;
                            secondLock = first;
                        }

                        firstLock.Lock();
                        Thread.Sleep(100);
                        secondLock.Lock();
                        Thread.Sleep(100);
                        secondLock.Unlock();
                        firstLock.Unlock();
                    }
                    else
                    {
                        first.Lock();
                        Thread.Sleep(300);
                        second.Lock();
                        Thread.Sleep(100);
                        second.Unlock();
                        first.Unlock();
                    }

                    Logger.Log($"{name}: итерация {i + 1} завершена");
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"{name}: ошибка - {ex.Message}");
            }
        }
    }
}