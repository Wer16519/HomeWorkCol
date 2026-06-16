using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Task40_AdvancedTimer
{
    public class AdvancedTimer : IDisposable
    {
        private Timer _timer;
        private int _interval;
        private int _maxCount;
        private int _currentCount;
        private Action _callback;
        private Func<bool> _condition;
        private bool _isRunning;
        private bool _disposed;
        private DateTime _startTime;
        private DateTime _nextTrigger;
        private readonly object _lock = new object();

        public AdvancedTimer(Action callback, int intervalMs, int maxCount = -1, Func<bool> condition = null)
        {
            _callback = callback;
            _interval = intervalMs;
            _maxCount = maxCount;
            _condition = condition;
            _currentCount = 0;
            _isRunning = false;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    Console.WriteLine("[Таймер] Уже запущен");
                    return;
                }

                _isRunning = true;
                _currentCount = 0;
                _startTime = DateTime.Now;
                _nextTrigger = DateTime.Now.AddMilliseconds(_interval);

                _timer = new Timer(TimerCallback, null, 0, _interval);
                Console.WriteLine($"[Таймер] Запущен (интервал: {_interval}мс, макс: {(_maxCount > 0 ? _maxCount.ToString() : "бесконечно")})");
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning) return;

                _isRunning = false;
                _timer?.Change(Timeout.Infinite, Timeout.Infinite);
                Console.WriteLine("[Таймер] Остановлен");
            }
        }

        public void SetCondition(Func<bool> condition)
        {
            lock (_lock)
            {
                _condition = condition;
                Console.WriteLine("[Таймер] Условие срабатывания обновлено");
            }
        }

        public void SetMaxCount(int maxCount)
        {
            lock (_lock)
            {
                _maxCount = maxCount;
                Console.WriteLine($"[Таймер] Максимальное количество срабатываний: {maxCount}");
            }
        }

        public void SetInterval(int intervalMs)
        {
            lock (_lock)
            {
                _interval = intervalMs;
                if (_isRunning)
                {
                    _timer?.Change(0, intervalMs);
                    Console.WriteLine($"[Таймер] Интервал изменен на {intervalMs}мс");
                }
            }
        }

        public int CurrentCount => _currentCount;
        public bool IsRunning => _isRunning;
        public TimeSpan Elapsed => DateTime.Now - _startTime;
        public DateTime NextTrigger => _nextTrigger;

        private void TimerCallback(object state)
        {
            lock (_lock)
            {
                if (!_isRunning || _disposed) return;

                _nextTrigger = DateTime.Now.AddMilliseconds(_interval);

                if (_condition != null)
                {
                    try
                    {
                        if (!_condition())
                        {
                            Console.WriteLine($"[Таймер] Условие не выполнено, пропуск срабатывания");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Таймер] Ошибка при проверке условия: {ex.Message}");
                        return;
                    }
                }

                _currentCount++;

                try
                {
                    _callback?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Таймер] Ошибка в callback: {ex.Message}");
                }

                if (_maxCount > 0 && _currentCount >= _maxCount)
                {
                    Console.WriteLine($"[Таймер] Достигнуто максимальное количество ({_maxCount})");
                    Stop();
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _timer?.Dispose();
                _disposed = true;
                Console.WriteLine("[Таймер] Ресурсы освобождены");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Расширенный таймер ===\n");

            int counter = 0;
            Random random = new Random();

            Func<bool> condition = () =>
            {
                int value = random.Next(1, 10);
                bool result = value > 3;
                Console.WriteLine($"[Условие] Случайное число: {value}, условие: {(result ? "выполнено" : "не выполнено")}");
                return result;
            };

            AdvancedTimer timer = new AdvancedTimer(
                callback: () =>
                {
                    counter++;
                    Console.WriteLine($"[Событие] Срабатывание #{counter} в {DateTime.Now:HH:mm:ss.fff}");
                },
                intervalMs: 1000,
                maxCount: 10,
                condition: condition
            );

            Console.WriteLine("Таймер с условием срабатывания (случайное число > 3)");
            Console.WriteLine("Максимум 10 срабатываний");
            Console.WriteLine("Нажмите Enter для остановки...\n");

            timer.Start();

            Thread.Sleep(3000);

            Console.WriteLine($"\nКоличество срабатываний за 3 секунды: {timer.CurrentCount}");
            Console.WriteLine($"Прошло времени: {timer.Elapsed.Seconds}с");

            timer.SetMaxCount(5);

            Thread.Sleep(5000);

            timer.Stop();

            Console.WriteLine($"\nИтого срабатываний: {timer.CurrentCount}");

            Console.WriteLine("\n--- Таймер с условием загрузки процессора ---");

            int cpuCounter = 0;
            AdvancedTimer cpuTimer = new AdvancedTimer(
                callback: () =>
                {
                    cpuCounter++;
                    Console.WriteLine($"[CPU Таймер] Срабатывание #{cpuCounter}");
                },
                intervalMs: 2000,
                maxCount: 5,
                condition: () =>
                {
                    using (PerformanceCounter pc = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                    {
                        pc.NextValue();
                        Thread.Sleep(100);
                        float cpuUsage = pc.NextValue();
                        bool result = cpuUsage < 50;
                        Console.WriteLine($"[CPU] Загрузка: {cpuUsage:F1}%, условие: {(result ? "выполнено" : "не выполнено")}");
                        return result;
                    }
                }
            );

            cpuTimer.Start();

            Thread.Sleep(8000);

            cpuTimer.Stop();

            timer.Dispose();
            cpuTimer.Dispose();

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}