using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace t11
{
    class Program
    {
        static async Task Main()
        {
            string fileName = "output.txt";

            // Асинхронная запись в файл и открытие в notepad
            Task fileTask = Task.Run(async () =>
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        await sw.WriteLineAsync($"Строка {i}: данные записаны асинхронно.");
                        await Task.Delay(50);
                    }
                }

                Process.Start(new ProcessStartInfo("notepad.exe", fileName)
                {
                    UseShellExecute = true
                });
            });

            // Бесконечный вывод "Hello, World" в консоль
            while (!fileTask.IsCompleted)
            {
                Console.WriteLine("Hello, World");
            }

            await fileTask;

            Console.WriteLine("\nЗапись в файл завершена. Файл открыт в Notepad.");
            Console.ReadKey();
        }
    }
}