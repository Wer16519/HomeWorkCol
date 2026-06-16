using System;
using System.Runtime.InteropServices;
using System.Text;

namespace t7
{
    class Program
    {
        const uint GENERIC_WRITE = 0x40000000;
        const uint GENERIC_READ = 0x80000000;
        const uint CREATE_ALWAYS = 2;
        const uint OPEN_EXISTING = 3;
        const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        static void Main()
        {
            string fileName = "test.txt";

            Console.Write("Введите текст для записи в файл: ");
            string text = Console.ReadLine();
            byte[] buffer = Encoding.UTF8.GetBytes(text);

            // Создание и запись
            IntPtr hFile = CreateFile(fileName, GENERIC_WRITE, 0, IntPtr.Zero,
                CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (hFile == IntPtr.Zero || hFile.ToInt64() == -1)
            {
                Console.WriteLine("Ошибка создания файла!");
                return;
            }

            uint bytesWritten;
            WriteFile(hFile, buffer, (uint)buffer.Length, out bytesWritten, IntPtr.Zero);
            CloseHandle(hFile);

            Console.WriteLine($"Записано байт: {bytesWritten}");

            // Чтение
            hFile = CreateFile(fileName, GENERIC_READ, 0, IntPtr.Zero,
                OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (hFile == IntPtr.Zero || hFile.ToInt64() == -1)
            {
                Console.WriteLine("Ошибка открытия файла!");
                return;
            }

            byte[] readBuffer = new byte[1024];
            uint bytesRead;
            ReadFile(hFile, readBuffer, (uint)readBuffer.Length, out bytesRead, IntPtr.Zero);
            CloseHandle(hFile);

            string result = Encoding.UTF8.GetString(readBuffer, 0, (int)bytesRead);
            Console.WriteLine("\nСодержимое файла:");
            Console.WriteLine(result);

            Console.ReadKey();
        }
    }
}