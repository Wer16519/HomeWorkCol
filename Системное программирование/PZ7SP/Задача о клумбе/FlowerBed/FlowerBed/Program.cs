using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class FlowerBed
{
    private static bool[] flowers = new bool[40];
    private static object gardenLock = new object();
    private static Random rand = new Random();

    static void Main()
    {
        Thread gardener1 = new Thread(() => Gardener(1));
        Thread gardener2 = new Thread(() => Gardener(2));
        Thread nature = new Thread(Nature);

        gardener1.Start();
        gardener2.Start();
        nature.Start();

        gardener1.Join();
        gardener2.Join();
        nature.Join();
    }

    static void Gardener(int id)
    {
        while (true)
        {
            lock (gardenLock)
            {
                for (int i = 0; i < flowers.Length; i++)
                {
                    if (flowers[i])
                    {
                        flowers[i] = false;
                        Console.WriteLine($"Садовник {id} полил цветок {i + 1}");
                    }
                }
            }
            Thread.Sleep(1000);
        }
    }

    static void Nature()
    {
        while (true)
        {
            lock (gardenLock)
            {
                int toWither = rand.Next(0, flowers.Length);
                flowers[toWither] = true;
                Console.WriteLine($"Цветок {toWither + 1} увял.");
            }
            Thread.Sleep(1500);
        }
    }
}