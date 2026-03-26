using System;
using System.Runtime.InteropServices;

public class DynamicArray : IDisposable
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

    const uint MEM_COMMIT = 0x1000;
    const uint MEM_RESERVE = 0x2000;
    const uint MEM_RELEASE = 0x8000;
    const uint PAGE_READWRITE = 0x04;

    private IntPtr _data;
    private int _size;
    private bool _disposed = false;

    public DynamicArray(int size)
    {
        _size = size;
        uint bytes = (uint)(size * sizeof(int));
        _data = VirtualAlloc(IntPtr.Zero, bytes, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        if (_data == IntPtr.Zero)
            throw new OutOfMemoryException();
    }

    public void Set(int index, int value)
    {
        if (index < 0 || index >= _size)
            throw new IndexOutOfRangeException();
        Marshal.WriteInt32(_data, index * sizeof(int), value);
    }

    public int Get(int index)
    {
        if (index < 0 || index >= _size)
            throw new IndexOutOfRangeException();
        return Marshal.ReadInt32(_data, index * sizeof(int));
    }

    public void Dispose()
    {
        if (!_disposed && _data != IntPtr.Zero)
        {
            VirtualFree(_data, 0, MEM_RELEASE);
            _data = IntPtr.Zero;
            _disposed = true;
        }
    }
}

class Program
{
    static void Main()
    {
        using (DynamicArray arr = new DynamicArray(10))
        {
            for (int i = 0; i < 10; i++)
                arr.Set(i, i * 5);
            for (int i = 0; i < 10; i++)
                Console.Write(arr.Get(i) + " ");
            Console.WriteLine();
        }
    }
}