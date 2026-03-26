using System;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

    const uint MEM_COMMIT = 0x1000;
    const uint MEM_RESERVE = 0x2000;
    const uint MEM_RELEASE = 0x8000;
    const uint PAGE_READWRITE = 0x04;
    const uint PAGE_READONLY = 0x02;

    static void Main()
    {
        uint size = 4096;
        IntPtr ptr = VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

        if (ptr == IntPtr.Zero)
        {
            Console.WriteLine("Allocation failed");
            return;
        }

        // Меняем защиту на только чтение
        if (!VirtualProtect(ptr, size, PAGE_READONLY, out uint oldProtect))
        {
            Console.WriteLine("VirtualProtect failed");
        }
        else
        {
            Console.WriteLine("Memory set to READONLY");
        }

        // Попытка записи вызовет нарушение доступа (закомментировано)
        // Marshal.WriteInt32(ptr, 0, 123);

        VirtualFree(ptr, 0, MEM_RELEASE);
    }
}