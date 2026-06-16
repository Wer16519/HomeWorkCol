using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Task47_Server
{
    class Program
    {
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static char[,] _board = new char[3, 3];
        private static char _currentPlayer = 'X';
        private static bool _gameOver = false;
        private static int _playerCount = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("       СЕРВЕР КРЕСТИКИ-НОЛИКИ");
            Console.WriteLine("========================================\n");

            string ip = GetLocalIP();
            Console.WriteLine($"IP адрес сервера: {ip}");
            Console.WriteLine("Порт: 8888");
            Console.WriteLine("\nОжидание подключения игроков...\n");

            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _playerCount++;
                _clients.Add(client);

                Console.WriteLine($"[Игрок {_playerCount}] Подключился");

                if (_playerCount == 1)
                {
                    await Send(client, "X");
                    await Send(client, "Ожидание второго игрока...");
                }
                else if (_playerCount == 2)
                {
                    await Send(_clients[0], "start");
                    await Send(_clients[0], "Ваш ход!");
                    await Send(_clients[1], "O");
                    await Send(_clients[1], "Ожидание хода соперника...");
                    ClearBoard();
                    Console.WriteLine("Игра началась!");
                }

                int playerNum = _playerCount;
                Task.Run(() => HandleClient(client, playerNum));
            }
        }

        private static void ClearBoard()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    _board[i, j] = '\0';
            _currentPlayer = 'X';
            _gameOver = false;
        }

        private static async Task HandleClient(TcpClient client, int playerNum)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (client.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    string[] parts = message.Split(',');

                    if (parts.Length == 2 && int.TryParse(parts[0], out int row) && int.TryParse(parts[1], out int col))
                    {
                        char player = (playerNum == 1) ? 'X' : 'O';

                        if (_gameOver)
                        {
                            await Send(client, "Игра окончена! Нажмите 'Новая игра'");
                            continue;
                        }

                        if (_currentPlayer != player)
                        {
                            await Send(client, "Сейчас не ваш ход!");
                            continue;
                        }

                        if (row < 0 || row > 2 || col < 0 || col > 2 || _board[row, col] != '\0')
                        {
                            await Send(client, "Неверный ход!");
                            continue;
                        }

                        _board[row, col] = player;
                        await Broadcast($"move,{row},{col},{player}");
                        Console.WriteLine($"[Ход] Игрок {player}: {row},{col}");

                        if (CheckWin(player))
                        {
                            _gameOver = true;
                            await Broadcast($"win,{player}");
                            Console.WriteLine($"Игрок {player} ПОБЕДИЛ!");
                            continue;
                        }

                        if (CheckDraw())
                        {
                            _gameOver = true;
                            await Broadcast("draw");
                            Console.WriteLine("НИЧЬЯ!");
                            continue;
                        }

                        _currentPlayer = (_currentPlayer == 'X') ? 'O' : 'X';
                        await Broadcast($"turn,{_currentPlayer}");
                    }
                    else if (message == "restart")
                    {
                        if (!_gameOver)
                        {
                            await Send(client, "Игра еще не окончена!");
                            continue;
                        }

                        ClearBoard();
                        await Broadcast("restart");
                        await Send(_clients[0], "Ваш ход!");
                        await Send(_clients[1], "Ожидание хода соперника...");
                        Console.WriteLine("Игра перезапущена!");
                    }
                }
            }
            catch { }
        }

        private static bool CheckWin(char player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_board[i, 0] == player && _board[i, 1] == player && _board[i, 2] == player) return true;
                if (_board[0, i] == player && _board[1, i] == player && _board[2, i] == player) return true;
            }
            if (_board[0, 0] == player && _board[1, 1] == player && _board[2, 2] == player) return true;
            if (_board[0, 2] == player && _board[1, 1] == player && _board[2, 0] == player) return true;
            return false;
        }

        private static bool CheckDraw()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (_board[i, j] == '\0') return false;
            return true;
        }

        private static async Task Broadcast(string message)
        {
            foreach (var client in _clients)
            {
                await Send(client, message);
            }
        }

        private static async Task Send(TcpClient client, string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }
            catch { }
        }

        private static string GetLocalIP()
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }
    }
}