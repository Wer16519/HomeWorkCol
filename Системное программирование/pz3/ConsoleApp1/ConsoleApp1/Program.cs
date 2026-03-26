using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        public static string[] _args;
        static void Main(string[] args)
        {
            _args = args;

            int pid = fork();

            if (pid == -1)
            {
                Console.WriteLine("fork");
           
            }
            else if (pid == 0)
            {
                Console.WriteLine($"Дочерний процесс: PID = {getpid()}");
            }
            else
            {
                Console.WriteLine($"Родительский процесс: PID = {getpid()}, Дочерний PID = {pid}");
            }
            Console.ReadKey();


        }

        static int fork()
        {
            try
            {
                if (_args.Length == 0 || (_args.Length>0 && _args[0] !="child"))
                {

                    Process parent = Process.GetCurrentProcess();
                    Console.WriteLine(parent.MainModule.FileName);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = parent.MainModule.FileName;
                    startInfo.Arguments = "child";
                    Process child = new Process();
                    child.StartInfo = startInfo;
                    child.Start();
                    return child.Id;
                }
                else return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        static int getpid()
        {
            return Process.GetCurrentProcess().Id;
        }
    }
}
