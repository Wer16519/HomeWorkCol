using System;
using MathLib;

namespace t23_prog
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Сложение двух чисел ===");
            Console.Write("Введите первое число: ");
            double a = double.Parse(Console.ReadLine());
            Console.Write("Введите второе число: ");
            double b = double.Parse(Console.ReadLine());

            double sum = MathOperations.Add(a, b);
            Console.WriteLine($"Результат: {a} + {b} = {sum}");

            Console.WriteLine("\n=== Умножение трёх чисел ===");
            Console.Write("Введите первое число: ");
            double x = double.Parse(Console.ReadLine());
            Console.Write("Введите второе число: ");
            double y = double.Parse(Console.ReadLine());
            Console.Write("Введите третье число: ");
            double z = double.Parse(Console.ReadLine());

            double product = MathOperations.MultiplyThree(x, y, z);
            Console.WriteLine($"Результат: {x} * {y} * {z} = {product}");

            Console.ReadKey();
        }
    }
}