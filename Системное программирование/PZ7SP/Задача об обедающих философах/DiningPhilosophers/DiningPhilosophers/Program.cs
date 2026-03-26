using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class DiningPhilosophers
{
    private static object[] forks = new object[5];
    private static Random rand = new Random();

    static void Main()
    {
        for (int i = 0; i < 5; i++)
            forks[i] = new object();

        Thread[] philosophers = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            int id = i;
            philosophers[i] = new Thread(() => Philosopher(id));
            philosophers[i].Start();
        }

        foreach (var philosopher in philosophers)
            philosopher.Join();
    }

    static void Philosopher(int id)
    {
        int leftFork = id;
        int rightFork = (id + 1) % 5;

        while (true)
        {
            Console.WriteLine($"Философ {id} размышляет...");
            Thread.Sleep(rand.Next(1000, 3000));

            lock (forks[leftFork])
            {
                lock (forks[rightFork])
                {
                    Console.WriteLine($"Философ {id} ест...");
                    Thread.Sleep(rand.Next(1000, 3000));
                }
            }
        }
    }
}