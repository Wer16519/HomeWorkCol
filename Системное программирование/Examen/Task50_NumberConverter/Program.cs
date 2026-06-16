using System;

namespace Task50_NumberConverter
{
    public static class NumberConverter
    {
        public static string ToBinary(int number)
        {
            return Convert.ToString(number, 2);
        }

        public static string ToOctal(int number)
        {
            return Convert.ToString(number, 8);
        }

        public static string ToHex(int number)
        {
            return Convert.ToString(number, 16).ToUpper();
        }

        public static int FromBinary(string number)
        {
            return Convert.ToInt32(number, 2);
        }

        public static int FromOctal(string number)
        {
            return Convert.ToInt32(number, 8);
        }

        public static int FromHex(string number)
        {
            return Convert.ToInt32(number, 16);
        }

        public static string ConvertBase(string number, int fromBase, int toBase)
        {
            int decimalValue = Convert.ToInt32(number, fromBase);
            return Convert.ToString(decimalValue, toBase).ToUpper();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Конвертер систем счисления ===\n");

            Console.WriteLine("Выберите операцию:");
            Console.WriteLine("1 - Перевод числа в разные системы");
            Console.WriteLine("2 - Перевод из системы в систему");
            Console.WriteLine("3 - Таблица перевода чисел");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ConvertNumber();
                    break;
                case "2":
                    ConvertBetweenBases();
                    break;
                case "3":
                    ShowTable();
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }

        static void ConvertNumber()
        {
            Console.Write("Введите число: ");
            int number = int.Parse(Console.ReadLine());

            Console.WriteLine($"\nДесятичная: {number}");
            Console.WriteLine($"Двоичная: {NumberConverter.ToBinary(number)}");
            Console.WriteLine($"Восьмеричная: {NumberConverter.ToOctal(number)}");
            Console.WriteLine($"Шестнадцатеричная: {NumberConverter.ToHex(number)}");
        }

        static void ConvertBetweenBases()
        {
            Console.WriteLine("Доступные системы: 2 (двоичная), 8 (восьмеричная), 10 (десятичная), 16 (шестнадцатеричная)");

            Console.Write("Введите число: ");
            string number = Console.ReadLine();

            Console.Write("Из какой системы (2/8/10/16): ");
            int fromBase = int.Parse(Console.ReadLine());

            Console.Write("В какую систему (2/8/10/16): ");
            int toBase = int.Parse(Console.ReadLine());

            try
            {
                string result = NumberConverter.ConvertBase(number, fromBase, toBase);
                Console.WriteLine($"\nРезультат: {number} (из {fromBase}) = {result} (в {toBase})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ShowTable()
        {
            Console.WriteLine("Таблица перевода чисел (десятичная → двоичная/восьмеричная/шестнадцатеричная):\n");

            int[] numbers = { 10, 20, 30, 40, 50, 100, 255, 512, 1000, 1024 };

            Console.WriteLine($"{"Десятичная",12} {"Двоичная",20} {"Восьмеричная",14} {"Шестнадцатеричная",18}");
            Console.WriteLine(new string('-', 66));

            foreach (int num in numbers)
            {
                Console.WriteLine($"{num,12} {NumberConverter.ToBinary(num),20} {NumberConverter.ToOctal(num),14} {NumberConverter.ToHex(num),18}");
            }
        }
    }
}