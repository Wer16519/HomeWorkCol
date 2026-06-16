using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Task41_UniversalTimer
{
    public class TimerTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Interval { get; set; }
        public int MaxCount { get; set; }
        public int CurrentCount { get; set; }
        public bool IsRunning { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime NextTrigger { get; set; }
        public Action<object> Callback { get; set; }
        public object Data { get; set; }
        public Func<object, bool> Condition { get; set; }
        public Timer Timer { get; set; }
    }

    public class UniversalTimer : IDisposable
    {
        private List<TimerTask> _tasks = new List<TimerTask>();
        private int _taskCounter = 0;
        private readonly object _lock = new object();
        private bool _disposed;

        public int AddTimer(string name, int intervalMs, Action<object> callback, object data = null, int maxCount = -1, Func<object, bool> condition = null)
        {
            lock (_lock)
            {
                int id = ++_taskCounter;
                TimerTask task = new TimerTask
                {
                    Id = id,
                    Name = name,
                    Interval = intervalMs,
                    MaxCount = maxCount,
                    CurrentCount = 0,
                    IsRunning = false,
                    Callback = callback,
                    Data = data,
                    Condition = condition
                };

                _tasks.Add(task);
                Console.WriteLine($"[Добавлен таймер] ID:{id}, Имя:{name}, Интервал:{intervalMs}мс");
                return id;
            }
        }

        public void StartTimer(int id)
        {
            lock (_lock)
            {
                TimerTask task = _tasks.Find(t => t.Id == id);
                if (task == null)
                {
                    Console.WriteLine($"[Ошибка] Таймер с ID {id} не найден");
                    return;
                }

                if (task.IsRunning)
                {
                    Console.WriteLine($"[Таймер {id}] Уже запущен");
                    return;
                }

                task.IsRunning = true;
                task.CurrentCount = 0;
                task.StartTime = DateTime.Now;
                task.NextTrigger = DateTime.Now.AddMilliseconds(task.Interval);

                task.Timer = new Timer(TimerCallback, task, 0, task.Interval);
                Console.WriteLine($"[Таймер {id}] Запущен");
            }
        }

        public void StopTimer(int id)
        {
            lock (_lock)
            {
                TimerTask task = _tasks.Find(t => t.Id == id);
                if (task == null)
                {
                    Console.WriteLine($"[Ошибка] Таймер с ID {id} не найден");
                    return;
                }

                if (!task.IsRunning)
                {
                    Console.WriteLine($"[Таймер {id}] Уже остановлен");
                    return;
                }

                task.IsRunning = false;
                task.Timer?.Change(Timeout.Infinite, Timeout.Infinite);
                Console.WriteLine($"[Таймер {id}] Остановлен");
            }
        }

        public void StartAll()
        {
            lock (_lock)
            {
                foreach (TimerTask task in _tasks)
                {
                    if (!task.IsRunning)
                    {
                        StartTimer(task.Id);
                    }
                }
            }
        }

        public void StopAll()
        {
            lock (_lock)
            {
                foreach (TimerTask task in _tasks)
                {
                    if (task.IsRunning)
                    {
                        StopTimer(task.Id);
                    }
                }
            }
        }

        public void ChangeInterval(int id, int newInterval)
        {
            lock (_lock)
            {
                TimerTask task = _tasks.Find(t => t.Id == id);
                if (task == null)
                {
                    Console.WriteLine($"[Ошибка] Таймер с ID {id} не найден");
                    return;
                }

                task.Interval = newInterval;
                if (task.IsRunning)
                {
                    task.Timer?.Change(0, newInterval);
                    Console.WriteLine($"[Таймер {id}] Интервал изменен на {newInterval}мс");
                }
            }
        }

        public void ChangeMaxCount(int id, int newMaxCount)
        {
            lock (_lock)
            {
                TimerTask task = _tasks.Find(t => t.Id == id);
                if (task == null)
                {
                    Console.WriteLine($"[Ошибка] Таймер с ID {id} не найден");
                    return;
                }

                task.MaxCount = newMaxCount;
                Console.WriteLine($"[Таймер {id}] Максимум срабатываний изменен на {newMaxCount}");
            }
        }

        public void ChangeData(int id, object newData)
        {
            lock (_lock)
            {
                TimerTask task = _tasks.Find(t => t.Id == id);
                if (task == null)
                {
                    Console.WriteLine($"[Ошибка] Таймер с ID {id} не найден");
                    return;
                }

                task.Data = newData;
                Console.WriteLine($"[Таймер {id}] Данные обновлены");
            }
        }

        public TimerTask GetTimerInfo(int id)
        {
            lock (_lock)
            {
                return _tasks.Find(t => t.Id == id);
            }
        }

        public void PrintAllTimers()
        {
            lock (_lock)
            {
                Console.WriteLine("\n=== Список таймеров ===");
                foreach (TimerTask task in _tasks)
                {
                    Console.WriteLine($"ID:{task.Id} | {task.Name} | Состояние:{(task.IsRunning ? "Запущен" : "Остановлен")} | Срабатываний:{task.CurrentCount} | Интервал:{task.Interval}мс");
                }
                Console.WriteLine("========================\n");
            }
        }

        private void TimerCallback(object state)
        {
            TimerTask task = (TimerTask)state;
            lock (_lock)
            {
                if (!task.IsRunning || _disposed) return;

                task.NextTrigger = DateTime.Now.AddMilliseconds(task.Interval);

                if (task.Condition != null)
                {
                    try
                    {
                        if (!task.Condition(task.Data))
                        {
                            Console.WriteLine($"[Таймер {task.Id}] Условие не выполнено");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Таймер {task.Id}] Ошибка условия: {ex.Message}");
                        return;
                    }
                }

                task.CurrentCount++;

                try
                {
                    task.Callback?.Invoke(task.Data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Таймер {task.Id}] Ошибка callback: {ex.Message}");
                }

                if (task.MaxCount > 0 && task.CurrentCount >= task.MaxCount)
                {
                    Console.WriteLine($"[Таймер {task.Id}] Достигнут максимум ({task.MaxCount})");
                    StopTimer(task.Id);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                StopAll();
                lock (_lock)
                {
                    foreach (TimerTask task in _tasks)
                    {
                        task.Timer?.Dispose();
                    }
                    _tasks.Clear();
                }
                _disposed = true;
                Console.WriteLine("[Универсальный таймер] Ресурсы освобождены");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Универсальный таймер ===\n");

            using (UniversalTimer universalTimer = new UniversalTimer())
            {
                int id1 = universalTimer.AddTimer(
                    name: "Счётчик",
                    intervalMs: 1000,
                    callback: (data) =>
                    {
                        int count = (int)data;
                        count++;
                        Console.WriteLine($"[Таймер 1] Счётчик: {count}");
                    },
                    data: 0,
                    maxCount: 5
                );

                int id2 = universalTimer.AddTimer(
                    name: "Сообщение",
                    intervalMs: 2000,
                    callback: (data) =>
                    {
                        string message = (string)data;
                        Console.WriteLine($"[Таймер 2] {message} в {DateTime.Now:HH:mm:ss}");
                    },
                    data: "Привет из таймера",
                    maxCount: 4,
                    condition: (data) =>
                    {
                        Random random = new Random();
                        int value = random.Next(1, 10);
                        bool result = value > 3;
                        Console.WriteLine($"[Таймер 2] Условие: число {value} > 3 = {result}");
                        return result;
                    }
                );

                int id3 = universalTimer.AddTimer(
                    name: "Данные",
                    intervalMs: 1500,
                    callback: (data) =>
                    {
                        List<int> numbers = (List<int>)data;
                        numbers.Add(numbers.Count + 1);
                        Console.WriteLine($"[Таймер 3] Элементов в списке: {numbers.Count}");
                    },
                    data: new List<int>(),
                    maxCount: 6
                );

                universalTimer.PrintAllTimers();

                Console.WriteLine("Запуск всех таймеров...\n");
                universalTimer.StartAll();

                Thread.Sleep(4000);

                Console.WriteLine("\nИзменение данных у таймера 2...");
                universalTimer.ChangeData(id2, "Новое сообщение!");

                Thread.Sleep(3000);

                Console.WriteLine("\nИзменение интервала у таймера 1 на 500мс...");
                universalTimer.ChangeInterval(id1, 500);

                Thread.Sleep(3000);

                TimerTask info = universalTimer.GetTimerInfo(id1);
                Console.WriteLine($"\nИнформация о таймере 1: Срабатываний={info.CurrentCount}, Состояние={(info.IsRunning ? "Запущен" : "Остановлен")}");

                universalTimer.PrintAllTimers();

                Console.WriteLine("\nОстановка всех таймеров...");
                universalTimer.StopAll();

                Thread.Sleep(1000);

                Console.WriteLine("\nЗапуск только таймера 3...");
                universalTimer.StartTimer(id3);

                Thread.Sleep(3000);

                universalTimer.StopTimer(id3);

                Console.WriteLine("\n[Завершение] Программа завершена");
                Console.ReadKey();
            }
        }
    }
}