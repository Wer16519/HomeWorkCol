using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MemoryMappedFilesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрация всех заданий по проецированию файлов ===\n");

            Task1_CreateWriteAndDelete();
            Task2_ClassForMemoryMappedFiles();
            Task3_CachingMechanism();
            Task4_RemoteFileAccessSimulation();
            Task5_AccessManagementBetweenProcesses();
            Task6_CompressionAndDecompression();
            Task7_MultithreadedProgram();
            Task8_LibraryDemonstration();
            Task9_MemoryManagementSystem();
            Task10_WorkingWithLargeFiles();
            Task11_UsingApiFunctionsDirectly();
            Task12_ProjectFileToRemoteServer();
            Task13_DeleteProjectedFilesAfterUse();

            Console.WriteLine("\n=== Все задания выполнены ===");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void Task1_CreateWriteAndDelete()
        {
            Console.WriteLine("--- Задание 1: Создание, запись и удаление проецированного файла ---");
            string filePath = "task1_data.txt";
            string mapName = "Task1Map";
            long capacity = 1024;

            using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, mapName, capacity))
            {
                using (var accessor = mmf.CreateViewAccessor())
                {
                    byte[] data = Encoding.UTF8.GetBytes("Hello from Task 1!");
                    accessor.WriteArray(0, data, 0, data.Length);
                    accessor.Flush();

                    byte[] buffer = new byte[20];
                    accessor.ReadArray(0, buffer, 0, 20);
                    string result = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                    Console.WriteLine($"Записано и прочитано: {result}");
                }
            }

            if (File.Exists(filePath))
                File.Delete(filePath);
            Console.WriteLine("Файл удален.\n");
        }

        static void Task2_ClassForMemoryMappedFiles()
        {
            Console.WriteLine("--- Задание 2: Разработка класса для работы с проецированными файлами ---");
            string filePath = "task2.dat";
            using (var mmfHelper = new MemoryMappedFileHelper(filePath, 512))
            {
                mmfHelper.WriteData(0, "Task 2 data");
                string data = mmfHelper.ReadData(0, 12);
                Console.WriteLine($"Прочитано через класс: {data}");
            }
            if (File.Exists(filePath))
                File.Delete(filePath);
            Console.WriteLine();
        }

        static void Task3_CachingMechanism()
        {
            Console.WriteLine("--- Задание 3: Реализация механизма кэширования проецированных файлов ---");
            var cache = new MemoryMappedFileCache();
            string key = "cachedItem";
            string value = "Important cached data";
            cache.AddOrUpdate(key, value);
            string retrieved = cache.Get(key);
            Console.WriteLine($"Из кэша получено: {retrieved}");
            cache.Remove(key);
            Console.WriteLine();
        }

        static void Task4_RemoteFileAccessSimulation()
        {
            Console.WriteLine("--- Задание 4: Работа с файлами на удаленном сервере (симуляция) ---");
            string localCopy = "remote_sim.dat";
            File.WriteAllText(localCopy, "Remote file content");

            using (var mmf = MemoryMappedFile.CreateFromFile(localCopy, FileMode.Open))
            using (var view = mmf.CreateViewAccessor())
            {
                byte[] buffer = new byte[50];
                view.ReadArray(0, buffer, 0, 50);
                string content = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                Console.WriteLine($"Содержимое удаленного файла: {content}");
            }
            File.Delete(localCopy);
            Console.WriteLine();
        }

        static void Task5_AccessManagementBetweenProcesses()
        {
            Console.WriteLine("--- Задание 5: Управление доступом к проецированным файлам между процессами ---");
            using (var mutex = new Mutex(false, "Global\\Task5Mutex"))
            using (var mmf = MemoryMappedFile.CreateOrOpen("Task5Shared", 1024))
            using (var view = mmf.CreateViewAccessor())
            {
                mutex.WaitOne();
                try
                {
                    view.Write(0, 42);
                    int value = view.ReadInt32(0);
                    Console.WriteLine($"Записано и прочитано значение с синхронизацией: {value}");
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            Console.WriteLine();
        }

        static void Task6_CompressionAndDecompression()
        {
            Console.WriteLine("--- Задание 6: Реализация алгоритма сжатия и распаковки в проецированном файле ---");
            string originalText = "Это текст для сжатия. Он должен быть достаточно длинным, чтобы сжатие имело смысл. Повторяющиеся данные: aaaaa bbbbb ccccc ddddd eeeee fffff ggggg hhhhh iiiii jjjjj.";
            byte[] originalBytes = Encoding.UTF8.GetBytes(originalText);
            byte[] compressed = CompressionHelper.Compress(originalBytes);
            byte[] decompressed = CompressionHelper.Decompress(compressed);
            string result = Encoding.UTF8.GetString(decompressed);
            Console.WriteLine($"Сжатие и распаковка успешны: {originalText == result}");

            string filePath = "task6_compressed.dat";
            using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, "Task6Map", compressed.Length + 100))
            using (var view = mmf.CreateViewAccessor())
            {
                view.WriteArray(0, compressed, 0, compressed.Length);
                view.Flush();

                byte[] readCompressed = new byte[compressed.Length];
                view.ReadArray(0, readCompressed, 0, compressed.Length);
                byte[] readDecompressed = CompressionHelper.Decompress(readCompressed);
                string readResult = Encoding.UTF8.GetString(readDecompressed);
                Console.WriteLine($"Прочитано из проецированного файла после распаковки: {readResult.Substring(0, 50)}...");
            }
            if (File.Exists(filePath))
                File.Delete(filePath);
            Console.WriteLine();
        }

        static void Task7_MultithreadedProgram()
        {
            Console.WriteLine("--- Задание 7: Создание многопоточной программы, использующей проецированные файлы ---");
            string filePath = "task7_multithread.dat";

            using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, "Task7Map", 4096))
            {
                Parallel.For(0, 4, i =>
                {
                    using (var view = mmf.CreateViewAccessor())
                    {
                        int offset = i * sizeof(int);
                        view.Write(offset, i * 100);
                        int readValue = view.ReadInt32(offset);
                        Console.WriteLine($"Поток {i}: записал {i * 100}, прочитал {readValue}");
                    }
                });
            }

            if (File.Exists(filePath))
                File.Delete(filePath);
            Console.WriteLine();
        }

        static void Task8_LibraryDemonstration()
        {
            Console.WriteLine("--- Задание 8: Разработка библиотеки для работы с проецированными файлами ---");
            string filePath = "task8_lib.dat";
            using (var lib = new MemoryMappedLibrary(filePath, 256))
            {
                lib.WriteString(0, "Library test");
                string read = lib.ReadString(0, 12);
                Console.WriteLine($"Библиотека прочитала: {read}");
            }
            if (File.Exists(filePath))
                File.Delete(filePath);
            Console.WriteLine();
        }

        static void Task9_MemoryManagementSystem()
        {
            Console.WriteLine("--- Задание 9: Реализация системы управления памятью для работы с проецированными файлами ---");
            string filePath = "task9.dat";
            using (var memManager = new MemoryManager(filePath, 1024))
            {
                int blockId = memManager.Allocate(128);
                memManager.Write(blockId, "Data in block");
                string data = memManager.Read(blockId);
                Console.WriteLine($"Прочитано из блока {blockId}: {data}");
                memManager.Free(blockId);
            }
            if (File.Exists(filePath))
                File.Delete(filePath);
            Console.WriteLine();
        }

        static void Task10_WorkingWithLargeFiles()
        {
            Console.WriteLine("--- Задание 10: Создание программы с использованием проецированных файлов для работы с большими файлами ---");
            string largeFilePath = "largefile.dat";
            long largeSize = 100 * 1024 * 1024;   

            Console.WriteLine("Создание большого файла (100 МБ)...");
            using (var fs = new FileStream(largeFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.SetLength(largeSize);
            }

            Console.WriteLine("Работа с проецированным файлом...");
            using (var mmf = MemoryMappedFile.CreateFromFile(largeFilePath, FileMode.Open))
            {
                using (var view = mmf.CreateViewAccessor(0, 1024))       
                {
                    view.Write(0, 123456789);
                    int value = view.ReadInt32(0);
                    Console.WriteLine($"Записано и прочитано значение в большом файле: {value}");
                }
            }

            if (File.Exists(largeFilePath))
            {
                File.Delete(largeFilePath);
                Console.WriteLine("Большой файл удален");
            }
            Console.WriteLine();
        }

        static void Task11_UsingApiFunctionsDirectly()
        {
            Console.WriteLine("--- Задание 11: Написание программы, которая использует API-функции для работы с проецированными файлами ---");
            IntPtr hFileMapping = WinApi.CreateFileMapping(
                new IntPtr(-1),      
                IntPtr.Zero,
                WinApi.PageReadWrite,
                0,
                1024,
                "Global\\ApiMapping");

            if (hFileMapping != IntPtr.Zero)
            {
                IntPtr pView = WinApi.MapViewOfFile(hFileMapping, WinApi.FileMapWrite, 0, 0, 1024);
                if (pView != IntPtr.Zero)
                {
                    Marshal.WriteInt32(pView, 0, 999);
                    int value = Marshal.ReadInt32(pView, 0);
                    Console.WriteLine($"Через API записано и прочитано: {value}");
                    WinApi.UnmapViewOfFile(pView);
                }
                WinApi.CloseHandle(hFileMapping);
            }
            Console.WriteLine();
        }

        static void Task12_ProjectFileToRemoteServer()
        {
            Console.WriteLine("--- Задание 12: Создание программы для проецирования файлов на удаленный сервер с использованием API-функций ---");
            string simPath = "remote_proj_sim.dat";
            File.WriteAllText(simPath, "Data for remote projection");

            IntPtr hFile = WinApi.CreateFile(simPath, WinApi.GenericReadWrite, WinApi.FileShareReadWrite, IntPtr.Zero,
                WinApi.OpenAlways, WinApi.FileAttributeNormal, IntPtr.Zero);

            if (hFile.ToInt64() != -1)
            {
                IntPtr hMapping = WinApi.CreateFileMapping(hFile, IntPtr.Zero, WinApi.PageReadWrite, 0, 1024, null);
                if (hMapping != IntPtr.Zero)
                {
                    IntPtr pView = WinApi.MapViewOfFile(hMapping, WinApi.FileMapWrite, 0, 0, 1024);
                    if (pView != IntPtr.Zero)
                    {
                        string testData = "Data written via API";
                        byte[] data = Encoding.UTF8.GetBytes(testData);
                        Marshal.Copy(data, 0, pView, data.Length);

                        byte[] buffer = new byte[100];
                        Marshal.Copy(pView, buffer, 0, 100);
                        string readData = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        Console.WriteLine($"Данные из файла: {readData}");

                        WinApi.UnmapViewOfFile(pView);
                    }
                    WinApi.CloseHandle(hMapping);
                }
                WinApi.CloseHandle(hFile);
            }

            if (File.Exists(simPath))
                File.Delete(simPath);
            Console.WriteLine();
        }

        static void Task13_DeleteProjectedFilesAfterUse()
        {
            Console.WriteLine("--- Задание 13: Разработка программы для удаления проецированных файлов после их использования ---");
            string filePath = "task13_delete.dat";
            using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, "Task13Map", 256))
            {
                using (var view = mmf.CreateViewAccessor())
                {
                    view.Write(0, 777);
                    int val = view.ReadInt32(0);
                    Console.WriteLine($"Записано и прочитано перед удалением: {val}");
                }
            }

            if (File.Exists(filePath))
                File.Delete(filePath);
            Console.WriteLine("Файл успешно удален после использования.\n");
        }
    }

    class MemoryMappedFileHelper : IDisposable
    {
        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _accessor;

        public MemoryMappedFileHelper(string filePath, long capacity)
        {
            _mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, null, capacity);
            _accessor = _mmf.CreateViewAccessor();
        }

        public void WriteData(long position, string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            _accessor.WriteArray(position, bytes, 0, bytes.Length);
            _accessor.Flush();
        }

        public string ReadData(long position, int length)
        {
            byte[] buffer = new byte[length];
            _accessor.ReadArray(position, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
        }
    }

    class MemoryMappedFileCache
    {
        private ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

        public void AddOrUpdate(string key, string value) => _cache.AddOrUpdate(key, value, (k, v) => value);
        public string Get(string key) => _cache.TryGetValue(key, out var value) ? value : null;
        public void Remove(string key) => _cache.TryRemove(key, out _);
    }

    static class CompressionHelper
    {
        public static byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            using (var gz = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress))
            {
                gz.Write(data, 0, data.Length);
                gz.Close();
                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            using (var ms = new MemoryStream(compressedData))
            using (var gz = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
            using (var result = new MemoryStream())
            {
                gz.CopyTo(result);
                return result.ToArray();
            }
        }
    }

    class MemoryMappedLibrary : IDisposable
    {
        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _accessor;

        public MemoryMappedLibrary(string filePath, long capacity)
        {
            _mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, null, capacity);
            _accessor = _mmf.CreateViewAccessor();
        }

        public void WriteString(long position, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            _accessor.WriteArray(position, bytes, 0, bytes.Length);
            _accessor.Flush();
        }

        public string ReadString(long position, int length)
        {
            byte[] buffer = new byte[length];
            _accessor.ReadArray(position, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
        }
    }

    class MemoryManager : IDisposable
    {
        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _accessor;
        private int _nextBlock = 0;
        private const int BlockSize = 256;      

        public MemoryManager(string filePath, long capacity)
        {
            _mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, null, capacity);
            _accessor = _mmf.CreateViewAccessor();
        }

        public int Allocate(int size)
        {
            int blockId = _nextBlock++;
            _accessor.Write(blockId * BlockSize, blockId);
            return blockId;
        }

        public void Write(int blockId, string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            long position = blockId * BlockSize + sizeof(int);
            int bytesToWrite = Math.Min(bytes.Length, BlockSize - sizeof(int));
            _accessor.WriteArray(position, bytes, 0, bytesToWrite);
            _accessor.Flush();
        }

        public string Read(int blockId)
        {
            byte[] buffer = new byte[BlockSize - sizeof(int)];
            long position = blockId * BlockSize + sizeof(int);
            _accessor.ReadArray(position, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public void Free(int blockId)
        {
            Console.WriteLine($"Блок {blockId} освобожден (симуляция)");
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
        }
    }

    static class WinApi
    {
        public const uint GenericRead = 0x80000000;
        public const uint GenericWrite = 0x40000000;
        public const uint GenericReadWrite = GenericRead | GenericWrite;
        public const uint FileShareRead = 0x00000001;
        public const uint FileShareWrite = 0x00000002;
        public const uint FileShareReadWrite = FileShareRead | FileShareWrite;
        public const uint OpenExisting = 3;
        public const uint OpenAlways = 4;
        public const uint FileAttributeNormal = 0x80;
        public const uint PageReadWrite = 0x04;
        public const uint FileMapWrite = 0x0002;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, uint flProtect,
            uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess,
            uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);
    }
}