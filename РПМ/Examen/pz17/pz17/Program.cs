struct Employee
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
        var list = new List<Employee>();

        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"\n--- Сотрудник {i + 1} ---");
            Employee emp;
            Console.Write("Фамилия: "); emp.LastName = Console.ReadLine();
            Console.Write("Имя: "); emp.FirstName = Console.ReadLine();
            Console.Write("Дата рождения (дд.мм.гггг): ");
            emp.BirthDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Должность: "); emp.Position = Console.ReadLine();
            list.Add(emp);
        }

        Console.WriteLine("\n=== Список сотрудников ===");
        foreach (var e in list)
            Console.WriteLine($"{e.LastName} {e.FirstName}, {e.BirthDate:dd.MM.yyyy}, {e.Position}");

        Console.ReadKey();
    }
}
