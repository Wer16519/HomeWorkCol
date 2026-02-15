// Программа для практической работы по LINQ
using System;
using System.Collections.Generic;
using System.Linq;

// Простой класс для представления студента
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Faculty { get; set; }
    public double AverageGrade { get; set; }
    public bool IsOnBudget { get; set; } // Учатся на бюджете?
}

class Program
{
    static void Main(string[] args)
    {
        // Создаем и заполняем список студентов
        List<Student> students = new List<Student>
        {
            new Student { Id = 1, Name = "Анна Иванова", Age = 20, Faculty = "ФИИТ", AverageGrade = 4.5, IsOnBudget = true },
            new Student { Id = 2, Name = "Иван Петров", Age = 22, Faculty = "ИВТ", AverageGrade = 3.9, IsOnBudget = false },
            new Student { Id = 3, Name = "Мария Сидорова", Age = 19, Faculty = "ФИИТ", AverageGrade = 4.8, IsOnBudget = true },
            new Student { Id = 4, Name = "Петр Кузнецов", Age = 21, Faculty = "ИВТ", AverageGrade = 3.2, IsOnBudget = true },
            new Student { Id = 5, Name = "Елена Васильева", Age = 23, Faculty = "ПМИ", AverageGrade = 4.1, IsOnBudget = false },
            new Student { Id = 6, Name = "Алексей Козлов", Age = 20, Faculty = "ПМИ", AverageGrade = 4.9, IsOnBudget = true },
            new Student { Id = 7, Name = "Светлана Николаева", Age = 22, Faculty = "ФИИТ", AverageGrade = 3.7, IsOnBudget = false },
            new Student { Id = 8, Name = "Дмитрий Орлов", Age = 19, Faculty = "ИВТ", AverageGrade = 4.3, IsOnBudget = true }
        };

        // Ваши решения заданий будут вставляться сюда, в блок TODO 

        // TODO: Раскомментируйте по очереди вызовы методов для проверки своих решений
        Task1_BasicOperations(students);
        Task2_SortingAndAggregation(students);
        Task3_Grouping(students);
        Task4_Advanced(students);

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadLine();
    }

    // Задание 1: Базовые операции
    static void Task1_BasicOperations(List<Student> students)
    {
        Console.WriteLine("Задание 1: Базовые операции");

        // а) Имена студентов со средним баллом >= 4.0
        var goodStudents = students
            .Where(s => s.AverageGrade >= 4.0)
            .Select(s => s.Name);
        Console.WriteLine("а) Студенты с баллом >= 4.0:");
        foreach (var name in goodStudents)
        {
            Console.WriteLine($"{name}");
        }

        // б) Имена и возраст студентов факультета "ФИИТ"
        var fiitStudents = students
            .Where(s => s.Faculty == "ФИИТ")
            .Select(s => new { s.Name, s.Age });
        Console.WriteLine("\nб) Студенты факультета ФИИТ (Имя, Возраст):");
        foreach (var student in fiitStudents)
        {
            Console.WriteLine($"{student.Name}, {student.Age} лет");
        }

        // в) Количество бюджетников
        int budgetCount = students.Count(s => s.IsOnBudget);
        Console.WriteLine($"\nв) Количество студентов на бюджете: {budgetCount}");
    }

    // Задание 2: Сортировка и агрегация
    static void Task2_SortingAndAggregation(List<Student> students)
    {
        Console.WriteLine("Задание 2: Сортировка и агрегация");

        // а) Сортировка по убыванию среднего балла
        var sortedByGrade = students
            .OrderByDescending(s => s.AverageGrade)
            .Select(s => new { s.Name, s.AverageGrade });
        Console.WriteLine("а) Студенты, отсортированные по убыванию балла:");
        foreach (var student in sortedByGrade)
        {
            Console.WriteLine($"{student.Name} - {student.AverageGrade}");
        }

        // б) Средний возраст всех студентов
        double averageAge = students.Average(s => s.Age);
        Console.WriteLine($"\nб) Средний возраст студентов: {averageAge:F2} лет");

        // в) Самый высокий средний балл
        double maxGrade = students.Max(s => s.AverageGrade);
        Console.WriteLine($"\nв) Максимальный средний балл: {maxGrade}");

        // г) Количество студентов на каждом факультете
        var facultyCounts = students
            .GroupBy(s => s.Faculty)
            .Select(g => new { Faculty = g.Key, Count = g.Count() });
        Console.WriteLine("\nг) Количество студентов по факультетам:");
        foreach (var item in facultyCounts)
        {
            Console.WriteLine($"{item.Faculty}: {item.Count} чел.");
        }
    }

    // Задание 3: Группировка
    static void Task3_Grouping(List<Student> students)
    {
        Console.WriteLine("Задание 3: Группировка");

        // а) Группировка студентов по факультетам с выводом списка имен
        var groups = students.GroupBy(s => s.Faculty);
        Console.WriteLine("а) Студенты по факультетам:");
        foreach (var group in groups)
        {
            Console.WriteLine($"Факультет: {group.Key}");
            foreach (var student in group)
            {
                Console.WriteLine($"{student.Name}");
            }
        }

        // б) Средний балл по каждому факультету
        var averageGradesByFaculty = students
            .GroupBy(s => s.Faculty)
            .Select(g => new { Faculty = g.Key, AvgGrade = g.Average(s => s.AverageGrade) });
        Console.WriteLine("\nб) Средний балл по факультетам:");
        foreach (var item in averageGradesByFaculty)
        {
            Console.WriteLine($"{item.Faculty}: {item.AvgGrade:F2}");
        }
    }

    // Задание 4: Продвинутые операции
    static void Task4_Advanced(List<Student> students)
    {
        Console.WriteLine("Задание 4: Продвинутые операции");

        // а) Самый старший бюджетник
        var oldestBudgetStudent = students
            .Where(s => s.IsOnBudget)
            .OrderByDescending(s => s.Age)
            .First();
        Console.WriteLine("а) Самый старший студент на бюджете:");
        Console.WriteLine($"{oldestBudgetStudent.Name}, факультет {oldestBudgetStudent.Faculty}, возраст {oldestBudgetStudent.Age}");

        // б) Первые три студента факультета ИВТ с самым высоким баллом, отсортированы по имени
        var topIvtStudents = students
            .Where(s => s.Faculty == "ИВТ")
            .OrderByDescending(s => s.AverageGrade)
            .ThenBy(s => s.Name)
            .Take(3)
            .Select(s => new { s.Name, s.AverageGrade });
        Console.WriteLine("\nб) Топ-3 студентов ИВТ по баллу (с сортировкой по имени):");
        foreach (var student in topIvtStudents)
        {
            Console.WriteLine($"{student.Name} - {student.AverageGrade}");
        }

        // в) Есть ли студент на ПМИ с баллом выше 4.5
        bool anyPmiHighGrade = students.Any(s => s.Faculty == "ПМИ" && s.AverageGrade > 4.5);
        Console.WriteLine($"\nв) Наличие студента на ПМИ с баллом > 4.5: {anyPmiHighGrade}");

        // г) Студенты, чьё имя содержит букву 'а' (без учёта регистра)
        var namesWithA = students
            .Where(s => s.Name.Contains("а", StringComparison.OrdinalIgnoreCase) || s.Name.Contains("А", StringComparison.OrdinalIgnoreCase))
            .Select(s => s.Name);
        Console.WriteLine("\nг) Студенты с буквой 'а' в имени:");
        foreach (var name in namesWithA)
        {
            Console.WriteLine($"{name}");
        }
    }
}