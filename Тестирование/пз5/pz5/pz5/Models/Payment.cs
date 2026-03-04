using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class Payment
    {
        private double _amount;

        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public double Amount
        {
            get => _amount;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Сумма оплаты должна быть положительной.");
                _amount = value;
            }
        }

        public string PaymentMethod { get; set; }    
        public string Status { get; private set; } = "Ожидание";
        public DateTime PaymentDate { get; set; }

        public Payment() { }

        public Payment(int paymentId, int orderId, double amount, string paymentMethod)
        {
            PaymentId = paymentId;
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            PaymentDate = DateTime.Now;
        }

        public void ProcessPayment()
        {
            Status = "Успешно";
            PaymentDate = DateTime.Now;
            Console.WriteLine($"Платеж {PaymentId} на сумму {Amount:C} проведен успешно.");
        }

        public void Refund()
        {
            if (Status == "Успешно")
            {
                Status = "Возвращен";
                Console.WriteLine($"Платеж {PaymentId} возвращен.");
            }
            else
            {
                Console.WriteLine($"Невозможно вернуть платеж {PaymentId} со статусом '{Status}'.");
            }
        }

        public override string ToString()
        {
            return $"[Платеж {PaymentId}] Заказ {OrderId} | {Amount:C} | Метод: {PaymentMethod} | Статус: {Status} | Дата: {PaymentDate:d}";
        }
    }
}