using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class Client
    {
        private string _address;

        public int ClientId { get; set; }

        public string Address
        {
            get => _address;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Адрес не может быть пустым.");
                _address = value;
            }
        }

        public List<Order> Orders { get; set; } = new List<Order>();

        public Client() { }

        public Client(int clientId, string address)
        {
            ClientId = clientId;
            Address = address;
        }

        public void PlaceOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            Orders.Add(order);
            Console.WriteLine($"Клиент {ClientId} разместил заказ {order.OrderId}.");
        }

        public void CancelOrder(int orderId)
        {
            var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.UpdateStatus("Отменен");
                Console.WriteLine($"Заказ {orderId} отменен.");
            }
            else
            {
                Console.WriteLine($"Заказ {orderId} не найден у клиента.");
            }
        }

        public void TrackOrder(int orderId)
        {
            var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                Console.WriteLine($"Статус заказа {orderId}: {order.Status}");
            }
            else
            {
                Console.WriteLine($"Заказ {orderId} не найден.");
            }
        }

        public override string ToString()
        {
            return $"[Клиент {ClientId}] Адрес: {Address}, Заказов: {Orders.Count}";
        }
    }
}