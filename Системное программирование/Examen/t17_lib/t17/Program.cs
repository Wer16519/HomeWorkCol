using System;
using t17_lib;

namespace t17
{
    class Program
    {
        static void Main()
        {
            Console.Write("Введите первое число для сложения: ");
            double a = double.Parse(Console.ReadLine());
            Console.Write("Введите второе число для сложения: ");
            double b = double.Parse(Console.ReadLine());

            double sum = MathFunctions.Sum(a, b);
            Console.WriteLine($"Сумма: {a} + {b} = {sum}");

            Console.Write("\nВведите первое число для умножения: ");
            double x = double.Parse(Console.ReadLine());
            Console.Write("Введите второе число для умножения: ");
            double y = double.Parse(Console.ReadLine());
            Console.Write("Введите третье число для умножения: ");
            double z = double.Parse(Console.ReadLine());

            double mul = MathFunctions.Multiply(x, y, z);
            Console.WriteLine($"Произведение: {x} * {y} * {z} = {mul}");

            Console.ReadKey();
        }
    }
}