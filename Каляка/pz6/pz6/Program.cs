using pz6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        StudentService studentService = new StudentService();

        // Получаем всех студентов из API
        Console.WriteLine("Загрузка данных...");
        var students = await studentService.GetAllStudentsAsync();

        if (students.Count == 0)
        {
            Console.WriteLine("Не удалось загрузить данные или список пуст.");
            return;
        }

        // Вывод всех студентов
        Console.WriteLine($"\n=== ВСЕ СТУДЕНТЫ (всего: {students.Count}) ===\n");
        foreach (var stud in students.Take(5)) // Показываем только первых 5 для компактности
        {
            Console.WriteLine($"Id: {stud.Id}");
            Console.WriteLine($"Имя: {stud.UserName}");
            Console.WriteLine($"Email: {stud.Email}");
            Console.WriteLine($"Группа: {stud.GroupName}");
            Console.WriteLine(new string('-', 30));
        }

        if (students.Count > 5)
            Console.WriteLine($"... и еще {students.Count - 5} студентов\n");

        // Задание 1: Фильтрация
        Console.WriteLine("=== ЗАДАНИЕ 1: ФИЛЬТРАЦИЯ ===\n");

        Console.WriteLine("1.1. Студенты с Email на gmail.com:");
        var gmailStudents = students.Where(s => s.Email != null && s.Email.Contains("gmail.com"));
        foreach (var student in gmailStudents)
        {
            Console.WriteLine($"  - {student.UserName} ({student.Email})");
        }

        Console.WriteLine("\n1.2. Студенты группы ИТ-25:");
        var it25Students = students.Where(s => s.GroupName == "ИТ-25");
        foreach (var student in it25Students)
        {
            Console.WriteLine($"  - {student.UserName} ({student.Email})");
        }

        // Задание 2: Сортировка
        Console.WriteLine("\n=== ЗАДАНИЕ 2: СОРТИРОВКА ===\n");

        Console.WriteLine("2.1. По имени пользователя (А-Я):");
        var sortedByName = students.OrderBy(s => s.UserName);
        foreach (var student in sortedByName.Take(5))
        {
            Console.WriteLine($"  - {student.UserName}");
        }

        Console.WriteLine("\n2.2. По длине имени пользователя:");
        var sortedByNameLength = students.OrderBy(s => s.UserName.Length);
        foreach (var student in sortedByNameLength.Take(5))
        {
            Console.WriteLine($"  - {student.UserName} (длина: {student.UserName.Length})");
        }

        Console.WriteLine("\n2.3. По группе и имени пользователя:");
        var sortedByGroupAndName = students.OrderBy(s => s.GroupName).ThenBy(s => s.UserName);
        foreach (var student in sortedByGroupAndName.Take(5))
        {
            Console.WriteLine($"  - [{student.GroupName}] {student.UserName}");
        }

        // Задание 3: Проекция
        Console.WriteLine("\n=== ЗАДАНИЕ 3: ПРОЕКЦИЯ ===\n");

        Console.WriteLine("3.1. Имя пользователя и Email:");
        var nameAndEmail = students.Select(s => new { s.UserName, s.Email });
        foreach (var item in nameAndEmail.Take(5))
        {
            Console.WriteLine($"  - {item.UserName}: {item.Email}");
        }

        Console.WriteLine("\n3.2. Имя пользователя и домен Email:");
        var nameAndDomain = students
            .Where(s => !string.IsNullOrEmpty(s.Email) && s.Email.Contains('@'))
            .Select(s => new
            {
                s.UserName,
                Domain = s.Email.Substring(s.Email.IndexOf('@') + 1)
            });
        foreach (var item in nameAndDomain.Take(5))
        {
            Console.WriteLine($"  - {item.UserName}: {item.Domain}");
        }

        // Задание 4: Агрегатные функции
        Console.WriteLine("\n=== ЗАДАНИЕ 4: АГРЕГАТНЫЕ ФУНКЦИИ ===\n");

        Console.WriteLine($"4.1. Общее количество студентов: {students.Count}");

        int it35Count = students.Count(s => s.GroupName == "ИТ-35");
        Console.WriteLine($"4.2. Количество студентов в группе ИТ-35: {it35Count}");

        // Задание 5: Группировка
        Console.WriteLine("\n=== ЗАДАНИЕ 5: ГРУППИРОВКА ПО ГРУППАМ ===\n");

        var groups = students
            .Where(s => !string.IsNullOrEmpty(s.GroupName))
            .GroupBy(s => s.GroupName)
            .OrderBy(g => g.Key);

        foreach (var group in groups)
        {
            Console.WriteLine($"\nГруппа: {group.Key}");
            Console.WriteLine($"Количество студентов: {group.Count()}");

            Console.WriteLine("Первые 3 студента:");
            var firstThree = group.OrderBy(s => s.UserName).Take(3);
            foreach (var student in firstThree)
            {
                Console.WriteLine($"  - {student.UserName} ({student.Email})");
            }
        }

        // Задание 6: Работа с элементами
        Console.WriteLine("\n=== ЗАДАНИЕ 6: ПОИСК И ПРОВЕРКИ ===\n");

        Console.WriteLine("6.1. Поиск студента с Id = 376:");
        var studentById = students.FirstOrDefault(s => s.Id == 376);
        if (studentById != null)
        {
            Console.WriteLine($"  Найден: {studentById.UserName} ({studentById.Email})");
        }
        else
        {
            Console.WriteLine("  Студент с Id 376 не найден");
        }

        Console.WriteLine("\n6.2. Поиск студента с email 'admin@example.com':");
        var studentByEmail = students.FirstOrDefault(s => s.Email == "admin@example.com");
        if (studentByEmail != null)
        {
            Console.WriteLine($"  Найден: {studentByEmail.UserName} (Id: {studentByEmail.Id})");
        }
        else
        {
            Console.WriteLine("  Студент с email 'admin@example.com' не найден");
        }

        Console.WriteLine("\n6.3. Проверка наличия студентов в группе 'АДМИН':");
        bool hasAdminGroup = students.Any(s => s.GroupName == "АДМИН");
        Console.WriteLine($"  {(hasAdminGroup ? "Есть студенты" : "Нет студентов")} в группе АДМИН");

        // Дополнительная статистика
        Console.WriteLine("\n=== ДОПОЛНИТЕЛЬНАЯ СТАТИСТИКА ===\n");

        // Уникальные группы
        var uniqueGroups = students.Select(s => s.GroupName).Distinct().Where(g => !string.IsNullOrEmpty(g));
        Console.WriteLine($"Всего групп: {uniqueGroups.Count()}");
        Console.WriteLine($"Группы: {string.Join(", ", uniqueGroups)}");

        // Самые популярные почтовые домены
        var emailDomains = students
            .Where(s => !string.IsNullOrEmpty(s.Email) && s.Email.Contains('@'))
            .Select(s => s.Email.Substring(s.Email.IndexOf('@') + 1))
            .GroupBy(d => d)
            .OrderByDescending(g => g.Count())
            .Take(3);

        Console.WriteLine("\nТоп-3 почтовых домена:");
        foreach (var domain in emailDomains)
        {
            Console.WriteLine($"  - {domain.Key}: {domain.Count()} студентов");
        }
    }
}