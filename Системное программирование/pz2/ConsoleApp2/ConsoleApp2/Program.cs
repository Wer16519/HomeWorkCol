using System;
using System.Threading;
using System.Windows.Forms;

class Program
{
    private static int _busyThreads = 0;
    private static int _threadCount = 0;
    private static int _completedThreads = 0;
    private static readonly object _lockObject = new object();

    static void Main(string[] args)
    {
        Console.WriteLine("=== Простой пул потоков с MessageBox ===\n");

        // Запрос количества потоков
        Console.Write("Введите количество потоков: ");

        while (!int.TryParse(Console.ReadLine(), out _threadCount) || _threadCount <= 0)
        {
            Console.Write("Пожалуйста, введите положительное число: ");
        }

        // Запрос сообщения
        Console.Write("Введите сообщение: ");
        string message = Console.ReadLine();

        // Запуск мониторинга
        Thread monitorThread = new Thread(MonitorThreads);
        monitorThread.IsBackground = true;
        monitorThread.Start();

        // Отправка задач в пул потоков
        for (int i = 0; i < _threadCount; i++)
        {
            int threadNumber = i + 1; // Нумерация с 1
            ThreadPool.QueueUserWorkItem(ShowMessageBox, new { Message = message, Number = threadNumber });
        }

        // Ожидание завершения всех потоков
        while (true)
        {
            lock (_lockObject)
            {
                if (_completedThreads >= _threadCount)
                    break;
            }
            Thread.Sleep(100);
        }

        Console.WriteLine("Программа завершена.");
    }

    static void ShowMessageBox(object state)
    {
        lock (_lockObject) { _busyThreads++; }

        dynamic data = state;
        string text = data.Message;
        int number = data.Number;

        // MessageBox в STA потоке
        Thread staThread = new Thread(() =>
        {
            MessageBox.Show(text, $"Поток {number}");
        });
        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();
        staThread.Join();

        lock (_lockObject)
        {
            _busyThreads--;
            _completedThreads++;
        }
    }

    static void MonitorThreads()
    {
        while (_completedThreads < _threadCount)
        {
            lock (_lockObject)
            {
                Console.WriteLine($"[Монитор] Занято: {_busyThreads}/{_threadCount} потоков | Завершено: {_completedThreads}");
            }
            Thread.Sleep(1000);
        }
    }
}