using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace Task27_AnonymousPipes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Анонимные каналы ===\n");

            using (AnonymousPipeServerStream pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            {
                Console.WriteLine("[Главный] Создан анонимный канал");

                string clientHandle = pipeServer.GetClientHandleAsString();

                Thread childThread = new Thread(() => RunChild(clientHandle));
                childThread.Start();

                Thread.Sleep(500);

                string message = "Привет из главного процесса!";
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                pipeServer.Write(bytes, 0, bytes.Length);
                pipeServer.Flush();
                pipeServer.WaitForPipeDrain();
                Console.WriteLine($"[Главный] Отправлено сообщение: \"{message}\"");

                pipeServer.Close();

                childThread.Join(2000);

                Console.WriteLine("\n[Главный] Программа завершена");
                Console.ReadKey();
            }
        }

        static void RunChild(string pipeHandle)
        {
            try
            {
                using (AnonymousPipeClientStream pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeHandle))
                {
                    Console.WriteLine("[Дочерний] Подключен к анонимному каналу");

                    byte[] buffer = new byte[1024];
                    int bytesRead = pipeClient.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"[Дочерний] Получено сообщение: \"{received}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Дочерний] Ошибка: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("[Дочерний] Завершение работы");
            }
        }
    }
}