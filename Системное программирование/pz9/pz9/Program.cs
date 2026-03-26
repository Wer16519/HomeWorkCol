using System;
using System.IO;
using System.Linq;

namespace pz9
{
    class Program
    {
        static int[,] matrix;
        static int[] vectorB;
        static int[] vectorC;
        static int rows, cols;

        static void Main(string[] args)
        {
            Console.WriteLine("   ВЫПОЛНЕНИЕ ВСЕХ 10 ВАРИАНТОВ ПОДРЯД");

            Console.WriteLine("\nВАРИАНТ 1");
            Console.WriteLine("B = ln(a[p,j]), C = 1/ln(a[p,j])\n");
            LoadMatrix("matrix.txt");
            if (matrix != null)
            {
                DisplayMatrix();
                int p = 0;
                Console.WriteLine($"\nВыбрана строка p = {p + 1}\n");
                double[] B = new double[cols];
                double[] C = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    try
                    {
                        B[j] = Math.Log(matrix[p, j]);
                        C[j] = 1.0 / B[j];
                        Console.WriteLine($"j={j + 1}: ln({matrix[p, j]}) = {B[j]:F4}, 1/ln = {C[j]:F4}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"j={j + 1}: ОШИБКА - {ex.Message}");
                        B[j] = 0; C[j] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 2");
            Console.WriteLine("C = целая часть(a[i,p]/cos(b[i]))\n");
            LoadMatrix("matrix.txt");
            LoadVector("vectorB.txt", out vectorB);
            if (matrix != null && vectorB != null)
            {
                DisplayMatrix();
                DisplayVector(vectorB, "B");
                int p = 0;
                Console.WriteLine($"\nВыбран столбец p = {p + 1}\n");
                int[] C = new int[rows];
                for (int i = 0; i < rows; i++)
                {
                    try
                    {
                        double cosVal = Math.Cos(vectorB[i]);
                        C[i] = (int)(matrix[i, p] / cosVal);
                        Console.WriteLine($"i={i + 1}: {matrix[i, p]}/cos({vectorB[i]}) = {matrix[i, p] / cosVal:F4}, целая часть = {C[i]}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"i={i + 1}: ОШИБКА - {ex.Message}");
                        C[i] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 3");
            Console.WriteLine("C = целая часть(a[p,j]/cos(b[j]))\n");
            LoadMatrix("matrix.txt");
            LoadVector("vectorB.txt", out vectorB);
            if (matrix != null && vectorB != null)
            {
                DisplayMatrix();
                DisplayVector(vectorB, "B");
                int p = 0;
                Console.WriteLine($"\nВыбрана строка p = {p + 1}\n");
                int[] C = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    try
                    {
                        double cosVal = Math.Cos(vectorB[j]);
                        C[j] = (int)(matrix[p, j] / cosVal);
                        Console.WriteLine($"j={j + 1}: {matrix[p, j]}/cos({vectorB[j]}) = {matrix[p, j] / cosVal:F4}, целая часть = {C[j]}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"j={j + 1}: ОШИБКА - {ex.Message}");
                        C[j] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 4");
            Console.WriteLine("C = целая часть(b[i]/sin(a[i,p]))\n");
            LoadMatrix("matrix.txt");
            LoadVector("vectorB.txt", out vectorB);
            if (matrix != null && vectorB != null)
            {
                DisplayMatrix();
                DisplayVector(vectorB, "B");
                int p = 0;
                Console.WriteLine($"\nВыбран столбец p = {p + 1}\n");
                int[] C = new int[rows];
                for (int i = 0; i < rows; i++)
                {
                    try
                    {
                        double sinVal = Math.Sin(matrix[i, p]);
                        C[i] = (int)(vectorB[i] / sinVal);
                        Console.WriteLine($"i={i + 1}: {vectorB[i]}/sin({matrix[i, p]}) = {vectorB[i] / sinVal:F4}, целая часть = {C[i]}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"i={i + 1}: ОШИБКА - {ex.Message}");
                        C[i] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 5");
            Console.WriteLine("B = tg(a[i,p])\n");
            LoadMatrix("matrix.txt");
            if (matrix != null)
            {
                DisplayMatrix();
                int p = 0;
                Console.WriteLine($"\nВыбран столбец p = {p + 1}\n");
                double[] B = new double[rows];
                for (int i = 0; i < rows; i++)
                {
                    try
                    {
                        B[i] = Math.Tan(matrix[i, p]);
                        Console.WriteLine($"i={i + 1}: tg({matrix[i, p]}) = {B[i]:F4}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"i={i + 1}: ОШИБКА - {ex.Message}");
                        B[i] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 6");
            Console.WriteLine("A = целая часть(c[i]/cos(b[i]))\n");
            LoadVector("vectorB.txt", out vectorB);
            LoadVector("vectorC.txt", out vectorC);
            if (vectorB != null && vectorC != null)
            {
                DisplayVector(vectorB, "B");
                DisplayVector(vectorC, "C");
                int[] A = new int[vectorB.Length];
                for (int i = 0; i < vectorB.Length; i++)
                {
                    try
                    {
                        double cosVal = Math.Cos(vectorB[i]);
                        A[i] = (int)(vectorC[i] / cosVal);
                        Console.WriteLine($"i={i + 1}: {vectorC[i]}/cos({vectorB[i]}) = {vectorC[i] / cosVal:F4}, целая часть = {A[i]}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"i={i + 1}: ОШИБКА - {ex.Message}");
                        A[i] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 7");
            Console.WriteLine("B = a[p,j]/a[p-1,j], C = a[p,j]/a[p+1,j]\n");
            LoadMatrix("matrix.txt");
            if (matrix != null)
            {
                DisplayMatrix();
                int p = 1;
                Console.WriteLine($"\nВыбрана строка p = {p + 1}\n");
                int prevRow = (p == 0) ? rows - 1 : p - 1;
                int nextRow = (p == rows - 1) ? 0 : p + 1;
                Console.WriteLine($"Используются строки: p-1 = {prevRow + 1}, p+1 = {nextRow + 1}\n");
                double[] B = new double[cols];
                double[] C = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    try
                    {
                        B[j] = (double)matrix[p, j] / matrix[prevRow, j];
                        C[j] = (double)matrix[p, j] / matrix[nextRow, j];
                        Console.WriteLine($"j={j + 1}: B = {matrix[p, j]}/{matrix[prevRow, j]} = {B[j]:F4}, C = {matrix[p, j]}/{matrix[nextRow, j]} = {C[j]:F4}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"j={j + 1}: ОШИБКА - {ex.Message}");
                        B[j] = 0; C[j] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 8");
            Console.WriteLine("C = целая часть(a[p,j]/sin(b[j]))\n");
            LoadMatrix("matrix.txt");
            LoadVector("vectorB.txt", out vectorB);
            if (matrix != null && vectorB != null)
            {
                DisplayMatrix();
                DisplayVector(vectorB, "B");
                int p = 0;
                Console.WriteLine($"\nВыбрана строка p = {p + 1}\n");
                int[] C = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    try
                    {
                        double sinVal = Math.Sin(vectorB[j]);
                        C[j] = (int)(matrix[p, j] / sinVal);
                        Console.WriteLine($"j={j + 1}: {matrix[p, j]}/sin({vectorB[j]}) = {matrix[p, j] / sinVal:F4}, целая часть = {C[j]}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"j={j + 1}: ОШИБКА - {ex.Message}");
                        C[j] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 9");
            Console.WriteLine("B = sin(a[i,p])/a[i,p]\n");
            LoadMatrix("matrix.txt");
            if (matrix != null)
            {
                DisplayMatrix();
                int p = 0;
                Console.WriteLine($"\nВыбран столбец p = {p + 1}\n");
                double[] B = new double[rows];
                for (int i = 0; i < rows; i++)
                {
                    try
                    {
                        B[i] = Math.Sin(matrix[i, p]) / matrix[i, p];
                        Console.WriteLine($"i={i + 1}: sin({matrix[i, p]})/{matrix[i, p]} = {B[i]:F4}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"i={i + 1}: ОШИБКА - {ex.Message}");
                        B[i] = 0;
                    }
                }
            }

            Console.WriteLine("\n\nВАРИАНТ 10");
            Console.WriteLine("C = целая часть(a[i,p]/sin(b[i]))\n");
            LoadMatrix("matrix.txt");
            LoadVector("vectorB.txt", out vectorB);
            if (matrix != null && vectorB != null)
            {
                DisplayMatrix();
                DisplayVector(vectorB, "B");
                int p = 0;
                Console.WriteLine($"\nВыбран столбец p = {p + 1}\n");
                int[] C = new int[rows];
                for (int i = 0; i < rows; i++)
                {
                    try
                    {
                        double sinVal = Math.Sin(vectorB[i]);
                        C[i] = (int)(matrix[i, p] / sinVal);
                        Console.WriteLine($"i={i + 1}: {matrix[i, p]}/sin({vectorB[i]}) = {matrix[i, p] / sinVal:F4}, целая часть = {C[i]}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"i={i + 1}: ОШИБКА - {ex.Message}");
                        C[i] = 0;
                    }
                }
            }

            Console.WriteLine("   ВЫПОЛНЕНИЕ ВСЕХ ВАРИАНТОВ ЗАВЕРШЕНО");
            Console.ReadKey();
        }

        static void LoadMatrix(string filename)
        {
            try
            {
                string[] lines = File.ReadAllLines(filename);
                rows = lines.Length;
                cols = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                matrix = new int[rows, cols];

                for (int i = 0; i < rows; i++)
                {
                    string[] values = lines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < cols; j++)
                        matrix[i, j] = int.Parse(values[j]);
                }
                Console.WriteLine($"Матрица {rows}x{cols} загружена из {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки матрицы: {ex.Message}");
                matrix = null;
            }
        }

        static void LoadVector(string filename, out int[] vector)
        {
            vector = null;
            try
            {
                string[] values = File.ReadAllText(filename).Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                vector = new int[values.Length];
                for (int i = 0; i < values.Length; i++)
                    vector[i] = int.Parse(values[i]);
                Console.WriteLine($"Вектор размером {vector.Length} загружен из {filename}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки вектора: {ex.Message}");
            }
        }

        static void DisplayMatrix()
        {
            Console.WriteLine("\nМатрица A:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    Console.Write($"{matrix[i, j],6}");
                Console.WriteLine();
            }
        }

        static void DisplayVector(int[] vector, string name)
        {
            Console.WriteLine($"\nВектор {name}:");
            for (int i = 0; i < vector.Length; i++)
                Console.Write($"{vector[i],6}");
            Console.WriteLine();
        }
    }
}