using System;
using System.Collections.Generic;
using System.Text;
using CardMaster.Models;
using System.Collections.ObjectModel;

namespace CardMaster.Operations
{
    public static class DataStore
    {
        public static ObservableCollection<Card> Cards { get; set; }
        public static ObservableCollection<Discount> Discounts { get; set; }

        static DataStore()
        {

            Cards = new ObservableCollection<Card>
            {
                new Card
                {
                    Id = 1,
                    Name = "Магнит",
                    Image = "magnit.png",
                    Number = "1234123412341234"
                },
                new Card
                {
                    Id = 2,
                    Name = "Пятерочка",
                    Image = "pyaterochka.png",
                    Number = "5678567856785678"
                },
                new Card
                {
                    Id = 3,
                    Name = "Дикси",
                    Image = "diksi.png",
                    Number = "9012901290129012"
                }
            };

            Discounts = new ObservableCollection<Discount>
            {
                new Discount
                {
                    Id = 1,
                    Name = "Магнит",
                    Promocode = "MAGNIT20",
                    Description = "Скидка 20% на всё",
                    Image = "magnit_disk.png"
                },
                new Discount
                {
                    Id = 2,
                    Name = "Пятерочка",
                    Promocode = "PYAT10",
                    Description = "Скидка 10% на молочку",
                    Image = "pyaterochka_disk.png"
                },
                new Discount
                {
                    Id = 3,
                    Name = "Дикси",
                    Promocode = "DIKSI15",
                    Description = "Скидка 15% на выпечку",
                    Image = "diksi_disc.png"
                }
            };
        }
    }
}
