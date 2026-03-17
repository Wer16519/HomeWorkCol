namespace pz6
{
   public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Уровень 1: Базовый генератор");
            var simpleGenerator = new SimpleDataGenerator();
            List<Person> simplePeople = simpleGenerator.GeneratePersons(100);
            simpleGenerator.PrintToConsole(simplePeople);
            Console.WriteLine($"Всего сгенерировано: {simplePeople.Count} записей.");

            Console.WriteLine("\nУровень 2: Генератор с параметрами и дефектами");
            var settings = new GeneratorSettings
            {
                MinAge = 20,
                MaxAge = 50,
                MinSalary = 30000,
                MaxSalary = 150000,
                UnemploymentRate = 0.1
            };

            var advancedGenerator = new AdvancedDataGenerator(settings);
            var people = advancedGenerator.GeneratePersons(50);

            Console.WriteLine("Первые 10 записей (без дефектов):");
            advancedGenerator.PrintToConsole(people);

            Console.WriteLine("\nВнедрение пропусков (вероятность 15%)...");
            advancedGenerator.InjectNullValues(people, 0.15);
            Console.WriteLine("Первые 10 записей (с пропусками):");
            advancedGenerator.PrintToConsole(people);

            Console.WriteLine("\nВнедрение дубликатов (5 копий)...");
            advancedGenerator.InjectDuplicates(people, 5);
            Console.WriteLine($"Всего записей после дубликатов: {people.Count}");

            Console.WriteLine("\nВнедрение выбросов (2% аномальных зарплат)...");
            advancedGenerator.InjectOutliers(people);
            advancedGenerator.PrintToConsole(people);
        }
    }
    public class Person
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public int? Age { get; set; }
        public double? Salary { get; set; }
        public bool IsEmployed { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {FullName ?? "NULL"}, Age: {(Age.HasValue ? Age.Value.ToString() : "NULL")}, " +
                   $"Salary: {(Salary.HasValue ? Salary.Value.ToString("C") : "NULL")}, Employed: {IsEmployed}";
        }
    }

    public class SimpleDataGenerator
    {
        private Random _random = new Random();
        private string[] _firstNames = { "Алексей", "Мария", "Дмитрий", "Елена", "Иван", "Ольга" };
        private string[] _lastNames = { "Иванов", "Петров", "Сидоров", "Смирнов", "Кузнецова" };
        private int _nextId = 1;

        public List<Person> GeneratePersons(int count)
        {
            var persons = new List<Person>();
            for (int i = 0; i < count; i++)
            {
                var person = new Person
                {
                    Id = _nextId++,
                    FullName = $"{_firstNames[_random.Next(_firstNames.Length)]}_{_lastNames[_random.Next(_lastNames.Length)]}_{_random.Next(100, 1000)}",
                    Age = _random.Next(18, 66),
                    Salary = _random.NextDouble() * (200000 - 20000) + 20000,
                    IsEmployed = true
                };
                persons.Add(person);
            }
            return persons;
        }

        public void PrintToConsole(List<Person> data)
        {
            for (int i = 0; i < Math.Min(10, data.Count); i++)
            {
                Console.WriteLine(data[i]);
            }
        }
    }

    public class GeneratorSettings
    {
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 65;
        public double MinSalary { get; set; } = 20000;
        public double MaxSalary { get; set; } = 200000;
        public double UnemploymentRate { get; set; } = 0.0;
    }

    public class AdvancedDataGenerator
    {
        private Random _random = new Random();
        private GeneratorSettings _settings;
        private string[] _firstNames = { "Алексей", "Мария", "Дмитрий", "Елена", "Иван", "Ольга" };
        private string[] _lastNames = { "Иванов", "Петров", "Сидоров", "Смирнов", "Кузнецова" };
        private int _nextId = 1;

        public AdvancedDataGenerator(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public List<Person> GeneratePersons(int count)
        {
            var persons = new List<Person>();
            for (int i = 0; i < count; i++)
            {
                bool isEmployed = _random.NextDouble() > _settings.UnemploymentRate;
                double salary = 0;
                if (isEmployed)
                {
                    salary = _random.NextDouble() * (_settings.MaxSalary - _settings.MinSalary) + _settings.MinSalary;
                }

                var person = new Person
                {
                    Id = _nextId++,
                    FullName = $"{_firstNames[_random.Next(_firstNames.Length)]}_{_lastNames[_random.Next(_lastNames.Length)]}_{_random.Next(100, 1000)}",
                    Age = _random.Next(_settings.MinAge, _settings.MaxAge + 1),
                    Salary = salary,
                    IsEmployed = isEmployed
                };
                persons.Add(person);
            }
            return persons;
        }

        public void InjectNullValues(List<Person> data, double probability)
        {
            foreach (var person in data)
            {
                if (_random.NextDouble() < probability)
                    person.FullName = null;
                if (_random.NextDouble() < probability)
                    person.Age = null;
                if (_random.NextDouble() < probability)
                    person.Salary = null;
            }
        }

        public void InjectDuplicates(List<Person> data, int duplicateCount)
        {
            for (int i = 0; i < duplicateCount; i++)
            {
                if (data.Count > 0)
                {
                    var randomPerson = data[_random.Next(data.Count)];
                    var duplicate = new Person
                    {
                        Id = _nextId++,
                        FullName = randomPerson.FullName,
                        Age = randomPerson.Age,
                        Salary = randomPerson.Salary,
                        IsEmployed = randomPerson.IsEmployed
                    };
                    data.Add(duplicate);
                }
            }
        }

        public void InjectOutliers(List<Person> data)
        {
            int outlierCount = (int)(data.Count * 0.02);
            for (int i = 0; i < outlierCount; i++)
            {
                if (data.Count > 0)
                {
                    var person = data[_random.Next(data.Count)];
                    if (_random.NextDouble() < 0.5)
                        person.Salary = person.Salary * 100;
                    else
                        person.Salary = 1;
                }
            }
        }

        public void PrintToConsole(List<Person> data)
        {
            for (int i = 0; i < Math.Min(10, data.Count); i++)
            {
                Console.WriteLine(data[i]);
            }
        }
    }
}