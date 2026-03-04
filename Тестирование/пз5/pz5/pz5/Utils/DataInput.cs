using System;
using System.Collections.Generic;
using System.Text;
using pz5.Models;

namespace pz5.Utils
{
    public static class DataInput
    {
        public static MenuItem InputMenuItem()
        {
            Console.WriteLine("Введите данные для нового блюда:");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Название: ");
            string name = Console.ReadLine();

            Console.Write("Описание: ");
            string desc = Console.ReadLine();

            Console.Write("Цена: ");
            double price = double.Parse(Console.ReadLine());

            Console.Write("Категория: ");
            string category = Console.ReadLine();

            Console.Write("Доступно (true/false): ");
            bool available = bool.Parse(Console.ReadLine());

            return new MenuItem(id, name, desc, price, category, available);
        }

        public static Client InputClient()
        {
            Console.WriteLine("Введите данные нового клиента:");

            Console.Write("ID клиента: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Адрес: ");
            string address = Console.ReadLine();

            return new Client(id, address);
        }
    }
}