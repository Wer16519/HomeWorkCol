using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        // Задание 8: WinAPI импорты для виртуальной памяти
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(
            IntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFree(
            IntPtr lpAddress,
            uint dwSize,
            uint dwFreeType);

        const uint MEM_COMMIT = 0x1000;
        const uint MEM_RESERVE = 0x2000;
        const uint MEM_RELEASE = 0x8000;
        const uint PAGE_READWRITE = 0x04;

        static void Main(string[] args)
        {
            Console.WriteLine("Выберите задание для выполнения (1-10):");
            Console.Write("Ваш выбор: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Task1();
                    break;
                case "2":
                    Task2();
                    break;
                case "3":
                    Task3();
                    break;
                case "4":
                    Task4();
                    break;
                case "5":
                    Task5();
                    break;
                case "6":
                    Task6();
                    break;
                case "7":
                    Task7();
                    break;
                case "8":
                    Task8();
                    break;
                case "9":
                    Task9();
                    break;
                case "10":
                    Task10();
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Выполняются все задания по порядку:");
                    Task1();
                    Task2();
                    Task3();
                    Task4();
                    Task5();
                    Task6();
                    Task7();
                    Task8();
                    Task9();
                    Task10();
                    break;
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // Задание 1: Выделение памяти для массива целых чисел
        static void Task1()
        {
            Console.WriteLine("\nЗадание 1: Выделение памяти для массива из 1000 элементов");

            // Выделение памяти для массива (управляемая куча)
            int[] numbers = new int[1000];

            // Инициализация массива значениями
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = i + 1;
            }

            Console.WriteLine($"Массив из {numbers.Length} элементов выделен. Первые 10 элементов:");
            for (int i = 0; i < 10; i++)
            {
                Console.Write($"{numbers[i]} ");
            }
            Console.WriteLine();
        }

        // Задание 2: Функция для освобождения памяти
        static void Task2()
        {
            Console.WriteLine("\nЗадание 2: Функция для освобождения памяти");

            IntPtr memoryPtr = AllocateMemory(1024);
            Console.WriteLine("Память выделена.");

            FreeMemory(memoryPtr);
        }

        // Функция выделения памяти через Marshal
        static IntPtr AllocateMemory(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        // Функция освобождения памяти
        static void FreeMemory(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
                Console.WriteLine("Память освобождена.");
            }
        }

        // Задание 3: Класс с динамическим выделением памяти
        class DynamicArray<T> : IDisposable
        {
            private T[] _items;
            private bool _disposed = false;

            public DynamicArray(int size)
            {
                _items = new T[size];
                Console.WriteLine($"Выделено памяти для {size} элементов.");
            }

            public T this[int index]
            {
                get => _items[index];
                set => _items[index] = value;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _items = null;
                    _disposed = true;
                    Console.WriteLine("Динамическая память освобождена.");
                    GC.SuppressFinalize(this);
                }
            }

            ~DynamicArray()
            {
                Dispose();
            }
        }

        static void Task3()
        {
            Console.WriteLine("\nЗадание 3: Класс с динамическим выделением памяти");

            using (DynamicArray<int> arr = new DynamicArray<int>(1000))
            {
                arr[0] = 42;
                Console.WriteLine($"Значение элемента [0]: {arr[0]}");
            } // Здесь автоматически вызывается Dispose()
        }

        // Задание 4: Механизм сбора мусора
        class GarbageObject
        {
            public int Data { get; set; }

            ~GarbageObject()
            {
                Console.WriteLine($"Объект с данными {Data} удалён сборщиком мусора.");
            }
        }

        static void Task4()
        {
            Console.WriteLine("\nЗадание 4: Механизм сбора мусора");

            for (int i = 0; i < 10; i++)
            {
                GarbageObject obj = new GarbageObject { Data = i };
            }

            Console.WriteLine("Принудительный вызов сборщика мусора...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Сборка мусора завершена.");
        }

        // Задание 5: Программа для работы с динамической памятью
        static void Task5()
        {
            Console.WriteLine("\nЗадание 5: Программа для работы с динамической памятью");

            List<byte[]> memoryBlocks = new List<byte[]>();

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    memoryBlocks.Add(new byte[10 * 1024 * 1024]); // выделяем по 10 МБ
                    Console.WriteLine($"Выделен блок {i + 1} x 10 МБ");
                }
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Недостаточно памяти!");
            }

            // Освобождение памяти
            memoryBlocks.Clear();
            GC.Collect();
            Console.WriteLine("Память освобождена.");
        }

        // Задание 6: Управление распределением памяти между процессами
        static void AllocateMemoryForProcess(string processName, int sizeMB)
        {
            byte[] memory = new byte[sizeMB * 1024 * 1024];
            Console.WriteLine($"{processName} выделил {sizeMB} МБ");
            Thread.Sleep(2000);
            // память освободится автоматически при выходе из метода
        }

        static void Task6()
        {
            Console.WriteLine("\n--- Задание 6: Управление распределением памяти между несколькими процессами ---");

            Process currentProcess = Process.GetCurrentProcess();
            Console.WriteLine($"Текущий процесс PID: {currentProcess.Id}");

            Thread t1 = new Thread(() => AllocateMemoryForProcess("Поток 1", 50));
            Thread t2 = new Thread(() => AllocateMemoryForProcess("Поток 2", 50));

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Console.WriteLine("Выделение памяти потоками завершено.");
        }

        // Задание 7: Алгоритм кэширования данных
        class Cache<TKey, TValue>
        {
            private Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
            private int _maxSize;

            public Cache(int maxSize)
            {
                _maxSize = maxSize;
            }

            public void Add(TKey key, TValue value)
            {
                if (_cache.Count >= _maxSize)
                {
                    // Простое удаление первого элемента (имитация вытеснения)
                    var firstKey = new List<TKey>(_cache.Keys)[0];
                    _cache.Remove(firstKey);
                    Console.WriteLine($"Кэш переполнен, удалён элемент: {firstKey}");
                }
                _cache[key] = value;
            }

            public TValue Get(TKey key)
            {
                return _cache.TryGetValue(key, out var value) ? value : default;
            }
        }

        static void Task7()
        {
            Console.WriteLine("\nЗадание 7: Алгоритм кэширования данных");

            Cache<string, string> cache = new Cache<string, string>(3);

            cache.Add("1", "A");
            cache.Add("2", "B");
            cache.Add("3", "C");
            cache.Add("4", "D");

            Console.WriteLine($"Значение для ключа '1': {cache.Get("1") ?? "null"}");
            Console.WriteLine($"Значение для ключа '4': {cache.Get("4")}");
        }

        // Задание 8: Использование виртуальной памяти
        static void Task8()
        {
            Console.WriteLine("\nЗадание 8: Использование виртуальной памяти");

            IntPtr largeMemory = VirtualAlloc(IntPtr.Zero, 10 * 1024 * 1024, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (largeMemory != IntPtr.Zero)
            {
                Console.WriteLine("Выделено 10 МБ виртуальной памяти.");

                // Запись данных
                byte[] data = new byte[10];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)(i + 65);
                }
                Marshal.Copy(data, 0, largeMemory, data.Length);
                Console.WriteLine("Данные записаны в виртуальную память.");

                // Чтение данных
                byte[] readData = new byte[10];
                Marshal.Copy(largeMemory, readData, 0, readData.Length);
                Console.Write("Прочитанные данные: ");
                foreach (byte b in readData)
                {
                    Console.Write((char)b);
                }
                Console.WriteLine();

                // Освобождение
                VirtualFree(largeMemory, 0, MEM_RELEASE);
                Console.WriteLine("Виртуальная память освобождена.");
            }
            else
            {
                Console.WriteLine("Ошибка выделения виртуальной памяти.");
            }
        }

        // Задание 9: Библиотека для работы с памятью
        public static class MemoryManager
        {
            private static List<IntPtr> _allocatedPointers = new List<IntPtr>();
            private static readonly object _lockObject = new object();

            public static IntPtr Allocate(int size)
            {
                IntPtr ptr = Marshal.AllocHGlobal(size);
                lock (_lockObject)
                {
                    _allocatedPointers.Add(ptr);
                }
                Console.WriteLine($"Выделено {size} байт. Всего выделений: {_allocatedPointers.Count}");
                return ptr;
            }

            public static void Free(IntPtr ptr)
            {
                lock (_lockObject)
                {
                    if (_allocatedPointers.Contains(ptr))
                    {
                        Marshal.FreeHGlobal(ptr);
                        _allocatedPointers.Remove(ptr);
                        Console.WriteLine($"Память по адресу {ptr} освобождена.");
                    }
                }
            }

            public static void FreeAll()
            {
                lock (_lockObject)
                {
                    foreach (var ptr in _allocatedPointers)
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                    _allocatedPointers.Clear();
                }
                Console.WriteLine("Освобождена вся выделенная память.");
            }

            public static int GetAllocatedCount()
            {
                lock (_lockObject)
                {
                    return _allocatedPointers.Count;
                }
            }
        }

        static void Task9()
        {
            Console.WriteLine("\nЗадание 9: Библиотека для работы с памятью");

            IntPtr p1 = MemoryManager.Allocate(100);
            IntPtr p2 = MemoryManager.Allocate(200);
            IntPtr p3 = MemoryManager.Allocate(150);

            Console.WriteLine($"Всего выделенных блоков: {MemoryManager.GetAllocatedCount()}");

            MemoryManager.Free(p2);
            Console.WriteLine($"Блоков осталось: {MemoryManager.GetAllocatedCount()}");

            MemoryManager.FreeAll();
            Console.WriteLine($"Блоков после освобождения всех: {MemoryManager.GetAllocatedCount()}");
        }

        // Задание 10: Система управления памятью для многопоточной программы
        class ThreadSafeMemoryPool : IDisposable
        {
            private ConcurrentBag<byte[]> _pool = new ConcurrentBag<byte[]>();
            private int _blockSize;
            private bool _disposed = false;

            public ThreadSafeMemoryPool(int blockSize, int initialCount)
            {
                _blockSize = blockSize;
                for (int i = 0; i < initialCount; i++)
                {
                    _pool.Add(new byte[blockSize]);
                }
                Console.WriteLine($"Пул памяти создан: блоки по {blockSize} байт, начальное количество: {initialCount}");
            }

            public byte[] Get()
            {
                if (_pool.TryTake(out byte[] block))
                {
                    return block;
                }
                Console.WriteLine("Пул пуст, создаётся новый блок.");
                return new byte[_blockSize];
            }

            public void Return(byte[] block)
            {
                if (block?.Length == _blockSize)
                {
                    _pool.Add(block);
                }
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    while (_pool.TryTake(out _)) { }
                    _disposed = true;
                    Console.WriteLine("Пул памяти уничтожен.");
                    GC.SuppressFinalize(this);
                }
            }
        }

        static void Task10()
        {
            Console.WriteLine("\nЗадание 10: Система управления памятью для многопоточной программы");

            using (var pool = new ThreadSafeMemoryPool(1024, 5))
            {
                Parallel.For(0, 10, i =>
                {
                    byte[] memory = pool.Get();
                    memory[0] = (byte)i;
                    Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} получил блок {i}");
                    Thread.Sleep(100);
                    pool.Return(memory);
                    Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} вернул блок {i}");
                });
            }

            Console.WriteLine("Работа с пулом памяти завершена.");
        }
    }
}