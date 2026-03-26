using System;
using System.Runtime.InteropServices;

struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Print()
    {
        Console.WriteLine($"Point({X}, {Y})");
    }
}

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
        uint size = (uint)Marshal.SizeOf<Point>();
        IntPtr rawMem = VirtualAlloc(IntPtr.Zero, size, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

        if (rawMem == IntPtr.Zero)
        {
            Console.WriteLine("Allocation failed");
            return;
        }

        // Создаём объект в неуправляемой памяти
        Point p = new Point(10, 20);

        // Копируем объект в неуправляемую память
        Marshal.StructureToPtr(p, rawMem, false);

        // Читаем обратно
        Point p2 = Marshal.PtrToStructure<Point>(rawMem);
        p2.Print();

        VirtualFree(rawMem, 0, MEM_RELEASE);
    }
}