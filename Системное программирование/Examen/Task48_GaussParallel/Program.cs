using System;
using System.Threading.Tasks;

namespace Task48_GaussParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Решение СЛАУ методом Гаусса (многопоточность) ===\n");

            int n = 4;
            double[,] matrix = new double[,]
            {
                { 2, 1, -1, 8, 7 },
                { -3, -1, 2, -11, -8 },
                { -2, 1, 2, -3, 0 },
                { 1, 3, -1, 2, 5 }
            };

            Console.WriteLine("Исходная матрица:");
            PrintMatrix(matrix, n);

            double[] solution = SolveGaussParallel(matrix, n);

            Console.WriteLine("\nРешение:");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"x{i + 1} = {solution[i]:F4}");
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }

        static double[] SolveGaussParallel(double[,] matrix, int n)
        {
            double[,] a = (double[,])matrix.Clone();

            for (int k = 0; k < n; k++)
            {
                if (a[k, k] == 0)
                {
                    for (int i = k + 1; i < n; i++)
                    {
                        if (a[i, k] != 0)
                        {
                            for (int j = k; j <= n; j++)
                            {
                                double temp = a[k, j];
                                a[k, j] = a[i, j];
                                a[i, j] = temp;
                            }
                            break;
                        }
                    }
                }

                double pivot = a[k, k];
                for (int j = k; j <= n; j++)
                {
                    a[k, j] /= pivot;
                }

                int[] rows = new int[n - k - 1];
                for (int i = k + 1; i < n; i++)
                {
                    rows[i - k - 1] = i;
                }

                Parallel.For(0, rows.Length, idx =>
                {
                    int i = rows[idx];
                    double factor = a[i, k];
                    for (int j = k; j <= n; j++)
                    {
                        a[i, j] -= factor * a[k, j];
                    }
                });
            }

            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = a[i, n];
                for (int j = i + 1; j < n; j++)
                {
                    x[i] -= a[i, j] * x[j];
                }
            }

            return x;
        }

        static void PrintMatrix(double[,] matrix, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    Console.Write($"{matrix[i, j],8:F2} ");
                }
                Console.WriteLine();
            }
        }
    }
}