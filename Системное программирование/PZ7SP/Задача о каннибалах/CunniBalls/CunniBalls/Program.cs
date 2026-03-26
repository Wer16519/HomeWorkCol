using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Cannibals
{
    private static int potCapacity = 5;
    private static int mealsInPot = 0;
    private static object potLock = new object();

    public static void Cook()
    {
        while (true)
        {
            lock (potLock)
            {
                if (mealsInPot == 0)
                {
                    Console.WriteLine("Повар наполняет горшок...");
                    mealsInPot = potCapacity;
                    Monitor.PulseAll(potLock);
                }
                Monitor.Wait(potLock);
            }
        }
    }

    public static void Cannibal(object id)
    {
        while (true)
        {
            lock (potLock)
            {
                while (mealsInPot == 0)
                {
                    Console.WriteLine($"Каннибал {id} будит повара.");
                    Monitor.Pulse(potLock);
                    Monitor.Wait(potLock);
                }
                mealsInPot--;
                Console.WriteLine($"Каннибал {id} ест. Осталось порций: {mealsInPot}");
            }
            Thread.Sleep(1000);
        }
    }

    static void Main()
    {
        Thread cookThread = new Thread(Cook);
        cookThread.Start();

        for (int i = 1; i <= 3; i++)
        {
            Thread cannibalThread = new Thread(Cannibal);
            cannibalThread.Start(i);
        }
    }
}