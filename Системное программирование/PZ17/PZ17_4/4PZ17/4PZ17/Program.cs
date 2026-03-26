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
        const long LARGE_SIZE = 100 * 1024 * 1024; // 100 MB
        IntPtr ptr = VirtualAlloc(IntPtr.Zero, (uint)LARGE_SIZE, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

        if (ptr == IntPtr.Zero)
        {
            Console.WriteLine("Allocation failed");
            return;
        }

        // Работа с большим блоком
        unsafe
        {
            byte* buffer = (byte*)ptr.ToPointer();
            for (long i = 0; i < LARGE_SIZE; i++)
                buffer[i] = (byte)'A';
        }

        Console.WriteLine("Large memory used successfully");
        VirtualFree(ptr, 0, MEM_RELEASE);
    }
}