using System;

namespace t20
{
    struct Employees
    {
        public string LastName;
        public string FirstName;
        public DateTime BirthDate;
        public string Position;
    }

    class Program
    {
        static void Main()
        {
            Console.Write("Введите количество сотрудников: ");
            int n = int.Parse(Console.ReadLine());

            Employees[] employees = new Employees[n];

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"\n--- Сотрудник {i + 1} ---");

                Console.Write("Фамилия: ");
                employees[i].LastName = Console.ReadLine();

                Console.Write("Имя: ");
                employees[i].FirstName = Console.ReadLine();

                Console.Write("Дата рождения (дд.мм.гггг): ");
                employees[i].BirthDate = DateTime.Parse(Console.ReadLine());

                Console.Write("Должность: ");
                employees[i].Position = Console.ReadLine();
            }

            Console.WriteLine("\n========== Список сотрудников ==========");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"\nСотрудник {i + 1}:");
                Console.WriteLine($"  Фамилия:        {employees[i].LastName}");
                Console.WriteLine($"  Имя:            {employees[i].FirstName}");
                Console.WriteLine($"  Дата рождения:  {employees[i].BirthDate:dd.MM.yyyy}");
                Console.WriteLine($"  Должность:      {employees[i].Position}");
            }

            Console.ReadKey();
        }
    }
}