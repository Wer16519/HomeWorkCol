using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace PracticalWork15
{
    public static class MathLibrary
    {
        public static double Add(double a, double b) => a + b;
        public static double Subtract(double a, double b) => a - b;
        public static double Multiply(double a, double b) => a * b;
        public static double Divide(double a, double b) => b != 0 ? a / b : throw new DivideByZeroException();
    }

    public class FileManager
    {
        private readonly string _basePath;

        public FileManager(string basePath = null)
        {
            _basePath = basePath ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        public void WriteFile(string fileName, string content)
        {
            string fullPath = Path.Combine(_basePath, fileName);
            File.WriteAllText(fullPath, content, Encoding.UTF8);
            Console.WriteLine($"✓ Файл '{fullPath}' успешно записан.");
        }

        public string ReadFile(string fileName)
        {
            string fullPath = Path.Combine(_basePath, fileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Файл '{fullPath}' не найден.");

            string content = File.ReadAllText(fullPath, Encoding.UTF8);
            Console.WriteLine($"✓ Файл '{fullPath}' успешно прочитан.");
            return content;
        }

        public void DeleteFile(string fileName)
        {
            string fullPath = Path.Combine(_basePath, fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Console.WriteLine($"✓ Файл '{fullPath}' успешно удалён.");
            }
            else
            {
                Console.WriteLine($"✗ Файл '{fullPath}' не найден для удаления.");
            }
        }
    }

    public class GraphicsDrawer
    {
        private readonly int _width;
        private readonly int _height;
        private char[,] _canvas;

        public GraphicsDrawer(int width, int height)
        {
            _width = width;
            _height = height;
            _canvas = new char[height, width];
            Clear();
        }

        public void Clear()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _canvas[i, j] = ' ';
                }
            }
        }

        public void DrawRectangle(int x, int y, int w, int h, char symbol = '#')
        {
            for (int i = y; i < y + h && i < _height; i++)
            {
                for (int j = x; j < x + w && j < _width; j++)
                {
                    if (i == y || i == y + h - 1 || j == x || j == x + w - 1)
                    {
                        if (i >= 0 && j >= 0)
                            _canvas[i, j] = symbol;
                    }
                }
            }
            Console.WriteLine($"✓ Нарисован прямоугольник (x={x}, y={y}, w={w}, h={h}) символом '{symbol}'");
        }

        public void DrawCircle(int centerX, int centerY, int radius, char symbol = '*')
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    if (x >= 0 && x < _width && y >= 0 && y < _height)
                    {
                        int dx = x - centerX;
                        int dy = y - centerY;
                        if (Math.Abs(dx * dx + dy * dy - radius * radius) <= radius)
                        {
                            _canvas[y, x] = symbol;
                        }
                    }
                }
            }
            Console.WriteLine($"✓ Нарисована окружность (центр x={centerX}, y={centerY}, радиус={radius}) символом '{symbol}'");
        }

        public void Show()
        {
            Console.WriteLine("\n=== ГРАФИЧЕСКИЙ ВЫВОД ===");
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    Console.Write(_canvas[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("=== КОНЕЦ ГРАФИЧЕСКОГО ВЫВОДА ===\n");
        }
    }

    public static class CryptoLibrary
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdef9876543210");

        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }

    public static class DatabaseLibrary
    {
        private const string ConnectionString = "Data Source=demo.db";

        public static void CreateAndQueryDatabase()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("✓ Подключение к базе данных SQLite установлено.");

                    string createTableQuery = @"
                        DROP TABLE IF EXISTS TestTable;
                        CREATE TABLE TestTable (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Value INTEGER NOT NULL
                        );";

                    using (var cmd = new SqliteCommand(createTableQuery, connection))
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("✓ Таблица 'TestTable' создана.");
                    }

                    string insertQuery = "INSERT INTO TestTable (Name, Value) VALUES (@Name, @Value);";
                    for (int i = 1; i <= 3; i++)
                    {
                        using (var cmd = new SqliteCommand(insertQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@Name", $"Record_{i}");
                            cmd.Parameters.AddWithValue("@Value", i * 10);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Console.WriteLine("✓ Данные вставлены в таблицу.");

                    string selectQuery = "SELECT Id, Name, Value FROM TestTable;";
                    using (var cmd = new SqliteCommand(selectQuery, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\nРезультаты запроса:");
                        Console.WriteLine("Id | Name     | Value");
                        Console.WriteLine("---+----------+------");
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["Id"],-2} | {reader["Name"],-8} | {reader["Value"]}");
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка работы с БД: {ex.Message}");
                Console.WriteLine("Для работы с БД установите пакет Microsoft.Data.Sqlite через NuGet.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("ПРАКТИЧЕСКАЯ РАБОТА №15 - ВЫПОЛНЕНИЕ ВСЕХ ЗАДАНИЙ");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();

            Console.WriteLine("--- ЗАДАНИЯ 1-3: Математическая библиотека ---");
            double a = 25, b = 4;
            Console.WriteLine($"{a} + {b} = {MathLibrary.Add(a, b)}");
            Console.WriteLine($"{a} - {b} = {MathLibrary.Subtract(a, b)}");
            Console.WriteLine($"{a} * {b} = {MathLibrary.Multiply(a, b)}");
            Console.WriteLine($"{a} / {b} = {MathLibrary.Divide(a, b)}");
            Console.WriteLine();

            Console.WriteLine("--- ЗАДАНИЯ 4-7: Библиотека для работы с файлами ---");
            var fileManager = new FileManager();
            string testFileName = "test_data.txt";
            string testContent = "Привет, мир! Это тестовый файл для практической работы №15.\nВторая строка файла.";

            fileManager.WriteFile(testFileName, testContent);
            string readContent = fileManager.ReadFile(testFileName);
            Console.WriteLine($"Прочитанное содержимое:\n{readContent}");
            fileManager.DeleteFile(testFileName);
            Console.WriteLine();

            Console.WriteLine("--- ЗАДАНИЯ 8-10: Графическая библиотека ---");
            var drawer = new GraphicsDrawer(50, 25);
            drawer.DrawRectangle(5, 2, 15, 8, '#');
            drawer.DrawRectangle(30, 5, 12, 6, '#');
            drawer.DrawCircle(12, 15, 4, '@');
            drawer.DrawCircle(38, 12, 5, 'O');
            drawer.Show();
            Console.WriteLine();

            Console.WriteLine("--- ЗАДАНИЯ 11-13: Библиотека шифрования ---");
            string originalData = "Секретные данные для шифрования: 12345";
            Console.WriteLine($"Исходные данные: {originalData}");

            string encrypted = CryptoLibrary.Encrypt(originalData);
            Console.WriteLine($"Зашифрованные данные (Base64): {encrypted}");

            string decrypted = CryptoLibrary.Decrypt(encrypted);
            Console.WriteLine($"Расшифрованные данные: {decrypted}");

            bool success = originalData == decrypted;
            Console.WriteLine($"Корректность шифрования: {(success ? "✅ УСПЕШНО" : "❌ ОШИБКА")}");
            Console.WriteLine();

            Console.WriteLine("--- ЗАДАНИЯ 14-15: Библиотека для работы с базами данных ---");
            Console.WriteLine("Используется SQLite (легковесная БД без необходимости установки)");
            DatabaseLibrary.CreateAndQueryDatabase();
            Console.WriteLine();

            Console.WriteLine(new string('=', 60));
            Console.WriteLine("ВСЕ ЗАДАНИЯ УСПЕШНО ВЫПОЛНЕНЫ");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}