using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class Courier
    {
        public int CourierId { get; set; }
        public List<Order> DeliveryOrders { get; set; } = new List<Order>();

        public Courier() { }

        public Courier(int courierId)
        {
            CourierId = courierId;
        }

        public void AcceptOrder(int orderId)
        {
            Console.WriteLine($"Курьер {CourierId} принял заказ {orderId}.");
        }

        public void UpdateDeliveryStatus(int orderId, string status)
        {
            var order = DeliveryOrders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.UpdateStatus(status);
                Console.WriteLine($"Курьер {CourierId} обновил статус заказа {orderId} на '{status}'.");
            }
            else
            {
                Console.WriteLine($"Заказ {orderId} не назначен курьеру {CourierId}.");
            }
        }

        public override string ToString()
        {
            return $"[Курьер {CourierId}] Доставок: {DeliveryOrders.Count}";
        }
    }
}