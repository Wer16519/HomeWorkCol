using System;
using System.Threading;

namespace t22
{
    class Program
    {
        static bool running = true;
        static Random rnd = new Random();

        static void Thread1()
        {
            while (running)
            {
                int a = rnd.Next(1, 100);
                int b = rnd.Next(1, 100);
                Console.WriteLine($"[Поток 1] Сложение: {a} + {b} = {a + b}");
                Thread.Sleep(1500);
            }
        }

        static void Thread2()
        {
            while (running)
            {
                int a = rnd.Next(1, 100);
                int b = rnd.Next(1, 100);
                Console.WriteLine($"[Поток 2] Умножение: {a} * {b} = {a * b}");
                Thread.Sleep(2000);
            }
        }

        static void Thread3()
        {
            while (running)
            {
                int a = rnd.Next(1, 100);
                int b = rnd.Next(1, 100);
                Console.WriteLine($"[Поток 3] Вычитание: {a} - {b} = {a - b}");
                Thread.Sleep(2500);
            }
        }

        static void Main()
        {
            Console.WriteLine("Запуск 3 потоков. Введите 'exit' для остановки.\n");

            Thread t1 = new Thread(Thread1);
            Thread t2 = new Thread(Thread2);
            Thread t3 = new Thread(Thread3);

            t1.Start();
            t2.Start();
            t3.Start();

            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd?.ToLower() == "exit")
                {
                    running = false;
                    break;
                }
            }

            t1.Join();
            t2.Join();
            t3.Join();

            Console.WriteLine("Все потоки остановлены. Программа завершена.");
        }
    }
}