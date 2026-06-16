using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Task31_FileCompression
{
    public class FileCompressor
    {
        public void CompressFile(string sourceFile, string destFile)
        {
            try
            {
                byte[] data = File.ReadAllBytes(sourceFile);
                Console.WriteLine($"[Сжатие] Исходный размер: {data.Length} байт");

                using (FileStream destStream = new FileStream(destFile, FileMode.Create))
                using (GZipStream gzipStream = new GZipStream(destStream, CompressionLevel.Optimal))
                {
                    gzipStream.Write(data, 0, data.Length);
                }

                long compressedSize = new FileInfo(destFile).Length;
                Console.WriteLine($"[Сжатие] Сжатый размер: {compressedSize} байт");
                Console.WriteLine($"[Сжатие] Коэффициент сжатия: {(double)compressedSize / data.Length * 100:F2}%");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка сжатия] {ex.Message}");
            }
        }

        public void DecompressFile(string sourceFile, string destFile)
        {
            try
            {
                using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
                using (GZipStream gzipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    gzipStream.CopyTo(memoryStream);
                    byte[] data = memoryStream.ToArray();
                    File.WriteAllBytes(destFile, data);
                    Console.WriteLine($"[Распаковка] Размер после распаковки: {data.Length} байт");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка распаковки] {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Сжатие и распаковка данных ===\n");

            string originalFile = "original.txt";
            string compressedFile = "compressed.gz";
            string decompressedFile = "decompressed.txt";

            string content = "Это тестовый текст для сжатия. " +
                           "Здесь содержится много повторяющихся слов и фраз. " +
                           "Сжатие должно работать эффективно. " +
                           "Это тестовый текст для сжатия. " +
                           "Здесь содержится много повторяющихся слов и фраз. " +
                           "Сжатие должно работать эффективно. " +
                           "Повторение - мать учения. " +
                           "Тестовые данные для демонстрации работы алгоритма сжатия.";

            File.WriteAllText(originalFile, content, Encoding.UTF8);
            Console.WriteLine($"[Создан] Исходный файл: {originalFile}");
            Console.WriteLine($"Размер: {new FileInfo(originalFile).Length} байт");

            FileCompressor compressor = new FileCompressor();

            compressor.CompressFile(originalFile, compressedFile);

            compressor.DecompressFile(compressedFile, decompressedFile);

            string decompressedContent = File.ReadAllText(decompressedFile, Encoding.UTF8);
            bool isEqual = content == decompressedContent;
            Console.WriteLine($"\n[Проверка] Данные совпадают: {isEqual}");

            if (isEqual)
            {
                Console.WriteLine("[Результат] Сжатие и распаковка выполнены успешно!");
            }

            Console.WriteLine($"\nФайлы:");
            Console.WriteLine($"- {originalFile}: {new FileInfo(originalFile).Length} байт");
            Console.WriteLine($"- {compressedFile}: {new FileInfo(compressedFile).Length} байт");
            Console.WriteLine($"- {decompressedFile}: {new FileInfo(decompressedFile).Length} байт");

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}