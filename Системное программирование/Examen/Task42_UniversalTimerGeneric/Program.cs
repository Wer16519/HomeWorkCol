using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Task42_UniversalTimerGeneric
{
    public class TimerTask<T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Interval { get; set; }
        public int MaxCount { get; set; }
        public int CurrentCount { get; set; }
        public bool IsRunning { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime NextTrigger { get; set; }
        public Action<T> Callback { get; set; }
        public T Data { get; set; }
        public Func<T, bool> Condition { get; set; }
        public Timer Timer { get; set; }
    }

    public class UniversalTimer<T> : IDisposable
    {
        private List<TimerTask<T>> _tasks = new List<TimerTask<T>>();
        private int _taskCounter = 0;
        private readonly object _lock = new object();
        private bool _disposed;

        public int AddTimer(string name, int intervalMs, Action<T> callback, T data = default(T), int maxCount = -1, Func<T, bool> condition = null)
        {
            lock (_lock)
            {
                int id = ++_taskCounter;
                TimerTask<T> task = new TimerTask<T>
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
                Console.WriteLine($"[Добавлен таймер] ID:{id}, Имя:{name}, Интервал:{intervalMs}мс, Тип данных:{typeof(T).Name}");
                return id;
            }
        }

        public void StartTimer(int id)
        {
            lock (_lock)
            {
                TimerTask<T> task = _tasks.Find(t => t.Id == id);
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
                TimerTask<T> task = _tasks.Find(t => t.Id == id);
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
                foreach (TimerTask<T> task in _tasks)
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
                foreach (TimerTask<T> task in _tasks)
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
                TimerTask<T> task = _tasks.Find(t => t.Id == id);
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
                TimerTask<T> task = _tasks.Find(t => t.Id == id);
                if (task == null)
                {
                    Console.WriteLine($"[Ошибка] Таймер с ID {id} не найден");
                    return;
                }

                task.MaxCount = newMaxCount;
                Console.WriteLine($"[Таймер {id}] Максимум срабатываний изменен на {newMaxCount}");
            }
        }

        public void ChangeData(int id, T newData)
        {
            lock (_lock)
            {
                TimerTask<T> task = _tasks.Find(t => t.Id == id);
                if (task == null)
                {
                    Console.WriteLine($"[Ошибка] Таймер с ID {id} не найден");
                    return;
                }

                task.Data = newData;
                Console.WriteLine($"[Таймер {id}] Данные обновлены");
            }
        }

        public TimerTask<T> GetTimerInfo(int id)
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
                foreach (TimerTask<T> task in _tasks)
                {
                    Console.WriteLine($"ID:{task.Id} | {task.Name} | Состояние:{(task.IsRunning ? "Запущен" : "Остановлен")} | Срабатываний:{task.CurrentCount} | Интервал:{task.Interval}мс | Данные:{task.Data}");
                }
                Console.WriteLine("========================\n");
            }
        }

        private void TimerCallback(object state)
        {
            TimerTask<T> task = (TimerTask<T>)state;
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
                    foreach (TimerTask<T> task in _tasks)
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
            Console.WriteLine("=== Универсальный таймер (Generic) ===\n");

            using (UniversalTimer<int> timerInt = new UniversalTimer<int>())
            {
                int id1 = timerInt.AddTimer(
                    name: "Счётчик",
                    intervalMs: 1000,
                    callback: (data) =>
                    {
                        data++;
                        Console.WriteLine($"[Таймер int] Счётчик: {data}");
                    },
                    data: 0,
                    maxCount: 5
                );

                timerInt.StartTimer(id1);

                Thread.Sleep(6000);
            }

            Console.WriteLine("\n--- Таймер с типом string ---\n");

            using (UniversalTimer<string> timerString = new UniversalTimer<string>())
            {
                int id2 = timerString.AddTimer(
                    name: "Сообщение",
                    intervalMs: 1500,
                    callback: (data) =>
                    {
                        Console.WriteLine($"[Таймер string] {data} в {DateTime.Now:HH:mm:ss}");
                    },
                    data: "Привет из таймера!",
                    maxCount: 4
                );

                timerString.StartTimer(id2);

                Thread.Sleep(2000);

                timerString.ChangeData(id2, "Новое сообщение!");

                Thread.Sleep(3000);
            }

            Console.WriteLine("\n--- Таймер с типом List<int> ---\n");

            using (UniversalTimer<List<int>> timerList = new UniversalTimer<List<int>>())
            {
                int id3 = timerList.AddTimer(
                    name: "Список чисел",
                    intervalMs: 1000,
                    callback: (data) =>
                    {
                        data.Add(data.Count + 1);
                        Console.WriteLine($"[Таймер List] Элементов: {data.Count}, Данные: [{string.Join(", ", data)}]");
                    },
                    data: new List<int>(),
                    maxCount: 6
                );

                timerList.StartTimer(id3);

                Thread.Sleep(3000);

                timerList.ChangeData(id3, new List<int> { 100, 200, 300 });

                Thread.Sleep(3000);
            }

            Console.WriteLine("\n--- Таймер с пользовательским типом ---\n");

            using (UniversalTimer<Person> timerPerson = new UniversalTimer<Person>())
            {
                int id4 = timerPerson.AddTimer(
                    name: "Персона",
                    intervalMs: 2000,
                    callback: (data) =>
                    {
                        data.Age++;
                        Console.WriteLine($"[Таймер Person] {data.Name}, возраст: {data.Age}");
                    },
                    data: new Person { Name = "Иван", Age = 25 },
                    maxCount: 4,
                    condition: (data) =>
                    {
                        bool result = data.Age < 30;
                        Console.WriteLine($"[Условие] Возраст {data.Age} < 30 = {result}");
                        return result;
                    }
                );

                timerPerson.StartTimer(id4);

                Thread.Sleep(8000);
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Age} лет)";
        }
    }
}