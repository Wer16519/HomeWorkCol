using System;
using System.Runtime.InteropServices;

namespace VirtualMemoryLib
{
    public class VirtualMemory : IDisposable
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint MEM_RELEASE = 0x8000;
        private const uint PAGE_READWRITE = 0x04;

        private IntPtr _ptr;
        private uint _size;
        private bool _disposed = false;

        public VirtualMemory(uint size)
        {
            _size = size;
            _ptr = VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (_ptr == IntPtr.Zero)
                throw new OutOfMemoryException();
        }

        public IntPtr GetPointer() => _ptr;

        public bool Protect(uint flNewProtect)
        {
            return VirtualProtect(_ptr, _size, flNewProtect, out _);
        }

        public void Dispose()
        {
            if (!_disposed && _ptr != IntPtr.Zero)
            {
                VirtualFree(_ptr, 0, MEM_RELEASE);
                _ptr = IntPtr.Zero;
                _disposed = true;
            }
        }
        class Program
        {
            static void Main(string[] args)
            {
                try
                {
                    Console.WriteLine("Тестирование VirtualMemory...");

                    // Выделяем 1 KB виртуальной памяти
                    using (var memory = new VirtualMemory(1024))
                    {
                        IntPtr ptr = memory.GetPointer();
                        Console.WriteLine($"Выделено памяти по адресу: 0x{ptr.ToInt64():X}");

                        // Пример записи в память
                        Marshal.WriteByte(ptr, 0, 0x42);
                        byte value = Marshal.ReadByte(ptr, 0);
                        Console.WriteLine($"Прочитано значение: 0x{value:X}");

                        // Изменяем защиту памяти
                        bool result = memory.Protect(0x10); // PAGE_EXECUTE
                        Console.WriteLine($"Защита памяти изменена: {result}");
                    }

                    Console.WriteLine("Память успешно освобождена");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }
    }
}