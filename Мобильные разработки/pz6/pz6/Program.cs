using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

public class Student
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string GroupName { get; set; }
}

public class StudentService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://vic-site.ru/api";

    public StudentService()
    {
        _httpClient = new HttpClient();
    }

    // 1. Получить всех студентов 
    public async Task<List<Student>> GetAllStudentsAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/students");
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            List<Student> students = JsonConvert.DeserializeObject<List<Student>>(content);
            return students;
        }
        return new List<Student>(); // Возвращаем пустой список в случае ошибки 
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        StudentService studentService = new StudentService();
        // Получаем всех студентов из API(надо вывести) 
        var students = await studentService.GetAllStudentsAsync();

        if (students.Count == 0)
        {
            Console.WriteLine("Не удалось загрузить данные или список пуст.");
            return;
        }
        else
        {
            foreach (var stud in students)
            {
                //Console.WriteLine($"{stud.Id}: {stud.UserName}, " + 
                //          $"Email: {stud.Email}, Группа: {stud.GroupName}"); 
                Console.WriteLine("Id: " + stud.Id);
                Console.WriteLine("Имя: " + stud.UserName);
                Console.WriteLine("Email: " + stud.Email);
                Console.WriteLine("Группа: " + stud.GroupName + Environment.NewLine);
            }
        }

        // Задание 1: Фильтрация 
        Console.WriteLine("=== Задание 1: Фильтрация ===");
        Console.WriteLine("\n1.1. Студенты с Email на gmail.com:");
        // Ваш код здесь с использованием .Where(...) 

        var gmailStudents = students.Where(s => s.Email.Contains("gmail.com"));
        foreach (var s in gmailStudents)
            Console.WriteLine($"{s.UserName}: {s.Email}");

        Console.WriteLine("\n1.2. Студенты группы ИТ-25:");
        // Ваш код здесь с использованием .Where(...) 

        var it25Students = students.Where(s => s.GroupName == "ИТ-25");
        foreach (var s in it25Students)
            Console.WriteLine($"{s.UserName}: {s.Email}");

        // Задание 2: Сортировка 
        Console.WriteLine("\n=== Задание 2: Сортировка ===");

        Console.WriteLine("\n2.1. По имени пользователя (А-Я):");

        var sortedByName = students.OrderBy(s => s.UserName);
        foreach (var s in sortedByName)
            Console.WriteLine($"{s.UserName}");

        Console.WriteLine("\n2.2. По длине имени пользователя:");

        var sortedByNameLength = students.OrderBy(s => s.UserName.Length);
        foreach (var s in sortedByNameLength)
            Console.WriteLine(s.UserName);

        Console.WriteLine("\n2.3. По группе и имени пользователя:");

        var sortedByGroupAndName = students.OrderBy(s => s.GroupName).ThenBy(s => s.UserName);
        foreach (var s in sortedByGroupAndName)
            Console.WriteLine($"{s.GroupName}: {s.UserName}");

        // Задание 3: Проекция 
        Console.WriteLine("\n=== Задание 3: Проекция ===");

        Console.WriteLine("\n3.1. Имя пользователя и Email:");

        var nameAndEmail = students.Select(s => new { s.UserName, s.Email });
        foreach (var item in nameAndEmail)
            Console.WriteLine($"{item.UserName}: {item.Email}");

        Console.WriteLine("\n3.2. Имя пользователя и домен Email:");//Имя и всё что после '@'

        var nameAndDomain = students.Select(s => new { s.UserName, Domain = s.Email.Substring(s.Email.IndexOf('@') + 1)});
        foreach (var item in nameAndDomain)
            Console.WriteLine($"{item.UserName}: {item.Domain}");

        // Задание 4: Агрегатные функции 
        Console.WriteLine("\n=== Задание 4: Агрегатные функции ===");

        Console.WriteLine($"\n4.1. Общее количество студентов:");

        Console.WriteLine(students.Count());

        Console.WriteLine($"\n4.2. Количество студентов в группе ИТ-35:");

        Console.WriteLine(students.Count(s => s.GroupName == "ИТ-35"));

        // Задание 5: Группировка GroupBy 
        Console.WriteLine("\n=== Задание 5: Группировка по группам ===");

        var groups = students.GroupBy(s => s.GroupName);

        foreach (var group in groups)
        {
            Console.WriteLine($"\nГруппа: {group.Key}");
            Console.WriteLine($"Количество студентов: {group.Count()}");
            Console.WriteLine("Первые 3 студента:");

            var firstThree = group.OrderBy(s => s.UserName).Take(3);
            foreach (var student in firstThree)
            {
                Console.WriteLine($"{student.UserName} ({student.Email})");
            }
        }

        //тут какойто LINQ в переменную groups а потом выводим в foreach 
        //var groups = 

        //foreach (var group in groups) 
        //{ 
        //    Console.WriteLine($"\nГруппа: {group.Key}"); 
        //    Console.WriteLine($"Количество студентов: {group.Count()}"); 

        //    Console.WriteLine("Первые 3 студента:"); 
        //    var firstThree = group.Take(3); 
        //    foreach (var student in firstThree) 
        //    { 
        //        Console.WriteLine($"  - {student.UserName} ({student.Email})"); 
        //    } 
        //} 

        // Задание 6: Работа с элементами 
        Console.WriteLine("\n=== Задание 6: Поиск и проверки ===");

        Console.WriteLine("\n6.1. Поиск студента с Id = 23:");

        var studentById = students.FirstOrDefault(s => s.Id == 23);
        if (studentById != null)
            Console.WriteLine(studentById.UserName);
        else
            Console.WriteLine("Студент не найден");

        Console.WriteLine("\n6.2. Поиск студента с email 'admin@example.com':");

        var studentByEmail = students.FirstOrDefault(s => s.Email == "admin@example.com");
        if (studentByEmail != null)
            Console.WriteLine(studentByEmail.UserName);
        else
            Console.WriteLine("Студент не найден");

        Console.WriteLine("\n6.3. Проверка наличия студентов в группе 'АДМИН':");

        bool hasAdminGroup = students.Any(s => s.GroupName == "АДМИН");

        Console.WriteLine(hasAdminGroup ? "Студент есть" : "Нет студента");
    }
}