using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Data.SqlClient;

namespace MyLibrary
{
    public class Calculator
    {
        public static double Add(double a, double b)
        {
            return a + b;
        }
    }

    public class MathFunctions
    {
        public static double SquareRoot(double number)
        {
            if (number < 0)
                throw new ArgumentException("Число не может быть отрицательным");
            return Math.Sqrt(number);
        }
    }

    public class MatrixOperations
    {
        public static double[,] AddMatrices(double[,] matrix1, double[,] matrix2)
        {
            int rows = matrix1.GetLength(0);
            int cols = matrix1.GetLength(1);

            if (rows != matrix2.GetLength(0) || cols != matrix2.GetLength(1))
                throw new ArgumentException("Размеры матриц должны совпадать");

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = matrix1[i, j] + matrix2[i, j];

            return result;
        }

        public static double[,] MultiplyMatrixByScalar(double[,] matrix, double scalar)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = matrix[i, j] * scalar;

            return result;
        }

        public static string PrintMatrix(double[,] matrix)
        {
            StringBuilder sb = new StringBuilder();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    sb.Append($"{matrix[i, j],8:F2}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    public class Encryption
    {
        public static string Encrypt(string text, int key)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    result.Append((char)((c + key - offset) % 26 + offset));
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public static string Decrypt(string text, int key)
        {
            return Encrypt(text, 26 - (key % 26));
        }
    }

    public class RandomGenerator
    {
        private static Random random = new Random();

        public static int GenerateRandomInt(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        public static double GenerateRandomDouble(double min, double max)
        {
            return min + (random.NextDouble() * (max - min));
        }

        public static int[] GenerateRandomArray(int size, int min, int max)
        {
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
                array[i] = GenerateRandomInt(min, max);
            return array;
        }
    }

    public class StringOperations
    {
        public static int FindSubstring(string text, string substring)
        {
            return text.IndexOf(substring, StringComparison.OrdinalIgnoreCase);
        }

        public static string ReplaceChar(string text, char oldChar, char newChar)
        {
            return text.Replace(oldChar, newChar);
        }

        public static string ConcatenateStrings(params string[] strings)
        {
            return string.Concat(strings);
        }
    }

    public class FileSystemOperations
    {
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void DeleteDirectory(string path, bool recursive = true)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
        }

        public static string ReadFile(string filePath)
        {
            if (File.Exists(filePath))
                return File.ReadAllText(filePath, Encoding.UTF8);
            throw new FileNotFoundException("Файл не найден", filePath);
        }

        public static void WriteFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        public static string[] GetFilesInDirectory(string directoryPath, string searchPattern = "*.*")
        {
            if (Directory.Exists(directoryPath))
                return Directory.GetFiles(directoryPath, searchPattern);
            return new string[0];
        }
    }

    public class ImageProcessing
    {
        public struct Pixel
        {
            public byte R, G, B, A;
            public Pixel(byte r, byte g, byte b, byte a = 255) { R = r; G = g; B = b; A = a; }
        }

        public class SimpleImage
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            private Pixel[,] pixels;

            public SimpleImage(int width, int height)
            {
                Width = width;
                Height = height;
                pixels = new Pixel[height, width];
            }

            public void SetPixel(int x, int y, Pixel pixel)
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                    pixels[y, x] = pixel;
            }

            public Pixel GetPixel(int x, int y)
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                    return pixels[y, x];
                return new Pixel(0, 0, 0);
            }
        }

        public static SimpleImage AdjustBrightness(SimpleImage image, int brightness)
        {
            SimpleImage result = new SimpleImage(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    Pixel p = image.GetPixel(x, y);
                    int r = Math.Min(255, Math.Max(0, p.R + brightness));
                    int g = Math.Min(255, Math.Max(0, p.G + brightness));
                    int b = Math.Min(255, Math.Max(0, p.B + brightness));
                    result.SetPixel(x, y, new Pixel((byte)r, (byte)g, (byte)b, p.A));
                }
            return result;
        }

        public static SimpleImage AdjustContrast(SimpleImage image, double contrast)
        {
            SimpleImage result = new SimpleImage(image.Width, image.Height);
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    Pixel p = image.GetPixel(x, y);
                    double r = ((p.R / 255.0 - 0.5) * contrast + 0.5) * 255;
                    double g = ((p.G / 255.0 - 0.5) * contrast + 0.5) * 255;
                    double b = ((p.B / 255.0 - 0.5) * contrast + 0.5) * 255;
                    r = Math.Min(255, Math.Max(0, r));
                    g = Math.Min(255, Math.Max(0, g));
                    b = Math.Min(255, Math.Max(0, b));
                    result.SetPixel(x, y, new Pixel((byte)r, (byte)g, (byte)b, p.A));
                }
            return result;
        }

        public static SimpleImage AdjustSaturation(SimpleImage image, double saturation)
        {
            SimpleImage result = new SimpleImage(image.Width, image.Height);
            saturation = saturation / 100.0;

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    Pixel p = image.GetPixel(x, y);
                    double gray = (p.R + p.G + p.B) / 3.0;
                    int r = (int)(gray + (p.R - gray) * saturation);
                    int g = (int)(gray + (p.G - gray) * saturation);
                    int b = (int)(gray + (p.B - gray) * saturation);
                    r = Math.Min(255, Math.Max(0, r));
                    g = Math.Min(255, Math.Max(0, g));
                    b = Math.Min(255, Math.Max(0, b));
                    result.SetPixel(x, y, new Pixel((byte)r, (byte)g, (byte)b, p.A));
                }
            return result;
        }

        public static SimpleImage CreateTestImage(int width, int height)
        {
            SimpleImage image = new SimpleImage(width, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    byte r = (byte)((double)x / width * 255);
                    byte g = (byte)((double)y / height * 255);
                    byte b = (byte)((double)(x + y) / (width + height) * 255);
                    image.SetPixel(x, y, new Pixel(r, g, b));
                }
            return image;
        }

        public static string ImageToString(SimpleImage image, int maxWidth = 40, int maxHeight = 20)
        {
            StringBuilder sb = new StringBuilder();
            int w = Math.Min(image.Width, maxWidth);
            int h = Math.Min(image.Height, maxHeight);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Pixel p = image.GetPixel(x, y);
                    char c = (char)('█' - ((p.R + p.G + p.B) / 3 / 8));
                    sb.Append(c);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    public class DatabaseOperations
    {
        private string connectionString;

        public DatabaseOperations(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string sqlQuery)
        {
            Console.WriteLine($"Выполнение запроса: {sqlQuery}");
            Console.WriteLine($"Строка подключения: {connectionString}");
            Console.WriteLine("Примечание: Для реальной работы с БД установите System.Data.SqlClient");
            return new DataTable();
        }

        public int ExecuteNonQuery(string sqlCommand)
        {
            Console.WriteLine($"Выполнение команды: {sqlCommand}");
            return 0;
        }

        public object ExecuteScalar(string sqlQuery)
        {
            Console.WriteLine($"Выполнение скалярного запроса: {sqlQuery}");
            return null;
        }
    }

    public class GaussianElimination
    {
        public static double[] SolveSystem(double[,] matrix, double[] constants)
        {
            int n = constants.Length;
            double[,] augmented = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    augmented[i, j] = matrix[i, j];
                augmented[i, n] = constants[i];
            }

            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                double maxVal = Math.Abs(augmented[i, i]);
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(augmented[k, i]) > maxVal)
                    {
                        maxVal = Math.Abs(augmented[k, i]);
                        maxRow = k;
                    }
                }

                if (maxRow != i)
                {
                    for (int j = i; j <= n; j++)
                    {
                        double temp = augmented[i, j];
                        augmented[i, j] = augmented[maxRow, j];
                        augmented[maxRow, j] = temp;
                    }
                }

                double pivot = augmented[i, i];
                if (Math.Abs(pivot) < 1e-10)
                    throw new InvalidOperationException("Система не имеет единственного решения");

                for (int j = i; j <= n; j++)
                    augmented[i, j] /= pivot;

                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        double factor = augmented[k, i];
                        for (int j = i; j <= n; j++)
                            augmented[k, j] -= factor * augmented[i, j];
                    }
                }
            }

            double[] solution = new double[n];
            for (int i = 0; i < n; i++)
                solution[i] = augmented[i, n];

            return solution;
        }

        public static double[] Solve3x3System(double a11, double a12, double a13, double b1,
                                              double a21, double a22, double a23, double b2,
                                              double a31, double a32, double a33, double b3)
        {
            double[,] matrix = { { a11, a12, a13 }, { a21, a22, a23 }, { a31, a32, a33 } };
            double[] constants = { b1, b2, b3 };
            return SolveSystem(matrix, constants);
        }
    }
}