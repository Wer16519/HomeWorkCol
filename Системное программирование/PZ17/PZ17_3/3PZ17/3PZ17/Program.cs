using System;
using System.IO.MemoryMappedFiles;

class Program
{
    static void Main()
    {
        // Создаём разделяемую память
        using (var mmf = MemoryMappedFile.CreateNew("SharedMemory", 4096))
        {
            using (var accessor = mmf.CreateViewAccessor(0, 4096))
            {
                // Записываем данные
                accessor.Write(0, 12345);
                Console.WriteLine($"Written: {accessor.ReadInt32(0)}");
            }
        }

        // Другой процесс может открыть так же:
        // using (var mmf = MemoryMappedFile.OpenExisting("SharedMemory"))
        // {
        //     using (var accessor = mmf.CreateViewAccessor(0, 4096))
        //     {
        //         int value = accessor.ReadInt32(0);
        //     }
        // }
    }
}