using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevinPZ8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Калькулятор суточной нормы калорий");

            bool repeat = true;

            while (repeat)
            {
                double weight = GetNumber("Ваш вес (кг): ");
                double height = GetNumber("Ваш рост (см): ");
                int age = (int)GetNumber("Ваш возраст (лет): ");
                bool isActive = GetActivityLevel();

                double calories = Ras(weight, height, age, isActive);

                Console.WriteLine($"Ваша суточная норма калорий: {Math.Round(calories, 0)} ккал");

                Console.Write("\nНовый расчёт? (да/нет): ");
                string answer = Console.ReadLine()?.Trim().ToLower();
                if (answer != "да" && answer != "yes" && answer != "д")
                {
                    repeat = false;
                }
                Console.WriteLine();
            }

            Console.WriteLine("Будьте здоровы!");
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

        static bool GetActivityLevel()
        {
            while (true)
            {
                Console.WriteLine("Уровень физической активности:");
                Console.WriteLine("1. Сидячий образ жизни");
                Console.WriteLine("2. Умеренная активность (1-3 тренировки в неделю)");
                Console.Write("Выбор: ");
                string input = Console.ReadLine();

                if (input == "1")
                    return false;
                if (input == "2")
                    return true;

                Console.WriteLine("Ошибка: введите 1 или 2");
            }
        }

        static double Ras(double weight, double height, int age, bool isActive)
        {
            double bmr = 10 * weight + 6.25 * height - 5 * age + 5;
            if (isActive)
            {
                bmr *= 1.55; // умеренная активность
            }
            else
            {
                bmr *= 1.2; // сидячий образ жизни
            }
            return bmr;
        }
    }
}