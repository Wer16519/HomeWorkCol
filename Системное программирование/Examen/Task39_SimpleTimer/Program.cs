using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Task39_SimpleTimer
{
    public class SimpleTimer : IDisposable
    {
        private Timer _timer;
        private int _interval;
        private Action _callback;
        private bool _isRunning;
        private bool _disposed;
        private int _tickCount;
        private int _maxTicks;
        private readonly object _lock = new object();

        public SimpleTimer(Action callback, int intervalMs, int maxTicks = -1)
        {
            _callback = callback;
            _interval = intervalMs;
            _maxTicks = maxTicks;
            _isRunning = false;
            _tickCount = 0;
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
                _tickCount = 0;

                _timer = new Timer(TimerCallback, null, 0, _interval);
                Console.WriteLine($"[Таймер] Запущен (интервал: {_interval}мс)");
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    Console.WriteLine("[Таймер] Уже остановлен");
                    return;
                }

                _isRunning = false;
                _timer?.Change(Timeout.Infinite, Timeout.Infinite);
                Console.WriteLine("[Таймер] Остановлен");
            }
        }

        public void ChangeInterval(int newInterval)
        {
            lock (_lock)
            {
                _interval = newInterval;
                if (_isRunning)
                {
                    _timer?.Change(0, newInterval);
                    Console.WriteLine($"[Таймер] Интервал изменен на {newInterval}мс");
                }
            }
        }

        public bool IsRunning => _isRunning;
        public int TickCount => _tickCount;

        private void TimerCallback(object state)
        {
            lock (_lock)
            {
                if (!_isRunning || _disposed) return;

                _tickCount++;

                try
                {
                    _callback?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Таймер] Ошибка в callback: {ex.Message}");
                }

                if (_maxTicks > 0 && _tickCount >= _maxTicks)
                {
                    Console.WriteLine($"[Таймер] Достигнуто максимальное количество срабатываний ({_maxTicks})");
                    Stop();
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
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
            Console.WriteLine("=== Простой таймер с обратным вызовом ===\n");

            int counter = 0;

            Action callback = () =>
            {
                counter++;
                Console.WriteLine($"[Событие] Срабатывание #{counter} в {DateTime.Now:HH:mm:ss.fff}");
            };

            SimpleTimer timer = new SimpleTimer(callback, 1000, 5);

            Console.WriteLine("Таймер запущен. Срабатывания каждую секунду (максимум 5 раз)");
            Console.WriteLine("Нажмите Enter для остановки таймера...\n");

            timer.Start();

            Console.ReadKey();

            timer.Stop();

            Console.WriteLine($"\nВсего срабатываний: {timer.TickCount}");

            Console.WriteLine("\n--- Запуск второго таймера с другим интервалом ---");

            int counter2 = 0;
            SimpleTimer timer2 = new SimpleTimer(() =>
            {
                counter2++;
                Console.WriteLine($"[Таймер2] Срабатывание #{counter2} в {DateTime.Now:HH:mm:ss.fff}");
            }, 2000, 3);

            timer2.Start();

            Thread.Sleep(7000);

            timer2.Stop();

            timer.Dispose();
            timer2.Dispose();

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}