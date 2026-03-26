using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ПРАКТИЧЕСКАЯ РАБОТА 4: ПАРАЛЛЕЛЬНАЯ ОБРАБОТКА ===");
            Console.WriteLine();

            Console.WriteLine("ЗАДАНИЕ 2: Анализ регистра букв");
            string text2 = "Hello World! This is a Test String with Mixed CASE letters.";
            Console.WriteLine($"Исходный текст: {text2}");
            string result2 = AnalyzeAndConvertLetters(text2);
            Console.WriteLine($"Результат: {result2}");
            Console.WriteLine();

            Console.WriteLine("ЗАДАНИЕ 3: Анализ позиций букв в словах");
            string text3 = "programming test hello world algorithm data structure";
            Console.WriteLine($"Исходный текст: {text3}");
            AnalyzeLetterPositions(text3);
            Console.WriteLine();

            Console.WriteLine("ЗАДАНИЕ 4: Статистика набора чисел");
            int[] numbers = { 12, 45, 67, 23, 89, 34, 56, 78, 90, 11, 33, 55, 77, 99, 22 };
            Console.WriteLine($"Набор чисел: {string.Join(", ", numbers)}");
            CalculateStatistics(numbers);
            Console.WriteLine();

            Console.WriteLine("ДОПОЛНИТЕЛЬНО: Сравнение производительности");
            ComparePerformance();
        }

        static string AnalyzeAndConvertLetters(string text)
        {
            int upperCount = 0;
            int lowerCount = 0;

            Parallel.For(0, text.Length, i =>
            {
                char c = text[i];
                if (char.IsLetter(c))
                {
                    if (char.IsUpper(c))
                        Interlocked.Increment(ref upperCount);
                    else
                        Interlocked.Increment(ref lowerCount);
                }
            });

            Console.WriteLine($"Прописных букв: {upperCount}, строчных: {lowerCount}");

            if (upperCount > lowerCount)
            {
                return text.ToUpper();
            }
            else if (lowerCount > upperCount)
            {
                return text.ToLower();
            }
            else
            {
                return text;   
            }
        }

        static void AnalyzeLetterPositions(string text)
        {
            string[] words = text.Split(' ').Where(word => !string.IsNullOrEmpty(word)).ToArray();
            int firstLetterCount = 0;
            int lastLetterCount = 0;
            int middleLetterCount = 0;

            Parallel.ForEach(words, word =>
            {
                if (word.Length > 0)
                {
                    Interlocked.Increment(ref firstLetterCount);

                    Interlocked.Increment(ref lastLetterCount);

                    if (word.Length >= 1)
                    {
                        int middlePos = word.Length / 2;        
                        Interlocked.Increment(ref middleLetterCount);
                    }
                }
            });

            Console.WriteLine($"Совпадения по позициям:");
            Console.WriteLine($"Первые буквы: {firstLetterCount}");
            Console.WriteLine($"Последние буквы: {lastLetterCount}");
            Console.WriteLine($"Средние буквы: {middleLetterCount}");

            string mostFrequent;
            if (firstLetterCount >= lastLetterCount && firstLetterCount >= middleLetterCount)
                mostFrequent = "первые буквы";
            else if (lastLetterCount >= firstLetterCount && lastLetterCount >= middleLetterCount)
                mostFrequent = "последние буквы";
            else
                mostFrequent = "средние буквы";

            Console.WriteLine($"Наиболее частые совпадения: {mostFrequent}");
        }

        static void CalculateStatistics(int[] numbers)
        {
            double average = 0;
            double median = 0;
            int max = int.MinValue;
            int min = int.MaxValue;
            object lockObj = new object();

            Parallel.Invoke(
                () =>
                {
                    double sum = 0;
                    Parallel.ForEach(numbers, () => 0.0, (num, loopState, localSum) =>
                    {
                        return localSum + num;
                    },
                    localSum => { lock (lockObj) { sum += localSum; } });
                    average = sum / numbers.Length;
                },
                () =>
                {
                    int[] sorted = new int[numbers.Length];
                    Array.Copy(numbers, sorted, numbers.Length);
                    Array.Sort(sorted);

                    if (sorted.Length % 2 == 0)
                        median = (sorted[sorted.Length / 2 - 1] + sorted[sorted.Length / 2]) / 2.0;
                    else
                        median = sorted[sorted.Length / 2];
                },
                () =>
                {
                    Parallel.ForEach(numbers, num =>
                    {
                        int currentMax = max;
                        while (num > currentMax)
                        {
                            if (Interlocked.CompareExchange(ref max, num, currentMax) == currentMax)
                                break;
                            currentMax = max;
                        }
                    });
                },
                () =>
                {
                    Parallel.ForEach(numbers, num =>
                    {
                        int currentMin = min;
                        while (num < currentMin)
                        {
                            if (Interlocked.CompareExchange(ref min, num, currentMin) == currentMin)
                                break;
                            currentMin = min;
                        }
                    });
                }
            );

            Console.WriteLine($"Среднее значение: {average:F2}");
            Console.WriteLine($"Медиана: {median:F2}");
            Console.WriteLine($"Максимальное значение: {max}");
            Console.WriteLine($"Минимальное значение: {min}");
        }

        static void ComparePerformance()
        {
            int[] largeArray = GenerateLargeArray(1000000);

            var stopwatch = Stopwatch.StartNew();
            SequentialStatistics(largeArray);
            stopwatch.Stop();
            long sequentialTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            CalculateStatistics(largeArray);
            stopwatch.Stop();
            long parallelTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Время последовательной обработки: {sequentialTime} мс");
            Console.WriteLine($"Время параллельной обработки: {parallelTime} мс");
            Console.WriteLine($"Ускорение: {(double)sequentialTime / parallelTime:F2}x");
        }

        static void SequentialStatistics(int[] numbers)
        {
            double average = numbers.Average();
            double median = CalculateSequentialMedian(numbers);
            int max = numbers.Max();
            int min = numbers.Min();
        }

        static double CalculateSequentialMedian(int[] numbers)
        {
            int[] sorted = new int[numbers.Length];
            Array.Copy(numbers, sorted, numbers.Length);
            Array.Sort(sorted);

            if (sorted.Length % 2 == 0)
                return (sorted[sorted.Length / 2 - 1] + sorted[sorted.Length / 2]) / 2.0;
            else
                return sorted[sorted.Length / 2];
        }

        static int[] GenerateLargeArray(int size)
        {
            Random rand = new Random();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = rand.Next(1, 1000);
            }
            return array;
        }
    }
    
}
