namespace Zad13
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Работа с двумерным массивом\n");

            int[,] array = new int[3, 4];
            Random rand = new Random();

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 4; j++)
                    array[i, j] = rand.Next(1, 10);

            PrintArray(array);
            double avg = CalculateAverage(array);
            Console.WriteLine($"\nСреднее арифметическое: {avg:F2}");

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static void PrintArray(int[,] array)
        {
            Console.WriteLine("Исходный массив:");
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                    Console.Write($"{array[i, j]} ");
                Console.WriteLine();
            }
        }

        static double CalculateAverage(int[,] array)
        {
            int sum = 0;
            int count = 0;

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    sum += array[i, j];
                    count++;
                }

            return (double)sum / count;
        }
    }
}