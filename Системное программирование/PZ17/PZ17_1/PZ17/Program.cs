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
    const uint PAGE_READWRITE = 0x04;

    static void Main()
    {
        int N = 100;
        uint size = (uint)(N * sizeof(int));

        IntPtr ptr = VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        if (ptr == IntPtr.Zero)
        {
            Console.WriteLine("Memory allocation failed");
            return;
        }

        // Записываем данные
        for (int i = 0; i < N; i++)
        {
            Marshal.WriteInt32(ptr, i * sizeof(int), i * 10);
        }

        // Читаем данные
        for (int i = 0; i < N; i++)
        {
            Console.Write(Marshal.ReadInt32(ptr, i * sizeof(int)) + " ");
        }
        Console.WriteLine();

        VirtualFree(ptr, 0, MEM_RELEASE);
    }
}