using System;
using System.Collections.Generic;

namespace Task49_MathLibrary
{
    public class MathLib
    {
        public static int GCD(int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
                throw new ArgumentException("Массив не может быть пустым");

            int result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                result = GCD(result, numbers[i]);
            }
            return result;
        }

        private static int GCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static List<int> PrimeFactors(int n)
        {
            if (n <= 0)
                throw new ArgumentException("Число должно быть натуральным (больше 0)");

            List<int> factors = new List<int>();
            int number = n;

            while (number % 2 == 0)
            {
                factors.Add(2);
                number /= 2;
            }

            for (int i = 3; i <= Math.Sqrt(number); i += 2)
            {
                while (number % i == 0)
                {
                    factors.Add(i);
                    number /= i;
                }
            }

            if (number > 1)
            {
                factors.Add(number);
            }

            return factors;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Библиотека MathLib ===\n");

            int[] numbers = { 48, 72, 120, 96 };
            Console.WriteLine($"Массив: [{string.Join(", ", numbers)}]");
            int gcd = MathLib.GCD(numbers);
            Console.WriteLine($"НОД всех элементов: {gcd}");

            Console.WriteLine("\n--- Разложение на простые множители ---");

            int[] testNumbers = { 12, 56, 97, 100, 256 };

            foreach (int num in testNumbers)
            {
                var factors = MathLib.PrimeFactors(num);
                Console.WriteLine($"{num} = {string.Join(" * ", factors)}");
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}