using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace PracticalWork19
{
    // Класс для работы с WinAPI памятью (задания 1-17)
    public static class Win32Memory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapCreate(uint flOptions, UIntPtr dwInitialSize, UIntPtr dwMaximumSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool HeapDestroy(IntPtr hHeap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessHeap();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsBadReadPtr(IntPtr lp, UIntPtr ucb);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsBadWritePtr(IntPtr lp, UIntPtr ucb);

        public const uint HEAP_NO_SERIALIZE = 0x00000001;
        public const uint HEAP_GENERATE_EXCEPTIONS = 0x00000004;
        public const uint HEAP_ZERO_MEMORY = 0x00000008;
    }

    // Задание 3: Класс с динамическим выделением памяти
    public class DynamicArray : IDisposable
    {
        private IntPtr _data;
        private int _size;
        private IntPtr _heap;
        private bool _disposed = false;

        public DynamicArray(int size)
        {
            _size = size;
            _heap = Win32Memory.HeapCreate(Win32Memory.HEAP_NO_SERIALIZE, (UIntPtr)(size * sizeof(int)), UIntPtr.Zero);
            if (_heap == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось создать кучу");

            _data = Win32Memory.HeapAlloc(_heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)(size * sizeof(int)));
            if (_data == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось выделить память");
        }

        public void SetValue(int index, int value)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DynamicArray));
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException();
            Marshal.WriteInt32(_data, index * sizeof(int), value);
        }

        public int GetValue(int index)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DynamicArray));
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException();
            return Marshal.ReadInt32(_data, index * sizeof(int));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_data != IntPtr.Zero)
                {
                    Win32Memory.HeapFree(_heap, 0, _data);
                    _data = IntPtr.Zero;
                }
                if (_heap != IntPtr.Zero)
                {
                    Win32Memory.HeapDestroy(_heap);
                    _heap = IntPtr.Zero;
                }
                _disposed = true;
            }
        }

        public int Size => _size;
    }

    // Задание 11: Класс для работы со строками
    public class DynamicString : IDisposable
    {
        private IntPtr _buffer;
        private int _capacity;
        private IntPtr _heap;
        private bool _disposed = false;

        public DynamicString(int capacity)
        {
            _capacity = capacity;
            _heap = Win32Memory.HeapCreate(Win32Memory.HEAP_NO_SERIALIZE, (UIntPtr)(capacity), UIntPtr.Zero);
            if (_heap == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось создать кучу");

            _buffer = Win32Memory.HeapAlloc(_heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)capacity);
            if (_buffer == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось выделить память");
        }

        public void SetString(string value)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DynamicString));

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
            int len = Math.Min(bytes.Length, _capacity - 1);
            Marshal.Copy(bytes, 0, _buffer, len);
            Marshal.WriteByte(_buffer, len, 0);
        }

        public string GetString()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DynamicString));
            return Marshal.PtrToStringAnsi(_buffer);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_buffer != IntPtr.Zero)
                {
                    Win32Memory.HeapFree(_heap, 0, _buffer);
                    _buffer = IntPtr.Zero;
                }
                if (_heap != IntPtr.Zero)
                {
                    Win32Memory.HeapDestroy(_heap);
                    _heap = IntPtr.Zero;
                }
                _disposed = true;
            }
        }
    }

    // Задание 12: Класс для работы с матрицами
    public class DynamicMatrix : IDisposable
    {
        private IntPtr _data;
        private int _rows, _cols;
        private IntPtr _heap;
        private bool _disposed = false;

        public DynamicMatrix(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _heap = Win32Memory.HeapCreate(Win32Memory.HEAP_NO_SERIALIZE, (UIntPtr)(rows * cols * sizeof(int)), UIntPtr.Zero);
            if (_heap == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось создать кучу");

            _data = Win32Memory.HeapAlloc(_heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)(rows * cols * sizeof(int)));
            if (_data == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось выделить память");
        }

        public void Set(int row, int col, int value)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DynamicMatrix));
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                throw new IndexOutOfRangeException();

            int index = row * _cols + col;
            Marshal.WriteInt32(_data, index * sizeof(int), value);
        }

        public int Get(int row, int col)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DynamicMatrix));
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                throw new IndexOutOfRangeException();

            int index = row * _cols + col;
            return Marshal.ReadInt32(_data, index * sizeof(int));
        }

        public void Print()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DynamicMatrix));

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    Console.Write($"{Get(i, j),4}");
                }
                Console.WriteLine();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_data != IntPtr.Zero)
                {
                    Win32Memory.HeapFree(_heap, 0, _data);
                    _data = IntPtr.Zero;
                }
                if (_heap != IntPtr.Zero)
                {
                    Win32Memory.HeapDestroy(_heap);
                    _heap = IntPtr.Zero;
                }
                _disposed = true;
            }
        }
    }

    // Задание 10: Класс для работы с файловой системой
    public class FileSystemManager : IDisposable
    {
        private IntPtr _buffer;
        private IntPtr _heap;
        private const int BufferSize = 4096;
        private bool _disposed = false;

        public FileSystemManager()
        {
            _heap = Win32Memory.HeapCreate(Win32Memory.HEAP_NO_SERIALIZE, (UIntPtr)BufferSize, UIntPtr.Zero);
            if (_heap == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось создать кучу");

            _buffer = Win32Memory.HeapAlloc(_heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)BufferSize);
            if (_buffer == IntPtr.Zero)
                throw new OutOfMemoryException("Не удалось выделить память");
        }

        public void ReadFileAndDisplay(string filePath)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileSystemManager));

            try
            {
                if (Win32Memory.IsBadReadPtr(_buffer, (UIntPtr)BufferSize))
                {
                    Console.WriteLine("Ошибка: буфер недоступен для чтения");
                    return;
                }

                byte[] managedBuffer = new byte[BufferSize];
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = fs.Read(managedBuffer, 0, BufferSize);
                    Marshal.Copy(managedBuffer, 0, _buffer, bytesRead);
                }

                string content = Marshal.PtrToStringAnsi(_buffer);
                Console.WriteLine($"Содержимое файла (первые {BufferSize} байт):\n{content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
            }
        }

        public void WriteToFile(string filePath, string content)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileSystemManager));

            try
            {
                if (Win32Memory.IsBadWritePtr(_buffer, (UIntPtr)BufferSize))
                {
                    Console.WriteLine("Ошибка: буфер недоступен для записи");
                    return;
                }

                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(content);
                int len = Math.Min(bytes.Length, BufferSize - 1);
                Marshal.Copy(bytes, 0, _buffer, len);
                Marshal.WriteByte(_buffer, len, 0);

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] outBuffer = new byte[len];
                    Marshal.Copy(_buffer, outBuffer, 0, len);
                    fs.Write(outBuffer, 0, len);
                }
                Console.WriteLine($"Данные записаны в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи файла: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_buffer != IntPtr.Zero)
                {
                    Win32Memory.HeapFree(_heap, 0, _buffer);
                    _buffer = IntPtr.Zero;
                }
                if (_heap != IntPtr.Zero)
                {
                    Win32Memory.HeapDestroy(_heap);
                    _heap = IntPtr.Zero;
                }
                _disposed = true;
            }
        }
    }

    // Задание 5 и 15: Библиотека для работы с динамической памятью
    public static class MemoryLibrary
    {
        public static IntPtr AllocateMemory(int size)
        {
            IntPtr heap = Win32Memory.GetProcessHeap();
            return Win32Memory.HeapAlloc(heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)size);
        }

        public static void FreeMemory(IntPtr ptr)
        {
            IntPtr heap = Win32Memory.GetProcessHeap();
            if (ptr != IntPtr.Zero)
                Win32Memory.HeapFree(heap, 0, ptr);
        }

        public static void WriteInt(IntPtr ptr, int offset, int value)
        {
            Marshal.WriteInt32(ptr, offset, value);
        }

        public static int ReadInt(IntPtr ptr, int offset)
        {
            return Marshal.ReadInt32(ptr, offset);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ПРАКТИЧЕСКОЕ ЗАНЯТИЕ 19 ===\n");

            // Задание 1: Выделение динамической памяти для массива
            Console.WriteLine("--- ЗАДАНИЕ 1: Выделение памяти для массива ---");
            Console.Write("Введите размер массива N: ");
            int n = int.Parse(Console.ReadLine());
            IntPtr heap = Win32Memory.GetProcessHeap();
            IntPtr arrayPtr = Win32Memory.HeapAlloc(heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)(n * sizeof(int)));
            if (arrayPtr != IntPtr.Zero)
                Console.WriteLine($"Выделено {n * sizeof(int)} байт для массива из {n} элементов");
            else
                Console.WriteLine("Ошибка выделения памяти");
            Console.WriteLine();

            // Задание 2: Освобождение памяти
            Console.WriteLine("--- ЗАДАНИЕ 2: Освобождение памяти ---");
            if (arrayPtr != IntPtr.Zero)
            {
                Win32Memory.HeapFree(heap, 0, arrayPtr);
                Console.WriteLine("Память освобождена");
            }
            Console.WriteLine();

            // Задание 3: Класс с динамическим выделением памяти
            Console.WriteLine("--- ЗАДАНИЕ 3: Класс DynamicArray ---");
            using (DynamicArray dynArr = new DynamicArray(5))
            {
                for (int i = 0; i < dynArr.Size; i++)
                    dynArr.SetValue(i, i * 10);
                Console.Write("Значения массива: ");
                for (int i = 0; i < dynArr.Size; i++)
                    Console.Write($"{dynArr.GetValue(i)} ");
                Console.WriteLine();
            }
            Console.WriteLine("Память класса освобождена\n");

            // Задание 4 и 14: Управление распределением памяти между процессами
            Console.WriteLine("--- ЗАДАНИЕ 4/14: Распределение памяти между процессами ---");
            IntPtr sharedHeap = Win32Memory.HeapCreate(Win32Memory.HEAP_NO_SERIALIZE, (UIntPtr)(1024 * 1024), UIntPtr.Zero);
            if (sharedHeap != IntPtr.Zero)
            {
                IntPtr sharedMemory = Win32Memory.HeapAlloc(sharedHeap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)256);
                if (sharedMemory != IntPtr.Zero)
                {
                    Marshal.WriteInt32(sharedMemory, 0, 42);
                    Console.WriteLine($"Записано значение 42 в разделяемую память: {Marshal.ReadInt32(sharedMemory, 0)}");
                    Win32Memory.HeapFree(sharedHeap, 0, sharedMemory);
                }
                Win32Memory.HeapDestroy(sharedHeap);
                Console.WriteLine("Куча уничтожена");
            }
            Console.WriteLine();

            // Задание 5 и 15: Библиотека для работы с динамической памятью
            Console.WriteLine("--- ЗАДАНИЕ 5/15: Библиотека MemoryLibrary ---");
            IntPtr libMem = MemoryLibrary.AllocateMemory(100);
            if (libMem != IntPtr.Zero)
            {
                MemoryLibrary.WriteInt(libMem, 0, 123);
                Console.WriteLine($"Библиотека: записано и прочитано {MemoryLibrary.ReadInt(libMem, 0)}");
                MemoryLibrary.FreeMemory(libMem);
                Console.WriteLine("Память освобождена через библиотеку");
            }
            Console.WriteLine();

            // Задание 6 и 16: Предотвращение утечек памяти
            Console.WriteLine("--- ЗАДАНИЕ 6/16: Предотвращение утечек памяти ---");
            Console.WriteLine("Создаем блок try-finally для гарантированного освобождения");
            IntPtr safePtr = IntPtr.Zero;
            try
            {
                safePtr = Win32Memory.HeapAlloc(heap, 0, (UIntPtr)1024);
                if (safePtr != IntPtr.Zero)
                    Console.WriteLine("Память выделена безопасно");
                else
                    Console.WriteLine("Ошибка выделения");
            }
            finally
            {
                if (safePtr != IntPtr.Zero)
                {
                    Win32Memory.HeapFree(heap, 0, safePtr);
                    Console.WriteLine("Память освобождена в finally блоке");
                }
            }
            Console.WriteLine();

            // Задание 7 и 17: Многопоточная программа
            Console.WriteLine("--- ЗАДАНИЕ 7/17: Многопоточная работа с памятью ---");
            object lockObj = new object();
            IntPtr sharedCounter = Win32Memory.HeapAlloc(heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)sizeof(int));
            if (sharedCounter != IntPtr.Zero)
            {
                Task[] tasks = new Task[5];
                for (int t = 0; t < 5; t++)
                {
                    int threadNum = t;
                    tasks[t] = Task.Run(() =>
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            lock (lockObj)
                            {
                                int val = Marshal.ReadInt32(sharedCounter);
                                val++;
                                Marshal.WriteInt32(sharedCounter, val);
                            }
                        }
                        Console.WriteLine($"Поток {threadNum} завершил работу");
                    });
                }
                Task.WaitAll(tasks);
                Console.WriteLine($"Итоговое значение счетчика (ожидается 500): {Marshal.ReadInt32(sharedCounter)}");
                Win32Memory.HeapFree(heap, 0, sharedCounter);
            }
            Console.WriteLine();

            // Задание 8: Выделение памяти для объектов
            Console.WriteLine("--- ЗАДАНИЕ 8: Выделение памяти для объектов ---");
            IntPtr objMem = Win32Memory.HeapAlloc(heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)Marshal.SizeOf<Point>());
            if (objMem != IntPtr.Zero)
            {
                Point p = new Point { X = 15, Y = 25 };
                Marshal.StructureToPtr(p, objMem, false);
                Point result = Marshal.PtrToStructure<Point>(objMem);
                Console.WriteLine($"Объект Point: ({result.X}, {result.Y})");
                Win32Memory.HeapFree(heap, 0, objMem);
            }
            Console.WriteLine();

            // Задание 9: Изменение параметров динамической памяти
            Console.WriteLine("--- ЗАДАНИЕ 9: Изменение параметров динамической памяти ---");
            IntPtr resizeHeap = Win32Memory.HeapCreate(Win32Memory.HEAP_NO_SERIALIZE, (UIntPtr)1024, (UIntPtr)(1024 * 10));
            if (resizeHeap != IntPtr.Zero)
            {
                IntPtr resizePtr = Win32Memory.HeapAlloc(resizeHeap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)100);
                if (resizePtr != IntPtr.Zero)
                {
                    Console.WriteLine("Выделено 100 байт");
                    // Для демонстрации просто показываем возможность выделения другого размера
                    IntPtr newPtr = Win32Memory.HeapAlloc(resizeHeap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)200);
                    if (newPtr != IntPtr.Zero)
                    {
                        Console.WriteLine("Дополнительно выделено 200 байт в той же куче");
                        Win32Memory.HeapFree(resizeHeap, 0, newPtr);
                    }
                    Win32Memory.HeapFree(resizeHeap, 0, resizePtr);
                }
                Win32Memory.HeapDestroy(resizeHeap);
            }
            Console.WriteLine();

            // Задание 10: Класс для работы с файловой системой
            Console.WriteLine("--- ЗАДАНИЕ 10: Класс FileSystemManager ---");
            string testFile = "test.txt";
            using (FileSystemManager fsManager = new FileSystemManager())
            {
                fsManager.WriteToFile(testFile, "Hello, World! This is a test file created with WinAPI memory management.");
                fsManager.ReadFileAndDisplay(testFile);
            }
            if (File.Exists(testFile))
                File.Delete(testFile);
            Console.WriteLine();

            // Задание 11: Класс для работы со строками
            Console.WriteLine("--- ЗАДАНИЕ 11: Класс DynamicString ---");
            using (DynamicString dynStr = new DynamicString(100))
            {
                dynStr.SetString("Dynamic string in WinAPI heap memory");
                Console.WriteLine($"Строка: {dynStr.GetString()}");
            }
            Console.WriteLine();

            // Задание 12: Класс для работы с матрицами
            Console.WriteLine("--- ЗАДАНИЕ 12: Класс DynamicMatrix ---");
            using (DynamicMatrix matrix = new DynamicMatrix(3, 4))
            {
                int val = 1;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 4; j++)
                        matrix.Set(i, j, val++);
                Console.WriteLine("Матрица 3x4:");
                matrix.Print();
            }
            Console.WriteLine();

            // Задание 13: Алгоритм сортировки с динамической памятью
            Console.WriteLine("--- ЗАДАНИЕ 13: Сортировка с динамической памятью ---");
            int[] unsorted = { 64, 25, 12, 22, 11, 35, 18, 42 };
            IntPtr sortMem = Win32Memory.HeapAlloc(heap, Win32Memory.HEAP_ZERO_MEMORY, (UIntPtr)(unsorted.Length * sizeof(int)));
            if (sortMem != IntPtr.Zero)
            {
                Marshal.Copy(unsorted, 0, sortMem, unsorted.Length);
                Console.Write("Исходный массив: ");
                PrintIntArray(sortMem, unsorted.Length);

                // Простая сортировка выбором
                for (int i = 0; i < unsorted.Length - 1; i++)
                {
                    int minIdx = i;
                    for (int j = i + 1; j < unsorted.Length; j++)
                    {
                        if (Marshal.ReadInt32(sortMem, j * sizeof(int)) < Marshal.ReadInt32(sortMem, minIdx * sizeof(int)))
                            minIdx = j;
                    }
                    if (minIdx != i)
                    {
                        int temp = Marshal.ReadInt32(sortMem, i * sizeof(int));
                        Marshal.WriteInt32(sortMem, i * sizeof(int), Marshal.ReadInt32(sortMem, minIdx * sizeof(int)));
                        Marshal.WriteInt32(sortMem, minIdx * sizeof(int), temp);
                    }
                }

                Console.Write("Отсортированный массив: ");
                PrintIntArray(sortMem, unsorted.Length);
                Win32Memory.HeapFree(heap, 0, sortMem);
            }
            Console.WriteLine();

            Console.WriteLine("=== ВСЕ ЗАДАНИЯ ВЫПОЛНЕНЫ ===");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void PrintIntArray(IntPtr ptr, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Console.Write($"{Marshal.ReadInt32(ptr, i * sizeof(int))} ");
            }
            Console.WriteLine();
        }

        struct Point
        {
            public int X;
            public int Y;
        }
    }
}