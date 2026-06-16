using System;
using System.Runtime.InteropServices;

namespace Task30_DynamicArray
{
    public class DynamicIntArray : IDisposable
    {
        private IntPtr _ptr;
        private int _capacity;
        private int _count;
        private bool _disposed;

        public DynamicIntArray()
        {
            _capacity = 4;
            _count = 0;
            _ptr = Marshal.AllocHGlobal(_capacity * sizeof(int));
            Console.WriteLine($"[Выделено] Память для {_capacity} элементов");
        }

        public void Add(int value)
        {
            if (_count >= _capacity)
            {
                int newCapacity = _capacity * 2;
                IntPtr newPtr = Marshal.AllocHGlobal(newCapacity * sizeof(int));

                for (int i = 0; i < _count; i++)
                {
                    int val = Marshal.ReadInt32(_ptr, i * sizeof(int));
                    Marshal.WriteInt32(newPtr, i * sizeof(int), val);
                }

                Marshal.FreeHGlobal(_ptr);
                _ptr = newPtr;
                _capacity = newCapacity;
                Console.WriteLine($"[Увеличение] Размер увеличен до {_capacity} элементов");
            }

            Marshal.WriteInt32(_ptr, _count * sizeof(int), value);
            _count++;
        }

        public int Get(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException($"Индекс {index} вне диапазона (0-{_count - 1})");
            return Marshal.ReadInt32(_ptr, index * sizeof(int));
        }

        public int Count => _count;
        public int Capacity => _capacity;

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_ptr);
                    _ptr = IntPtr.Zero;
                    Console.WriteLine("[Освобождено] Память освобождена");
                }
                _disposed = true;
            }
        }

        public void Print()
        {
            Console.Write("[Массив]: ");
            for (int i = 0; i < _count; i++)
            {
                Console.Write(Get(i) + " ");
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Динамическое выделение памяти через WinAPI ===\n");

            using (DynamicIntArray array = new DynamicIntArray())
            {
                Console.WriteLine($"Начальное состояние: Count={array.Count}, Capacity={array.Capacity}");

                for (int i = 1; i <= 10; i++)
                {
                    array.Add(i * 10);
                    Console.WriteLine($"Добавлен элемент: {i * 10}, Count={array.Count}, Capacity={array.Capacity}");
                }

                array.Print();

                Console.WriteLine($"\nЭлемент с индексом 3: {array.Get(3)}");
                Console.WriteLine($"Элемент с индексом 7: {array.Get(7)}");

                Console.WriteLine($"\nИтоговое состояние: Count={array.Count}, Capacity={array.Capacity}");
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}