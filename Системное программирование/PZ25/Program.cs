using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PZ25
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Практическое занятие 25 - Работа с ожидающим таймером");
            Console.WriteLine("===================================================");
            Console.WriteLine("Выберите задание (1-12):");
            Console.WriteLine("1 - Простой таймер с callback");
            Console.WriteLine("2 - Несколько таймеров одновременно");
            Console.WriteLine("3 - Таймер с пользовательскими параметрами");
            Console.WriteLine("4 - Таймер с остановкой до срабатывания");
            Console.WriteLine("5 - Таймер с отменой (CancellationToken)");
            Console.WriteLine("6 - Многопоточный таймер");
            Console.WriteLine("7 - Поддержка часовых поясов");
            Console.WriteLine("8 - Таймер с будущим временем");
            Console.WriteLine("9 - Периодический таймер");
            Console.WriteLine("10 - Таймер с лимитом срабатываний");
            Console.WriteLine("11 - Условный таймер (по CPU)");
            Console.WriteLine("12 - Универсальный таймер");
            Console.Write("\nВаш выбор: ");

            string choice = Console.ReadLine();
            Console.Clear();

            switch (choice)
            {
                case "1": Task1_SimpleTimer(); break;
                case "2": Task2_MultipleTimers(); break;
                case "3": Task3_UserTimer(); break;
                case "4": Task4_StoppableTimer(); break;
                case "5": Task5_CancellableTimer().GetAwaiter().GetResult(); break;
                case "6": Task6_MultiThreadTimer(); break;
                case "7": Task7_TimeZoneTimer(); break;
                case "8": Task8_FutureTimeTimer(); break;
                case "9": Task9_PeriodicTimer(); break;
                case "10": Task10_LimitedTimer(); break;
                case "11": Task11_ConditionalTimer(); break;
                case "12": Task12_UniversalTimer(); break;
                default: Console.WriteLine("Неверный выбор!"); break;
            }
        }

        // ЗАДАНИЕ 1: Простой ожидающий таймер с callback
        static void Task1_SimpleTimer()
        {
            Console.WriteLine("Задание 1: Простой таймер на 3 секунды");
            Timer timer = new Timer(Callback1, null, 3000, Timeout.Infinite);
            Console.WriteLine("Таймер запущен. Нажмите Enter для выхода...");
            Console.ReadLine();

            void Callback1(object state)
            {
                Console.WriteLine($"[{DateTime.Now:T}] Таймер сработал!");
            }
        }

        // ЗАДАНИЕ 2: Несколько таймеров одновременно
        static void Task2_MultipleTimers()
        {
            Console.WriteLine("Задание 2: Запущено 3 параллельных таймера");
            new Timer(Callback2, "Таймер A (1 сек)", 1000, Timeout.Infinite);
            new Timer(Callback2, "Таймер B (2 сек)", 2000, Timeout.Infinite);
            new Timer(Callback2, "Таймер C (3 сек)", 3000, Timeout.Infinite);
            Console.WriteLine("Все таймеры запущены. Нажмите Enter для выхода...");
            Console.ReadLine();

            void Callback2(object state)
            {
                Console.WriteLine($"[{DateTime.Now:T}] {state} сработал в потоке {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        // ЗАДАНИЕ 3: Таймер с пользовательскими параметрами
        static void Task3_UserTimer()
        {
            Console.Write("Введите количество секунд до срабатывания: ");
            int seconds = int.Parse(Console.ReadLine());
            Console.Write("Введите сообщение для вывода: ");
            string message = Console.ReadLine();

            Timer timer = new Timer(Callback3, message, seconds * 1000, Timeout.Infinite);
            Console.WriteLine($"Таймер установлен на {seconds} секунд(ы). Нажмите Enter для выхода...");
            Console.ReadLine();

            void Callback3(object state)
            {
                Console.WriteLine($"[{DateTime.Now:T}] {state}");
            }
        }

        // ЗАДАНИЕ 4: Таймер с остановкой до срабатывания (ИСПРАВЛЕНО)
        static void Task4_StoppableTimer()
        {
            Timer timer = null; // Явно инициализируем null

            timer = new Timer(Callback4, null, 10000, Timeout.Infinite);
            Console.WriteLine("Таймер установлен на 10 секунд.");
            Console.WriteLine("Нажмите 'S' для остановки таймера до срабатывания.");

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S)
                {
                    timer.Dispose();
                    Console.WriteLine("Таймер был остановлен досрочно!");
                    break;
                }
                Thread.Sleep(100);
            }
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();

            void Callback4(object state)
            {
                Console.WriteLine($"[{DateTime.Now:T}] Таймер сработал!");
            }
        }

        // ЗАДАНИЕ 5: Отмена таймера через CancellationToken
        static async Task Task5_CancellableTimer()
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Console.WriteLine("Таймер установлен на 5 секунд.");
                Console.WriteLine("Нажмите 'C' для отмены таймера.");

                Task monitorTask = Task.Run(() =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)
                        {
                            cts.Cancel();
                            Console.WriteLine("Отмена запрошена!");
                            break;
                        }
                        Thread.Sleep(100);
                    }
                });

                try
                {
                    await Task.Delay(5000, cts.Token);
                    Console.WriteLine($"[{DateTime.Now:T}] Таймер сработал!");
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Таймер был отменён до срабатывания!");
                }

                await monitorTask;
                Console.WriteLine("Нажмите Enter для выхода...");
                Console.ReadLine();
            }
        }

        // ЗАДАНИЕ 6: Многопоточный таймер
        static void Task6_MultiThreadTimer()
        {
            Console.WriteLine("Задание 6: Запущено 5 таймеров в разных потоках");
            Console.WriteLine("Каждый таймер срабатывает каждые 2 секунды");

            for (int i = 1; i <= 5; i++)
            {
                int timerId = i;
                Timer timer = new Timer(Callback6, timerId, 1000, 2000);
            }

            Console.WriteLine("Нажмите Enter для остановки всех таймеров...");
            Console.ReadLine();

            void Callback6(object state)
            {
                int id = (int)state;
                Console.WriteLine($"[{DateTime.Now:T}] Таймер {id} сработал в потоке {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        // ЗАДАНИЕ 7: Поддержка часовых поясов
        static void Task7_TimeZoneTimer()
        {
            Console.WriteLine("Задание 7: Таймер с отображением времени в разных часовых поясах");
            Console.WriteLine("Обновление каждые 2 секунды. Нажмите Enter для выхода...");

            TimeZoneInfo mskZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            TimeZoneInfo tomskZone = TimeZoneInfo.FindSystemTimeZoneById("Tomsk Standard Time");

            Timer timer = new Timer(Callback7, null, 0, 2000);
            Console.ReadLine();
            timer.Dispose();

            void Callback7(object state)
            {
                DateTime now = DateTime.Now;
                DateTime mskTime = TimeZoneInfo.ConvertTime(now, mskZone);
                DateTime tomskTime = TimeZoneInfo.ConvertTime(now, tomskZone);

                Console.WriteLine($"Москва: {mskTime:T} | Томск: {tomskTime:T} | UTC: {DateTime.UtcNow:T}");
            }
        }

        // ЗАДАНИЕ 8: Таймер с будущим временем срабатывания
        static void Task8_FutureTimeTimer()
        {
            Console.Write("Введите будущее время (формат ЧЧ:ММ): ");
            string input = Console.ReadLine();
            DateTime targetTime = DateTime.Today + TimeSpan.Parse(input);

            if (targetTime <= DateTime.Now)
            {
                targetTime = targetTime.AddDays(1);
            }

            int delayMs = (int)(targetTime - DateTime.Now).TotalMilliseconds;

            Timer timer = new Timer(Callback8, null, delayMs, Timeout.Infinite);
            Console.WriteLine($"Таймер установлен на {targetTime:T} (через {delayMs / 1000} секунд)");
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();

            void Callback8(object state)
            {
                Console.WriteLine($"[{DateTime.Now:T}] ЗАПУСК! Таймер сработал в запланированное время!");
            }
        }

        // ЗАДАНИЕ 9: Периодический таймер
        static void Task9_PeriodicTimer()
        {
            Console.Write("Введите интервал в секундах: ");
            int interval = int.Parse(Console.ReadLine());

            int count = 0;
            Timer timer = new Timer(Callback9, null, 0, interval * 1000);
            Console.WriteLine($"Таймер будет срабатывать каждые {interval} секунд(ы)");
            Console.WriteLine("Нажмите Enter для остановки...");
            Console.ReadLine();
            timer.Dispose();

            void Callback9(object state)
            {
                count++;
                Console.WriteLine($"[{DateTime.Now:T}] Срабатывание #{count}");
            }
        }

        // ЗАДАНИЕ 10: Таймер с ограничением количества срабатываний (ИСПРАВЛЕНО)
        static void Task10_LimitedTimer()
        {
            Console.Write("Введите максимальное количество срабатываний: ");
            int maxFires = int.Parse(Console.ReadLine());
            Console.Write("Введите интервал в секундах: ");
            int interval = int.Parse(Console.ReadLine());

            int fireCount = 0;
            Timer timer = null;
            timer = new Timer(Callback10, null, 0, interval * 1000);
            Console.WriteLine($"Таймер сработает максимум {maxFires} раз(а) каждые {interval} секунд(ы)");
            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();

            void Callback10(object state)
            {
                fireCount++;
                Console.WriteLine($"[{DateTime.Now:T}] Срабатывание #{fireCount} из {maxFires}");

                if (fireCount >= maxFires)
                {
                    timer?.Dispose();
                    Console.WriteLine("Достигнуто максимальное количество срабатываний! Таймер остановлен.");
                }
            }
        }

        // ЗАДАНИЕ 11: Условный таймер (по загрузке CPU) - ПОЛНОСТЬЮ ПЕРЕПИСАН
        static void Task11_ConditionalTimer()
        {
            Console.WriteLine("Задание 11: Таймер сработает при загрузке CPU > 50%");
            Console.WriteLine("Для теста: запустите что-нибудь ресурсоёмкое (например, видеоигру или стресс-тест)");

            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // Первое значение всегда 0

            Timer timer = null;
            timer = new Timer((state) =>
            {
                float cpuUsage = cpuCounter.NextValue();
                Console.WriteLine($"Текущая загрузка CPU: {cpuUsage:F1}%");

                if (cpuUsage > 50)
                {
                    Console.WriteLine($"[{DateTime.Now:T}] УСЛОВИЕ ВЫПОЛНЕНО! Загрузка CPU {cpuUsage:F1}% > 50%");
                    Console.WriteLine("Таймер сработал по условию!");
                    timer?.Dispose();
                }
            }, null, 1000, 1000);

            Console.WriteLine("Мониторинг CPU запущен. Нажмите Enter для выхода...");
            Console.ReadLine();
            timer?.Dispose();
        }

        // ЗАДАНИЕ 12: Универсальный таймер с generics (ИСПРАВЛЕНО)
        static void Task12_UniversalTimer()
        {
            Console.WriteLine("Задание 12: Универсальный таймер с разными типами данных");

            // Таймер со строкой
            UniversalTimer<string> stringTimer = new UniversalTimer<string>(
                data => Console.WriteLine($"Строковый таймер: {data}"),
                "Привет, мир!",
                2000
            );

            // Таймер с числом
            UniversalTimer<int> intTimer = new UniversalTimer<int>(
                data => Console.WriteLine($"Числовой таймер: {data} * 2 = {data * 2}"),
                21,
                3000
            );

            // Таймер с double
            UniversalTimer<double> doubleTimer = new UniversalTimer<double>(
                data => Console.WriteLine($"Double таймер: PI = {data:F5}"),
                Math.PI,
                4000
            );

            // Таймер с DateTime
            UniversalTimer<DateTime> dateTimer = new UniversalTimer<DateTime>(
                data => Console.WriteLine($"Таймер времени: {data:T}"),
                DateTime.Now,
                5000
            );

            Console.WriteLine("Запущены 4 универсальных таймера с разными типами данных");
            Console.WriteLine("Нажмите Enter для остановки всех таймеров...");
            Console.ReadLine();

            stringTimer.Stop();
            intTimer.Stop();
            doubleTimer.Stop();
            dateTimer.Stop();

            Console.WriteLine("Все таймеры остановлены.");
            Console.ReadLine();
        }

        // Универсальный класс таймера для задания 12
        public class UniversalTimer<T>
        {
            private Timer timer;
            private bool isDisposed = false;

            public UniversalTimer(Action<T> callback, T data, int delayMilliseconds)
            {
                timer = new Timer(state =>
                {
                    callback((T)state);
                }, data, delayMilliseconds, Timeout.Infinite);
            }

            public void Stop()
            {
                if (!isDisposed)
                {
                    timer.Dispose();
                    isDisposed = true;
                }
            }
        }
    }
}