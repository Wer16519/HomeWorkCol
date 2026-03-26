using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        private static readonly object ResourceA = new object();
        private static readonly object ResourceB = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("ДЕМОНСТРАЦИЯ ТУПИКА (DEADLOCK)");
            Console.WriteLine("Запускается два процесса, которые создадут взаимную блокировку...\n");

            Task.Run(() => Process1());
            Task.Run(() => Process2());

            Console.WriteLine("Нажмите Enter для завершения программы...");
            Console.WriteLine("(Если возник тупик, программа не завершится сама)");
            Console.ReadLine();

            Console.WriteLine("\nПрограмма завершена.");
        }

        static void Process1()
        {
            lock (ResourceA)
            {
                Console.WriteLine("[Процесс 1] Захватил Resource A");
                Thread.Sleep(1000);

                Console.WriteLine("[Процесс 1] Пытается захватить Resource B...");
                lock (ResourceB)
                {
                    Console.WriteLine("[Процесс 1] Захватил Resource B и выполнил работу.");
                }
            }
        }

        static void Process2()
        {
            lock (ResourceB)
            {
                Console.WriteLine("[Процесс 2] Захватил Resource B");
                Thread.Sleep(1000);

                Console.WriteLine("[Процесс 2] Пытается захватить Resource A...");
                lock (ResourceA)
                {
                    Console.WriteLine("[Процесс 2] Захватил Resource A и выполнил работу.");
                }
            }
        }
    }
}