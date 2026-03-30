using System;

namespace DeliveryCost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Калькулятор доставки");

            bool repeat = true;

            while (repeat)
            {
                double weight = GetNumber("Вес (кг): ");
                double distance = GetNumber("Расстояние (км): ");
                bool express = GetDelivery();

                double price = Calculate(weight, distance, express);

                Console.WriteLine($"Стоимость: {Math.Round(price, 2)} руб.");

                Console.Write("\nНовый расчет? (да/нет): ");
                string answer = Console.ReadLine()?.Trim().ToLower();
                if (answer != "да" && answer != "yes" && answer != "д")
                {
                    repeat = false;
                }
                Console.WriteLine();
            }

            Console.WriteLine("Спасибо!");
        }

        static double GetNumber(string prompt)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, out value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Нужно положительное число.");
            }
        }

        static bool GetDelivery()
        {
            while (true)
            {
                Console.WriteLine("Тип доставки: ");
                Console.WriteLine("1. Обычная");
                Console.WriteLine("2. Экспресс");
                Console.Write("Выбор: ");
                string input = Console.ReadLine();

                if (input == "1")
                    return false;
                if (input == "2")
                    return true;

                Console.WriteLine("Ошибка: введите 1 или 2");
            }
        }

        static double Calculate(double weight, double distance, bool express)
        {
            double total = weight * distance * 5;
            if (express)
            {
                total *= 1.5;
            }
            return total;
        }
    }
}