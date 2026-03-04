using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        public Admin() { }

        public Admin(int adminId)
        {
            AdminId = adminId;
        }

        public void AddMenuItem(MenuItem item)
        {
            Console.WriteLine($"Админ {AdminId} добавил новое блюдо: {item.Name}");
        }

        public void EditMenuItem(MenuItem item)
        {
            Console.WriteLine($"Админ {AdminId} отредактировал блюдо ID {item.Id}");
        }

        public void DeleteMenuItem(int itemId)
        {
            Console.WriteLine($"Админ {AdminId} удалил блюдо ID {itemId}");
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            Console.WriteLine($"Админ {AdminId} обновил статус заказа {orderId} на '{status}'");
        }

        public override string ToString()
        {
            return $"[Админ {AdminId}]";
        }
    }
}