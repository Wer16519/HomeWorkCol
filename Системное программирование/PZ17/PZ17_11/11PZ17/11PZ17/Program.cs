using System;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

    const uint MEM_COMMIT = 0x1000;
    const uint MEM_RESERVE = 0x2000;
    const uint MEM_RELEASE = 0x8000;
    const uint PAGE_NOACCESS = 0x01;
    const uint PAGE_READWRITE = 0x04;

    static void Main()
    {
        // Резервируем адресное пространство (1 MB)
        IntPtr baseAddr = VirtualAlloc(IntPtr.Zero, 1024 * 1024, MEM_RESERVE, PAGE_NOACCESS);
        if (baseAddr == IntPtr.Zero)
        {
            Console.WriteLine("Reservation failed");
            return;
        }
        Console.WriteLine("Reserved 1 MB");

        // Фиксируем первую страницу
        IntPtr committed = VirtualAlloc(baseAddr, 4096, MEM_COMMIT, PAGE_READWRITE);
        if (committed == IntPtr.Zero)
        {
            VirtualFree(baseAddr, 0, MEM_RELEASE);
            Console.WriteLine("Commit failed");
            return;
        }
        Console.WriteLine("Committed 1 page");

        // Используем память
        for (int i = 0; i < 1024; i++)
        {
            Marshal.WriteInt32(committed, i * sizeof(int), i);
        }

        // Выводим первые 10 значений
        for (int i = 0; i < 10; i++)
        {
            Console.Write(Marshal.ReadInt32(committed, i * sizeof(int)) + " ");
        }
        Console.WriteLine();

        // Освобождаем всю зарезервированную область
        VirtualFree(baseAddr, 0, MEM_RELEASE);
        Console.WriteLine("All released");
    }
}