using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace t15
{
    class Program
    {
        static async Task Main()
        {
            string fileName = "output.txt";

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

            while (!fileTask.IsCompleted)
            {
                Console.WriteLine("Hello, World");
            }

            await fileTask;
        }
    }
}