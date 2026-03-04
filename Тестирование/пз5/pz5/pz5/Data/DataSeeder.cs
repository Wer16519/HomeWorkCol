using System;
using System.Collections.Generic;
using System.Text;
using pz5.Models;

namespace pz5.Data
{
    public class DataSeeder
    {
        private List<User> Users { get; set; } = new List<User>();
        private List<Client> Clients { get; set; } = new List<Client>();
        private List<Courier> Couriers { get; set; } = new List<Courier>();
        private List<Admin> Admins { get; set; } = new List<Admin>();
        private List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private List<Order> Orders { get; set; } = new List<Order>();
        private List<Payment> Payments { get; set; } = new List<Payment>();
        private List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        private Random _random = new Random();

        public DataSeeder()
        {
        }

        public void SeedWithValidation()
        {
            Console.WriteLine("=== Заполнение с валидацией ===");

            AddUserWithUniqueId(1, "Иван Иванов", "+7 (123) 456-78-90", "ivan@mail.com");
            AddUserWithUniqueId(2, "Петр Петров", "+7 (987) 654-32-10", "petr@mail.com");

            AddClientWithUniqueId(101, "ул. Ленина, д.1, кв.1");
            AddClientWithUniqueId(102, "пр. Мира, д.5, кв.42");

            AddCourierWithUniqueId(201);
            AddCourierWithUniqueId(202);

            AddAdminWithUniqueId(301);

            AddMenuItemWithUniqueId(1, "Пицца Маргарита", "Классическая пицца с томатами и моцареллой", 450.00, "Пицца");
            AddMenuItemWithUniqueId(2, "Борщ", "Украинский борщ с пампушкой", 250.00, "Супы");
            AddMenuItemWithUniqueId(3, "Цезарь с курицей", "Салат с курицей и соусом Цезарь", 350.00, "Салаты");

            Console.WriteLine("Валидированные данные добавлены.\n");
        }

        private void AddUserWithUniqueId(int id, string name, string phone, string email)
        {
            if (Users.Any(u => u.UserId == id))
                throw new Exception($"Пользователь с ID {id} уже существует.");
            Users.Add(new User(id, name, phone, email));
        }

        private void AddClientWithUniqueId(int id, string address)
        {
            if (Clients.Any(c => c.ClientId == id))
                throw new Exception($"Клиент с ID {id} уже существует.");
            Clients.Add(new Client(id, address));
        }

        private void AddCourierWithUniqueId(int id)
        {
            if (Couriers.Any(c => c.CourierId == id))
                throw new Exception($"Курьер с ID {id} уже существует.");
            Couriers.Add(new Courier(id));
        }

        private void AddAdminWithUniqueId(int id)
        {
            if (Admins.Any(a => a.AdminId == id))
                throw new Exception($"Админ с ID {id} уже существует.");
            Admins.Add(new Admin(id));
        }

        private void AddMenuItemWithUniqueId(int id, string name, string desc, double price, string category)
        {
            if (MenuItems.Any(m => m.Id == id))
                throw new Exception($"Блюдо с ID {id} уже существует.");
            MenuItems.Add(new MenuItem(id, name, desc, price, category));
        }


        public void SeedWithRandomData(int count)
        {
            Console.WriteLine($"\n=== Генерация {count} случайных объектов (пример для MenuItem) ===");

            string[] names = { "Бургер", "Картошка фри", "Кола", "Суп", "Салат", "Стейк", "Роллы", "Паста" };
            string[] categories = { "Фастфуд", "Напитки", "Супы", "Основное", "Японская" };

            for (int i = 1; i <= count; i++)
            {
                int newId = MenuItems.Count + 1;
                string randomName = names[_random.Next(names.Length)] + " " + _random.Next(100);
                string randomCategory = categories[_random.Next(categories.Length)];
                double randomPrice = Math.Round(_random.NextDouble() * 500 + 50, 2);     

                try
                {
                    var item = new MenuItem(newId, randomName, $"Описание для {randomName}", randomPrice, randomCategory);
                    MenuItems.Add(item);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Ошибка генерации: {ex.Message}. Пропускаем.");
                }
            }
            Console.WriteLine($"Сгенерировано {count} новых пунктов меню.\n");
        }

        public void SeedFromPredefinedLists()
        {
            Console.WriteLine("=== Заполнение предопределенными данными ===");

            Users.Clear();
            Clients.Clear();
            Couriers.Clear();
            Admins.Clear();
            MenuItems.Clear();
            Orders.Clear();
            Payments.Clear();
            OrderItems.Clear();

            Users.Add(new User(1, "Дмитрий Соколов", "+79001234567", "dima@mail.ru"));
            Users.Add(new User(2, "Анна Смирнова", "+79007654321", "anna@mail.ru"));
            Users.Add(new User(3, "Сергей Иванов", "+79112223344", "sergey@mail.ru"));

            Clients.Add(new Client(101, "ул. Тверская, д.7, кв.15"));
            Clients.Add(new Client(102, "ул. Арбат, д.12, кв.8"));
            Clients.Add(new Client(103, "Ленинградский пр-т, д.20, кв.101"));

            Couriers.Add(new Courier(201));
            Couriers.Add(new Courier(202));
            Couriers.Add(new Courier(203));

            Admins.Add(new Admin(301));
            Admins.Add(new Admin(302));

            MenuItems.Add(new MenuItem(1, "Пицца Пепперони", "Пикантная пепперони с сыром", 520.00, "Пицца"));
            MenuItems.Add(new MenuItem(2, "Лазанья", "Итальянская лазанья с мясом", 380.00, "Паста"));
            MenuItems.Add(new MenuItem(3, "Чизкейк", "Нью-Йорк", 290.00, "Десерты"));
            MenuItems.Add(new MenuItem(4, "Капучино", "Кофе с молоком", 180.00, "Напитки"));
            MenuItems.Add(new MenuItem(5, "Греческий салат", "Свежие овощи с сыром фета", 310.00, "Салаты"));

            var order1 = new Order(1001, Clients[0].Address, Clients[0].ClientId);
            var item1 = new OrderItem(1, MenuItems[0], 2);   
            var item2 = new OrderItem(2, MenuItems[3], 1);   
            order1.AddItem(item1);
            order1.AddItem(item2);
            order1.CalculateTotal();
            Orders.Add(order1);
            Clients[0].Orders.Add(order1);
            OrderItems.Add(item1);
            OrderItems.Add(item2);

            var order2 = new Order(1002, Clients[1].Address, Clients[1].ClientId);
            var item3 = new OrderItem(3, MenuItems[1], 1);   
            var item4 = new OrderItem(4, MenuItems[4], 1);   
            var item5 = new OrderItem(5, MenuItems[2], 2);   
            order2.AddItem(item3);
            order2.AddItem(item4);
            order2.AddItem(item5);
            order2.CalculateTotal();
            Orders.Add(order2);
            Clients[1].Orders.Add(order2);
            OrderItems.Add(item3);
            OrderItems.Add(item4);
            OrderItems.Add(item5);

            var order3 = new Order(1003, Clients[2].Address, Clients[2].ClientId);
            var item6 = new OrderItem(6, MenuItems[0], 1);   
            var item7 = new OrderItem(7, MenuItems[3], 2);   
            order3.AddItem(item6);
            order3.AddItem(item7);
            order3.CalculateTotal();
            Orders.Add(order3);
            Clients[2].Orders.Add(order3);
            OrderItems.Add(item6);
            OrderItems.Add(item7);

            Payments.Add(new Payment(501, 1001, order1.TotalAmount, "Карта"));
            Payments.Add(new Payment(502, 1002, order2.TotalAmount, "Наличные"));
            Payments.Add(new Payment(503, 1003, order3.TotalAmount, "Карта"));

            Payments[0].ProcessPayment();
            Payments[1].ProcessPayment();
            Payments[2].ProcessPayment();

            Console.WriteLine("Предопределенные данные загружены.\n");
        }

        public List<User> GetUsers() => Users;
        public List<Client> GetClients() => Clients;
        public List<Courier> GetCouriers() => Couriers;
        public List<Admin> GetAdmins() => Admins;
        public List<MenuItem> GetMenuItems() => MenuItems;
        public List<Order> GetOrders() => Orders;
        public List<Payment> GetPayments() => Payments;

        public List<object> SearchByCriteria(string criteria)
        {
            var results = new List<object>();

            if (string.IsNullOrWhiteSpace(criteria))
                return results;

            criteria = criteria.ToLower();

            results.AddRange(MenuItems.Where(x =>
                x.Name.ToLower().Contains(criteria) ||
                x.Description.ToLower().Contains(criteria) ||
                x.Category.ToLower().Contains(criteria)).Cast<object>());

            results.AddRange(Orders.Where(x =>
                x.Status.ToLower().Contains(criteria) ||
                x.DeliveryAddress.ToLower().Contains(criteria)).Cast<object>());

            results.AddRange(Users.Where(x =>
                x.Name.ToLower().Contains(criteria) ||
                x.Email.ToLower().Contains(criteria)).Cast<object>());

            results.AddRange(Clients.Where(x =>
                x.Address.ToLower().Contains(criteria)).Cast<object>());

            return results.Distinct().ToList();    
        }

        public void EstablishRelationships()
        {
            Console.WriteLine("Установка связей между объектами...");

            if (Couriers.Any() && Orders.Any())
            {
                foreach (var order in Orders)
                {
                    var randomCourier = Couriers[_random.Next(Couriers.Count)];
                    randomCourier.DeliveryOrders.Add(order);
                }
                Console.WriteLine("Заказы назначены курьерам.");
            }

            Console.WriteLine("Связи установлены.\n");
        }
    }
}