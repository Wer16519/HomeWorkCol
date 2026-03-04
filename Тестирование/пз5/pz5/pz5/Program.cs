using pz5.Data;
using pz5.Models;
using pz5.Utils;

namespace pz5
{
    class Program
    {
        static DataSeeder seeder = new DataSeeder();

        static List<User> users;
        static List<Client> clients;
        static List<Courier> couriers;
        static List<Admin> admins;
        static List<MenuItem> menuItems;
        static List<Order> orders;
        static List<Payment> payments;

        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМА ДОСТАВКИ ЕДЫ ===\n");

            seeder.SeedFromPredefinedLists();
            seeder.EstablishRelationships();

            RefreshLocalCollections();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- ГЛАВНОЕ МЕНЮ ---");
                Console.WriteLine("1. Показать все данные");
                Console.WriteLine("2. Добавить новый объект");
                Console.WriteLine("3. Сгенерировать тестовые данные");
                Console.WriteLine("4. Поиск");
                Console.WriteLine("5. Показать статистику");
                Console.WriteLine("0. Выход");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAllData();
                        break;
                    case "2":
                        AddNewObject();
                        break;
                    case "3":
                        GenerateTestData();
                        break;
                    case "4":
                        SearchData();
                        break;
                    case "5":
                        ShowStatistics();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Выход из программы.");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void RefreshLocalCollections()
        {
            users = seeder.GetUsers();
            clients = seeder.GetClients();
            couriers = seeder.GetCouriers();
            admins = seeder.GetAdmins();
            menuItems = seeder.GetMenuItems();
            orders = seeder.GetOrders();
            payments = seeder.GetPayments();
        }

        static void ShowAllData()
        {
            Console.WriteLine("=== ВСЕ ДАННЫЕ ===");

            Console.WriteLine($"\n--- Пользователи ({users.Count}) ---");
            foreach (var item in users) Console.WriteLine($"  {item}");

            Console.WriteLine($"\n--- Клиенты ({clients.Count}) ---");
            foreach (var item in clients) Console.WriteLine($"  {item}");

            Console.WriteLine($"\n--- Курьеры ({couriers.Count}) ---");
            foreach (var item in couriers) Console.WriteLine($"  {item}");

            Console.WriteLine($"\n--- Администраторы ({admins.Count}) ---");
            foreach (var item in admins) Console.WriteLine($"  {item}");

            Console.WriteLine($"\n--- Меню ({menuItems.Count}) ---");
            foreach (var item in menuItems) Console.WriteLine($"  {item}");

            Console.WriteLine($"\n--- Заказы ({orders.Count}) ---");
            foreach (var order in orders)
            {
                Console.WriteLine($"  {order}");
                foreach (var orderItem in order.Items)
                {
                    Console.WriteLine($"    - {orderItem}");
                }
            }

            Console.WriteLine($"\n--- Платежи ({payments.Count}) ---");
            foreach (var item in payments) Console.WriteLine($"  {item}");
        }

        static void AddNewObject()
        {
            Console.WriteLine("=== ДОБАВЛЕНИЕ НОВОГО ОБЪЕКТА ===");
            Console.WriteLine("Выберите тип объекта:");
            Console.WriteLine("1. Блюдо (MenuItem)");
            Console.WriteLine("2. Клиент (Client)");
            Console.WriteLine("3. Заказ (Order)");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        var newItem = DataInput.InputMenuItem();
                        menuItems.Add(newItem);
                        Console.WriteLine("Блюдо успешно добавлено!");
                        break;
                    case "2":
                        var newClient = DataInput.InputClient();
                        clients.Add(newClient);
                        Console.WriteLine("Клиент успешно добавлен!");
                        break;
                    case "3":
                        Console.WriteLine("Функция добавления заказа пока не реализована напрямую.");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении: {ex.Message}");
            }
        }

        static void GenerateTestData()
        {
            Console.WriteLine("=== ГЕНЕРАЦИЯ ТЕСТОВЫХ ДАННЫХ ===");
            Console.WriteLine("1. Предопределенные данные (заново)");
            Console.WriteLine("2. Случайные данные (добавить к существующим)");
            Console.Write("Выбор: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                seeder.SeedFromPredefinedLists();
                seeder.EstablishRelationships();
                RefreshLocalCollections();
                Console.WriteLine("Данные перезаписаны предопределенным набором.");
            }
            else if (choice == "2")
            {
                Console.Write("Введите количество новых блюд для генерации: ");
                if (int.TryParse(Console.ReadLine(), out int count))
                {
                    seeder.SeedWithRandomData(count);
                    RefreshLocalCollections();
                }
                else
                {
                    Console.WriteLine("Некорректное число.");
                }
            }
        }

        static void SearchData()
        {
            Console.WriteLine("=== ПОИСК ===");
            Console.Write("Введите текст для поиска: ");
            string criteria = Console.ReadLine();

            var results = seeder.SearchByCriteria(criteria);

            Console.WriteLine($"\nНайдено объектов: {results.Count}");
            foreach (var item in results)
            {
                Console.WriteLine($"  {item}");
            }
        }

        static void ShowStatistics()
        {
            Console.WriteLine("=== СТАТИСТИКА ===");
            Console.WriteLine($"Всего пользователей: {users.Count}");
            Console.WriteLine($"Всего клиентов: {clients.Count}");
            Console.WriteLine($"Всего курьеров: {couriers.Count}");
            Console.WriteLine($"Всего блюд в меню: {menuItems.Count}");
            Console.WriteLine($"Всего заказов: {orders.Count}");

            if (orders.Any())
            {
                double avgCheck = orders.Average(o => o.TotalAmount);
                Console.WriteLine($"Средний чек: {avgCheck:C}");

                var popularItems = orders.SelectMany(o => o.Items)
                                          .GroupBy(i => i.Item.Name)
                                          .OrderByDescending(g => g.Count())
                                          .FirstOrDefault();
                if (popularItems != null)
                    Console.WriteLine($"Самое популярное блюдо: {popularItems.Key} (заказано {popularItems.Count()} раз)");
            }
        }
    }
}