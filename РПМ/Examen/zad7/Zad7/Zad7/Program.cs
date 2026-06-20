using System;
using System.Collections.Generic;
using System.Linq;

namespace Task7_Console
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("АНАЛИЗ МАССИВА\n");

            try
            {
                int size = GetSize();
                int min = GetMin();
                int max = GetMax(min);
                int[] array = GenerateArray(size, min, max);

                Console.WriteLine("\nИсходный массив:");
                Console.WriteLine(string.Join(" ", array));

                double avg = array.Average();
                Console.WriteLine($"\nСреднее арифметическое: {avg:F2}");

                var result = array.Where(x => x % 4 == 0 && x > avg).ToList();
                Console.WriteLine("\nЧисла кратные 4 и больше среднего:");
                if (result.Count == 0)
                    Console.WriteLine("Таких чисел нет");
                else
                    Console.WriteLine(string.Join(" ", result));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static int GetSize()
        {
            int size;
            while (true)
            {
                Console.Write("Введите размер массива (5-100): ");
                if (int.TryParse(Console.ReadLine(), out size) && size >= 5 && size <= 100)
                    return size;
                Console.WriteLine("Ошибка! Размер от 5 до 100.");
            }
        }

        static int GetMin()
        {
            int min;
            while (true)
            {
                Console.Write("Введите минимальное значение: ");
                if (int.TryParse(Console.ReadLine(), out min))
                    return min;
                Console.WriteLine("Ошибка! Введите число.");
            }
        }

        static int GetMax(int min)
        {
            int max;
            while (true)
            {
                Console.Write("Введите максимальное значение: ");
                if (int.TryParse(Console.ReadLine(), out max) && max > min)
                    return max;
                Console.WriteLine($"Ошибка! Максимум должен быть больше {min}.");
            }
        }

        static int[] GenerateArray(int size, int min, int max)
        {
            Random rand = new Random();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
                array[i] = rand.Next(min, max + 1);
            return array;
        }
    }
}