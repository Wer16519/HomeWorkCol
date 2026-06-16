namespace t3
{
    class Program
    {
        static void Main()
        {
            Random rnd = new Random();
            int n;

            do
            {
                Console.Write("Введите размер массива (от 5 до 100): ");
            } while (!int.TryParse(Console.ReadLine(), out n) || n < 5 || n > 100);

            int min, max;
            Console.Write("Введите минимальное значение диапазона: ");
            min = int.Parse(Console.ReadLine());
            Console.Write("Введите максимальное значение диапазона: ");
            max = int.Parse(Console.ReadLine());

            int[] arr = new int[n];
            for (int i = 0; i < n; i++)
                arr[i] = rnd.Next(min, max + 1);

            Console.WriteLine("\nИсходный массив:");
            for (int i = 0; i < n; i++)
                Console.Write(arr[i] + " ");

            double avg = 0;
            for (int i = 0; i < n; i++)
                avg += arr[i];
            avg /= n;

            Console.WriteLine($"\n\nСреднее арифметическое: {avg:F2}");

            Console.WriteLine("\nЧисла кратные 4 и больше среднего:");
            bool found = false;
            for (int i = 0; i < n; i++)
            {
                if (arr[i] % 4 == 0 && arr[i] > avg)
                {
                    Console.Write(arr[i] + " ");
                    found = true;
                }
            }

            if (!found)
                Console.WriteLine("Таких чисел нет.");

            Console.ReadKey();
        }
    }
}