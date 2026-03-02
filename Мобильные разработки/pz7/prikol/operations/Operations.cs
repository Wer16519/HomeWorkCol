using prikol.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace prikol.operations
{
    internal static class Operations
    {
        public static ObservableCollection<Cards> AllCards { get; set; }
        static Operations()
        {
            AllCards = new ObservableCollection<Cards>()
            {
                new Cards()
                {
                    Id = 1,
                    Name = "Магнит",
                    Image = "carta.png",
                    Number = "12341241124315"
                },
                new Cards()
                {
                    Id = 2,
                    Name = "Пятерочка",
                    Image = "carta.png",
                    Number = "1234124115477"
                },
                new Cards()
                {
                    Id = 3,
                    Name = "Дикси",
                    Image = "carta.png",
                    Number = "12345645724315"
                }
            };
        }
    }
}