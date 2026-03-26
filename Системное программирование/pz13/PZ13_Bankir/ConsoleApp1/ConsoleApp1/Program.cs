using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== АЛГОРИТМ БАНКИРА ===");
            Console.WriteLine("Проверка безопасности состояния системы\n");

            int[] available = { 3, 3, 2 };

            int[,] max = {
                { 7, 5, 3 },
                { 3, 2, 2 },
                { 9, 0, 2 },
                { 2, 2, 2 },
                { 4, 3, 3 }
            };

            int[,] allocation = {
                { 0, 1, 0 },
                { 2, 0, 0 },
                { 3, 0, 2 },
                { 2, 1, 1 },
                { 0, 0, 2 } 
            };

            int processes = allocation.GetLength(0);
            int resources = available.Length;

            int[,] need = new int[processes, resources];
            for (int i = 0; i < processes; i++)
            {
                for (int j = 0; j < resources; j++)
                {
                    need[i, j] = max[i, j] - allocation[i, j];
                }
            }

            Console.WriteLine("Текущее состояние системы:");
            Console.WriteLine("\nДоступные ресурсы: A={0} B={1} C={2}", available[0], available[1], available[2]);
            Console.WriteLine("\nПроцесс | Allocation  | Max       | Need");
            Console.WriteLine("        | A  B  C    | A  B  C   | A  B  C");
            for (int i = 0; i < processes; i++)
            {
                Console.WriteLine("P{0}     | {1}  {2}  {3}    | {4}  {5}  {6}  | {7}  {8}  {9}",
                    i,
                    allocation[i, 0], allocation[i, 1], allocation[i, 2],
                    max[i, 0], max[i, 1], max[i, 2],
                    need[i, 0], need[i, 1], need[i, 2]);
            }

            Console.WriteLine("\n" + new string('-', 60));
            Console.WriteLine("Проверка безопасности...\n");

            bool isSafe = IsSafeState(available, allocation, need, processes, resources);

            if (isSafe)
            {
                Console.WriteLine("\n✓ СОСТОЯНИЕ БЕЗОПАСНО - тупик не возникнет.");
            }
            else
            {
                Console.WriteLine("\n✗ СОСТОЯНИЕ НЕБЕЗОПАСНО - существует риск тупика!");
            }

            Console.WriteLine("\nНажмите Enter для выхода...");
            Console.ReadLine();
        }

        static bool IsSafeState(int[] available, int[,] allocation, int[,] need, int processes, int resources)
        {
            bool[] finished = new bool[processes];
            int[] work = (int[])available.Clone();
            int[] safeSequence = new int[processes];
            int count = 0;

            while (count < processes)
            {
                bool found = false;
                for (int i = 0; i < processes; i++)
                {
                    if (!finished[i])
                    {
                        bool canExecute = true;
                        for (int j = 0; j < resources; j++)
                        {
                            if (need[i, j] > work[j])
                            {
                                canExecute = false;
                                break;
                            }
                        }

                        if (canExecute)
                        {
                            for (int j = 0; j < resources; j++)
                            {
                                work[j] += allocation[i, j];
                            }
                            safeSequence[count] = i;
                            finished[i] = true;
                            count++;
                            found = true;

                            Console.WriteLine("Процесс P{0} выполнен. Освобождены ресурсы. Текущие доступные: A={1} B={2} C={3}",
                                i, work[0], work[1], work[2]);
                        }
                    }
                }

                if (!found)
                {
                    Console.WriteLine("\nНЕ найден процесс, который можно выполнить!");
                    return false;
                }
            }

            Console.Write("\nБезопасная последовательность выполнения: ");
            for (int i = 0; i < safeSequence.Length; i++)
            {
                Console.Write("P" + safeSequence[i]);
                if (i < safeSequence.Length - 1) Console.Write(" → ");
            }
            Console.WriteLine();

            return true;
        }
    }
}