using System;

namespace t9
{
    class Program
    {
        static void Main()
        {
            try
            {
                Console.Write("Введите значение x: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    throw new ArgumentNullException("Ввод не может быть пустым!");

                double x = double.Parse(input);

                double result = Math.Sin(Math.PI * x);

                if (double.IsNaN(result) || double.IsInfinity(result))
                    throw new ArithmeticException("Результат не является числом!");

                Console.WriteLine($"f(x) = sin(π * {x}) = {result:F6}");
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Ошибка формата: " + ex.Message);
            }
            catch (OverflowException ex)
            {
                Console.WriteLine("Переполнение: " + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            catch (ArithmeticException ex)
            {
                Console.WriteLine("Арифметическая ошибка: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Неизвестная ошибка: " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}