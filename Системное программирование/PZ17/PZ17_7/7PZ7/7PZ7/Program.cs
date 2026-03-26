using System;
using System.Runtime.InteropServices;
using System.Threading;

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

    static void ThreadProc(object param)
    {
        IntPtr ptr = (IntPtr)param;
        for (int i = 0; i < 10; i++)
        {
            Marshal.WriteInt32(ptr, i * sizeof(int), i * 2);
        }
    }

    static void Main()
    {
        uint size = (uint)(10 * sizeof(int));
        IntPtr sharedData = VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

        if (sharedData == IntPtr.Zero)
        {
            Console.WriteLine("Allocation failed");
            return;
        }

        Thread thread = new Thread(ThreadProc);
        thread.Start(sharedData);
        thread.Join();

        for (int i = 0; i < 10; i++)
        {
            Console.Write(Marshal.ReadInt32(sharedData, i * sizeof(int)) + " ");
        }
        Console.WriteLine();

        VirtualFree(sharedData, 0, MEM_RELEASE);
    }
}