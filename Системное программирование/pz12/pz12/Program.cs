using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;


namespace pz12
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Запуск всех заданий по многопоточности ===\n");

            Task1_FactorialAndMultiplicationTable.Run();

            Task2_ParallelFileReadWrite.Run();

            Task3_GaussMethod.Run();

            Task4_BigDataProcessing.Run();

            Task5_RandomNumbers.Run();

            Task6_AESEncryption.Run();

            Task7_GaussMethodParallel.Run();

            Task8_AESEncryptionParallel.Run();

            Task9_ImageProcessing.Run();

            Task10_ParallelFileDownload.Run();

            Task11_MathOperations.Run();

            Console.WriteLine("\n=== Все задания выполнены ===");
            Console.ReadKey();
        }
    }

    class Task1_FactorialAndMultiplicationTable
    {
        static object locker = new object();
        static int sharedCounter = 0;

        public static void Run()
        {
            Console.WriteLine("\n--- Задание 1: Факториал и таблица умножения ---");
            sharedCounter = 0;

            Thread threadFactorial = new Thread(CalculateFactorial);
            Thread threadMultiplication = new Thread(PrintMultiplicationTable);

            threadFactorial.Start();
            threadMultiplication.Start();

            threadFactorial.Join();
            threadMultiplication.Join();

            Console.WriteLine("Задание 1 завершено.\n");
        }

        static void CalculateFactorial()
        {
            int n = 6;
            long fact = 1;
            for (int i = 2; i <= n; i++)
            {
                lock (locker)
                {
                    fact *= i;
                    Console.WriteLine($"[Факториал] {i}! = {fact}, общий счётчик: {++sharedCounter}");
                }
                Thread.Sleep(300);
            }
        }

        static void PrintMultiplicationTable()
        {
            int number = 5;
            for (int i = 1; i <= 5; i++)
            {
                lock (locker)
                {
                    Console.WriteLine($"[Таблица] {number} x {i} = {number * i}, общий счётчик: {++sharedCounter}");
                }
                Thread.Sleep(300);
            }
        }
    }

    class Task2_ParallelFileReadWrite
    {
        static object fileLocker = new object();
        static string filePath = "test_parallel.txt";

        public static void Run()
        {
            Console.WriteLine("\n--- Задание 2: Параллельное чтение и запись файлов ---");

            if (File.Exists(filePath)) File.Delete(filePath);

            Thread writer1 = new Thread(() => WriteToFile("Запись от потока 1\n"));
            Thread writer2 = new Thread(() => WriteToFile("Запись от потока 2\n"));
            Thread reader = new Thread(ReadFromFile);

            writer1.Start();
            writer2.Start();
            writer1.Join();
            writer2.Join();
            reader.Start();
            reader.Join();

            Console.WriteLine("Задание 2 завершено.\n");
        }

        static void WriteToFile(string content)
        {
            lock (fileLocker)
            {
                File.AppendAllText(filePath, content);
                Console.WriteLine($"Записано: {content.Trim()}");
                Thread.Sleep(100);
            }
        }

        static void ReadFromFile()
        {
            lock (fileLocker)
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    Console.WriteLine($"Прочитано из файла:\n{content}");
                }
                else
                    Console.WriteLine("Файл не найден.");
            }
        }
    }

    class Task3_GaussMethod
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 3: Решение СЛАУ методом Гаусса ---");
            double[,] matrix = {
                { 2, 1, -1, 8 },
                { -3, -1, 2, -11 },
                { -2, 1, 2, -3 }
            };

            double[] result = SolveGauss(matrix);
            Console.WriteLine("Решение:");
            for (int i = 0; i < result.Length; i++)
                Console.WriteLine($"x{i + 1} = {result[i]:F2}");
        }

        static double[] SolveGauss(double[,] a)
        {
            int n = a.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                    if (Math.Abs(a[k, i]) > Math.Abs(a[maxRow, i]))
                        maxRow = k;
                for (int k = i; k <= n; k++)
                {
                    double temp = a[maxRow, k];
                    a[maxRow, k] = a[i, k];
                    a[i, k] = temp;
                }

                for (int k = i + 1; k <= n; k++)
                    a[i, k] /= a[i, i];
                a[i, i] = 1;

                for (int k = 0; k < n; k++)
                {
                    if (k != i && a[k, i] != 0)
                    {
                        double factor = a[k, i];
                        for (int j = i; j <= n; j++)
                            a[k, j] -= factor * a[i, j];
                    }
                }
            }

            double[] x = new double[n];
            for (int i = 0; i < n; i++)
                x[i] = a[i, n];
            return x;
        }
    }

    class Task4_BigDataProcessing
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 4: Обработка больших данных с многопоточностью ---");
            int[] data = Enumerable.Range(1, 100).ToArray();
            int threads = 4;
            int chunkSize = data.Length / threads;
            long sum = 0;
            object sumLocker = new object();

            Thread[] workers = new Thread[threads];
            for (int i = 0; i < threads; i++)
            {
                int start = i * chunkSize;
                int end = (i == threads - 1) ? data.Length : start + chunkSize;
                workers[i] = new Thread(() =>
                {
                    long partialSum = 0;
                    for (int j = start; j < end; j++)
                        partialSum += data[j];
                    lock (sumLocker)
                        sum += partialSum;
                });
                workers[i].Start();
            }

            foreach (var t in workers) t.Join();
            Console.WriteLine($"Сумма элементов (1..100) = {sum}");
        }
    }

    class Task5_RandomNumbers
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 5: Генерация случайных чисел ---");
            int threadsCount = 3;
            List<int> allNumbers = new List<int>();
            object listLocker = new object();
            Thread[] threads = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Random rand = new Random(Guid.NewGuid().GetHashCode());
                    List<int> local = new List<int>();
                    for (int j = 0; j < 5; j++)
                        local.Add(rand.Next(1, 100));
                    lock (listLocker)
                        allNumbers.AddRange(local);
                });
                threads[i].Start();
            }

            foreach (var t in threads) t.Join();
            Console.WriteLine("Сгенерированные числа: " + string.Join(", ", allNumbers));
        }
    }

    class Task6_AESEncryption
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 6: AES шифрование ---");
            string original = "Многопоточность в C#";
            using (Aes aes = Aes.Create())
            {
                byte[] encrypted = Encrypt(original, aes.Key, aes.IV);
                string decrypted = Decrypt(encrypted, aes.Key, aes.IV);
                Console.WriteLine($"Оригинал: {original}");
                Console.WriteLine($"Расшифровано: {decrypted}");
            }
        }

        static byte[] Encrypt(string text, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                var encryptor = aes.CreateEncryptor();
                byte[] plain = Encoding.UTF8.GetBytes(text);
                return encryptor.TransformFinalBlock(plain, 0, plain.Length);
            }
        }

        static string Decrypt(byte[] cipher, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                var decryptor = aes.CreateDecryptor();
                byte[] decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }

    class Task7_GaussMethodParallel
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 7: Метод Гаусса с многопоточностью ---");
            double[,] matrix = {
                { 2, 1, -1, 8 },
                { -3, -1, 2, -11 },
                { -2, 1, 2, -3 }
            };
            int n = matrix.GetLength(0);
            object locker = new object();

            for (int i = 0; i < n; i++)
            {
                double divisor = matrix[i, i];
                Parallel.For(i, n + 1, j =>
                {
                    lock (locker)
                        matrix[i, j] /= divisor;
                });

                Parallel.For(0, n, k =>
                {
                    if (k != i && matrix[k, i] != 0)
                    {
                        double factor = matrix[k, i];
                        for (int j = i; j <= n; j++)
                        {
                            lock (locker)
                                matrix[k, j] -= factor * matrix[i, j];
                        }
                    }
                });
            }

            double[] result = new double[n];
            for (int i = 0; i < n; i++)
                result[i] = matrix[i, n];
            Console.WriteLine("Решение (многопоточный Гаусс):");
            for (int i = 0; i < result.Length; i++)
                Console.WriteLine($"x{i + 1} = {result[i]:F2}");
        }
    }

    class Task8_AESEncryptionParallel
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 8: AES шифрование с многопоточностью ---");
            string[] texts = {
                "Первый блок данных для шифрования",
                "Второй блок данных для шифрования",
                "Третий блок данных для шифрования"
            };
            byte[][] encrypted = new byte[texts.Length][];
            byte[][] decrypted = new byte[texts.Length][];

            using (Aes aes = Aes.Create())
            {
                byte[] key = aes.Key;
                byte[] iv = aes.IV;

                Parallel.For(0, texts.Length, i =>
                {
                    encrypted[i] = Encrypt(texts[i], key, iv);
                    decrypted[i] = Decrypt(encrypted[i], key, iv);
                });

                for (int i = 0; i < texts.Length; i++)
                {
                    Console.WriteLine($"Исходный текст: {texts[i]}");
                    Console.WriteLine($"Расшифровано: {Encoding.UTF8.GetString(decrypted[i])}");
                }
            }
        }

        static byte[] Encrypt(string text, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                var encryptor = aes.CreateEncryptor();
                byte[] plain = Encoding.UTF8.GetBytes(text);
                return encryptor.TransformFinalBlock(plain, 0, plain.Length);
            }
        }

        static byte[] Decrypt(byte[] cipher, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                var decryptor = aes.CreateDecryptor();
                return decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            }
        }
    }

    class Task9_ImageProcessing
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 9: Обработка изображений (демонстрация) ---");
            Console.WriteLine("Имитация обработки изображения: изменение размера, поворот, обрезка");
            Bitmap dummy = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(dummy))
            {
                g.Clear(Color.Blue);
            }

            Thread resizeThread = new Thread(() => ResizeImage(dummy, 50, 50));
            Thread rotateThread = new Thread(() => RotateImage(dummy, 45));
            Thread cropThread = new Thread(() => CropImage(dummy, 20, 20, 60, 60));

            resizeThread.Start();
            rotateThread.Start();
            cropThread.Start();

            resizeThread.Join();
            rotateThread.Join();
            cropThread.Join();

            Console.WriteLine("Все операции над изображением выполнены (симуляция).");
        }

        static void ResizeImage(Bitmap img, int width, int height)
        {
            Thread.Sleep(200);
            Console.WriteLine($"  Изменение размера: {img.Width}x{img.Height} -> {width}x{height}");
        }

        static void RotateImage(Bitmap img, float angle)
        {
            Thread.Sleep(200);
            Console.WriteLine($"  Поворот на {angle} градусов");
        }

        static void CropImage(Bitmap img, int x, int y, int w, int h)
        {
            Thread.Sleep(200);
            Console.WriteLine($"  Обрезка: ({x},{y}) размер {w}x{h}");
        }
    }

    class Task10_ParallelFileDownload
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 10: Параллельная загрузка файлов ---");
            string[] urls = { "file1.dat", "file2.dat", "file3.dat", "file4.dat" };
            Thread[] threads = new Thread[urls.Length];

            for (int i = 0; i < urls.Length; i++)
            {
                string url = urls[i];
                threads[i] = new Thread(() => DownloadFile(url));
                threads[i].Start();
            }

            foreach (var t in threads) t.Join();
            Console.WriteLine("Все файлы загружены (симуляция).");
        }

        static void DownloadFile(string fileName)
        {
            Console.WriteLine($"  Начинаем загрузку: {fileName}");
            Thread.Sleep(500);   
            Console.WriteLine($"  Завершена загрузка: {fileName}");
        }
    }

    class Task11_MathOperations
    {
        public static void Run()
        {
            Console.WriteLine("\n--- Задание 11: Математические операции ---");
            object locker = new object();

            Thread sqrtThread = new Thread(() => ComputeSquareRoot(25));
            Thread gcdThread = new Thread(() => ComputeGCD(48, 18));
            Thread powThread = new Thread(() => ComputePower(2, 10));

            sqrtThread.Start();
            gcdThread.Start();
            powThread.Start();

            sqrtThread.Join();
            gcdThread.Join();
            powThread.Join();
        }

        static void ComputeSquareRoot(double x)
        {
            double result = Math.Sqrt(x);
            Console.WriteLine($"  Корень из {x} = {result:F4}");
        }

        static void ComputeGCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            Console.WriteLine($"  НОД(48,18) = {a}");
        }

        static void ComputePower(double baseNum, int exponent)
        {
            double result = Math.Pow(baseNum, exponent);
            Console.WriteLine($"  {baseNum}^{exponent} = {result}");
        }
    }
}