using System;

namespace t14
{
    class Program
    {
        static void PrintArray(int[,] arr)
        {
            Console.WriteLine("Исходный массив:");
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                    Console.Write($"{arr[i, j],5}");
                Console.WriteLine();
            }
        }

        static double AverageArray(int[,] arr)
        {
            double sum = 0;
            int count = arr.GetLength(0) * arr.GetLength(1);

            for (int i = 0; i < arr.GetLength(0); i++)
                for (int j = 0; j < arr.GetLength(1); j++)
                    sum += arr[i, j];

            double avg = sum / count;
            Console.WriteLine($"\nСреднее арифметическое: {avg:F2}");
            return avg;
        }

        static void Main()
        {
            Console.Write("Введите количество строк: ");
            int rows = int.Parse(Console.ReadLine());
            Console.Write("Введите количество столбцов: ");
            int cols = int.Parse(Console.ReadLine());

            int[,] arr = new int[rows, cols];
            Random rnd = new Random();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    arr[i, j] = rnd.Next(-50, 51);

            PrintArray(arr);
            AverageArray(arr);

            Console.ReadKey();
        }
    }
}