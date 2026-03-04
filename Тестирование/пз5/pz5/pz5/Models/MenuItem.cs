using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class MenuItem
    {
        private string _name;
        private double _price;

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название блюда не может быть пустым.");
                _name = value;
            }
        }

        public string Description { get; set; }

        public double Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Цена должна быть положительной.");
                _price = value;
            }
        }

        public string Category { get; set; }
        public bool Available { get; set; } = true;

        public MenuItem() { }

        public MenuItem(int id, string name, string description, double price, string category, bool available = true)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            Available = available;
        }

        public bool CheckAvailability() => Available;

        public void SetAvailability(bool status)
        {
            Available = status;
            Console.WriteLine($"Доступность блюда '{Name}' изменена на: {(status ? "Доступно" : "Недоступно")}");
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} - {Price:C} ({Category}) | {(Available ? "В наличии" : "Нет в наличии")}";
        }
    }
}