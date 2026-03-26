using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Supermarket
{
    private static object[] cashierLocks = new object[2];
    private static int[] queueLengths = new int[2];

    static void Main()
    {
        for (int i = 0; i < 2; i++)
            cashierLocks[i] = new object();

        Thread[] cashiers = new Thread[2];
        for (int i = 0; i < 2; i++)
        {
            int id = i;
            cashiers[i] = new Thread(() => Cashier(id));
            cashiers[i].Start();
        }

        Random rand = new Random();
        for (int i = 1; i <= 10; i++)
        {
            int customerId = i;
            Thread customerThread = new Thread(() => Customer(customerId, rand.Next(0, 2)));
            customerThread.Start();
            Thread.Sleep(rand.Next(500, 1500));
        }
    }

    static void Cashier(int id)
    {
        while (true)
        {
            lock (cashierLocks[id])
            {
                if (queueLengths[id] == 0)
                {
                    Console.WriteLine($"Кассир {id} спит...");
                    Monitor.Wait(cashierLocks[id]);
                }
                queueLengths[id]--;
                Console.WriteLine($"Кассир {id} обслуживает покупателя. Очередь: {queueLengths[id]}");
            }
            Thread.Sleep(2000);
        }
    }

    static void Customer(int customerId, int cashierId)
    {
        lock (cashierLocks[cashierId])
        {
            queueLengths[cashierId]++;
            Console.WriteLine($"Покупатель {customerId} встал в очередь к кассиру {cashierId}. Очередь: {queueLengths[cashierId]}");
            Monitor.Pulse(cashierLocks[cashierId]);
        }
    }
}