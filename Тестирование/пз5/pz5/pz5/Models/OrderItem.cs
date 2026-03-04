using System;
using System.Collections.Generic;
using System.Text;

namespace pz5.Models
{
    public class OrderItem
    {
        private int _quantity;

        public int OrderItemId { get; set; }

        public MenuItem Item { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Количество должно быть больше 0.");
                _quantity = value;
            }
        }

        public double Price { get; set; }

        public OrderItem() { }

        public OrderItem(int orderItemId, MenuItem item, int quantity)
        {
            OrderItemId = orderItemId;
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Quantity = quantity;
            Price = item.Price;   
        }

        public double GetSubtotal()
        {
            return Price * Quantity;
        }

        public override string ToString()
        {
            return $"{Item.Name} x{Quantity} = {GetSubtotal():C}";
        }
    }
}