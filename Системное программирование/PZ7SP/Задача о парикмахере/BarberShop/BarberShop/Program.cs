using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class BarberShop
{
    private static int waitingCustomers = 0;
    private static object lockObject = new object();
    private static bool barberSleeping = true;

    public static void Barber()
    {
        while (true)
        {
            lock (lockObject)
            {
                if (waitingCustomers == 0)
                {
                    Console.WriteLine("Парикмахер спит...");
                    barberSleeping = true;
                    Monitor.Wait(lockObject);
                }
                waitingCustomers--;
                Console.WriteLine("Парикмахер стрижет клиента. Ожидающих: " + waitingCustomers);
            }
            Thread.Sleep(2000); // Процесс стрижки
            Console.WriteLine("Парикмахер закончил стрижку.");
        }
    }

    public static void Customer(object id)
    {
        lock (lockObject)
        {
            if (barberSleeping)
            {
                barberSleeping = false;
                Console.WriteLine("Клиент " + id + " будит парикмахера.");
                Monitor.Pulse(lockObject);
            }
            else
            {
                waitingCustomers++;
                Console.WriteLine("Клиент " + id + " встал в очередь. Ожидающих: " + waitingCustomers);
            }
        }
    }

    static void Main()
    {
        Thread barberThread = new Thread(Barber);
        barberThread.Start();

        Random rand = new Random();
        for (int i = 1; i <= 10; i++)
        {
            Thread customerThread = new Thread(Customer);
            customerThread.Start(i);
            Thread.Sleep(rand.Next(1000, 3000));
        }
    }
}
