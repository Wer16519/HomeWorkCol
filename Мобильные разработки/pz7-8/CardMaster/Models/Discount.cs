using System;
using System.Collections.Generic;
using System.Text;

namespace CardMaster.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Promocode { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
