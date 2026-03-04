using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class Order
    {
        private string _status;
        private string _deliveryAddress;

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status
        {
            get => _status;
            private set => _status = value;      
        }

        public double TotalAmount { get; private set; }   

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Адрес доставки не может быть пустым.");
                _deliveryAddress = value;
            }
        }

        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();

        public int ClientId { get; set; }

        public Order()
        {
            Status = "Новый";
        }

        public Order(int orderId, string deliveryAddress, int clientId)
        {
            OrderId = orderId;
            DeliveryAddress = deliveryAddress;
            ClientId = clientId;
            Status = "Новый";
            OrderDate = DateTime.Now;
        }

        public double CalculateTotal()
        {
            TotalAmount = Items.Sum(item => item.GetSubtotal());
            return TotalAmount;
        }

        public void UpdateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Статус не может быть пустым.");

            _status = status;
            Console.WriteLine($"Статус заказа {OrderId} обновлен на '{status}'");
        }

        public void AddItem(OrderItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Items.Add(item);
            CalculateTotal();    
            Console.WriteLine($"Товар '{item.Item.Name}' добавлен в заказ {OrderId}.");
        }

        public override string ToString()
        {
            return $"[Заказ {OrderId}] от {OrderDate:d} | Статус: {Status} | Сумма: {CalculateTotal():C} | Адрес: {DeliveryAddress}";
        }
    }
}