using System;
using System.Runtime.InteropServices;

class SafeVirtualMemory : IDisposable
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

    const uint MEM_COMMIT = 0x1000;
    const uint MEM_RESERVE = 0x2000;
    const uint MEM_RELEASE = 0x8000;
    const uint PAGE_READWRITE = 0x04;

    private IntPtr _ptr;
    private bool _disposed = false;

    public SafeVirtualMemory(uint size)
    {
        _ptr = VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        if (_ptr == IntPtr.Zero)
            throw new OutOfMemoryException();
        Console.WriteLine("Memory allocated");
    }

    public IntPtr GetPointer() => _ptr;

    public void Dispose()
    {
        if (!_disposed && _ptr != IntPtr.Zero)
        {
            VirtualFree(_ptr, 0, MEM_RELEASE);
            _ptr = IntPtr.Zero;
            Console.WriteLine("Memory freed");
            _disposed = true;
        }
    }
}

class Program
{
    static void Main()
    {
        using (var mem = new SafeVirtualMemory(1024))
        {
            // Используем память
            Marshal.WriteInt32(mem.GetPointer(), 0, 12345);
            int value = Marshal.ReadInt32(mem.GetPointer(), 0);
            Console.WriteLine($"Value: {value}");
        } // Автоматическое освобождение
    }
}