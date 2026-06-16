namespace t1
{
    internal class Program
    {
        static bool paused = false;

        static void Main()
        {
            Console.WriteLine("Вывод чисел от 0 до 100000");

            Thread keyThread = new Thread(CheckKey);
            keyThread.Start();

            for (int i = 0; i <= 100000; i++)
            {
                if (i == 70000 && !paused)
                {
                    paused = true;
                    Console.WriteLine("\n=== ПАУЗА ===");

                    while (paused)
                    {
                        Thread.Sleep(100);
                    }

                    Console.WriteLine("=== ПРОДОЛЖАЕМ ===\n");
                }

                Console.WriteLine(i);
            }

            Console.WriteLine("\nГотово!");
            Console.ReadKey();
        }

        static void CheckKey()
        {
            while (true)
            {
                if (paused)
                {
                    Console.WriteLine("Нажми любую кнопку");

                    DateTime start = DateTime.Now;
                    while ((DateTime.Now - start).TotalSeconds < 3)
                    {
                        if (Console.KeyAvailable)
                        {
                            Console.ReadKey(true);
                            paused = false;
                            return;
                        }
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}