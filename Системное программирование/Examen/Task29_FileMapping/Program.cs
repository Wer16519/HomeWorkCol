using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Task29_FileMapping
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Проецирование файлов (File Mapping) ===\n");

            string filePath = "testfile.txt";
            long fileSize = 1024;

            try
            {
                if (!File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fs.SetLength(fileSize);
                    }
                    Console.WriteLine($"[Создан] Файл {filePath} размером {fileSize} байт");
                }

                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, "TestMap", fileSize))
                {
                    Console.WriteLine("[Создано] Проецирование файла в память");

                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(0, fileSize))
                    {
                        string writeData = "Привет из проецированного файла!";
                        byte[] writeBytes = Encoding.UTF8.GetBytes(writeData);
                        accessor.WriteArray(0, writeBytes, 0, writeBytes.Length);
                        Console.WriteLine($"[Запись] Данные записаны: \"{writeData}\"");

                        byte[] readBytes = new byte[writeBytes.Length];
                        accessor.ReadArray(0, readBytes, 0, readBytes.Length);
                        string readData = Encoding.UTF8.GetString(readBytes);
                        Console.WriteLine($"[Чтение] Данные прочитаны: \"{readData}\"");
                    }
                }

                Console.WriteLine("\n[Содержимое файла]");
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string content = reader.ReadToEnd();
                    Console.WriteLine(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\n[Завершение] Программа завершена");
                Console.ReadKey();
            }
        }
    }
}