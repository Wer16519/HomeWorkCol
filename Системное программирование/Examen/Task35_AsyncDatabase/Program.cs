using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace Task35_AsyncDatabase
{
    public class AsyncDatabaseHelper : IDisposable
    {
        private SQLiteConnection _connection;
        private bool _disposed;

        public AsyncDatabaseHelper(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
            Console.WriteLine("[База данных] Инициализация подключения");
        }

        public async Task OpenConnectionAsync()
        {
            try
            {
                await _connection.OpenAsync();
                Console.WriteLine("[База данных] Подключение открыто");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Не удалось открыть подключение: {ex.Message}");
                throw;
            }
        }

        public async Task CreateTableAsync()
        {
            try
            {
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Age INTEGER,
                        Email TEXT,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";

                using (SQLiteCommand command = new SQLiteCommand(createTableQuery, _connection))
                {
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine("[База данных] Таблица Users создана/проверена");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Создание таблицы: {ex.Message}");
            }
        }

        public async Task InsertUserAsync(string name, int age, string email)
        {
            try
            {
                string query = "INSERT INTO Users (Name, Age, Email) VALUES (@Name, @Age, @Email)";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Email", email);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"[Запись] Добавлен пользователь: {name} (возраст: {age})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Вставка данных: {ex.Message}");
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = new List<User>();
            try
            {
                string query = "SELECT Id, Name, Age, Email, CreatedAt FROM Users ORDER BY Id";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Age = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                            Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4)
                        });
                    }
                }
                Console.WriteLine($"[Чтение] Получено {users.Count} пользователей");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Чтение данных: {ex.Message}");
            }
            return users;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                string query = "SELECT Id, Name, Age, Email, CreatedAt FROM Users WHERE Id = @Id";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Age = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Получение пользователя: {ex.Message}");
            }
            return null;
        }

        public async Task UpdateUserAsync(int id, string name, int age, string email)
        {
            try
            {
                string query = "UPDATE Users SET Name = @Name, Age = @Age, Email = @Email WHERE Id = @Id";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Email", email);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"[Обновление] Обновлен пользователь с Id={id}, строк затронуто: {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Обновление данных: {ex.Message}");
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                string query = "DELETE FROM Users WHERE Id = @Id";
                using (SQLiteCommand command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"[Удаление] Удален пользователь с Id={id}, строк затронуто: {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка] Удаление данных: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                    Console.WriteLine("[База данных] Подключение закрыто");
                }
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Имя: {Name}, Возраст: {Age}, Email: {Email}, Создан: {CreatedAt:yyyy-MM-dd HH:mm}";
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Асинхронная работа с базой данных ===\n");

            string dbPath = "test.db";
            string connectionString = $"Data Source={dbPath};Version=3;";

            using (AsyncDatabaseHelper db = new AsyncDatabaseHelper(connectionString))
            {
                try
                {
                    await db.OpenConnectionAsync();

                    await db.CreateTableAsync();

                    Console.WriteLine("\n--- Добавление пользователей ---");
                    await db.InsertUserAsync("Иван Петров", 25, "ivan@mail.ru");
                    await db.InsertUserAsync("Мария Иванова", 30, "maria@mail.ru");
                    await db.InsertUserAsync("Петр Сидоров", 35, "petr@mail.ru");

                    Console.WriteLine("\n--- Получение всех пользователей ---");
                    List<User> users = await db.GetUsersAsync();
                    foreach (User user in users)
                    {
                        Console.WriteLine(user);
                    }

                    Console.WriteLine("\n--- Получение пользователя по Id ---");
                    User userById = await db.GetUserByIdAsync(1);
                    if (userById != null)
                    {
                        Console.WriteLine(userById);
                    }

                    Console.WriteLine("\n--- Обновление пользователя ---");
                    await db.UpdateUserAsync(1, "Иван Петров (обновлен)", 26, "ivan.new@mail.ru");

                    Console.WriteLine("\n--- Удаление пользователя ---");
                    await db.DeleteUserAsync(3);

                    Console.WriteLine("\n--- Пользователи после обновлений ---");
                    users = await db.GetUsersAsync();
                    foreach (User user in users)
                    {
                        Console.WriteLine(user);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Ошибка] {ex.Message}");
                }
            }

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}