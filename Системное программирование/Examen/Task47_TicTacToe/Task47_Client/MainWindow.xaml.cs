using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Task47_Client
{
    public partial class MainWindow : Window
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private Button[,] _buttons = new Button[3, 3];
        private char _myPlayer = '\0';
        private bool _myTurn = false;
        private bool _gameOver = false;
        private bool _isConnected = false;
        private Thread _receiveThread;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            ClearBoard();
        }

        private void InitializeBoard()
        {
            _buttons[0, 0] = Btn00;
            _buttons[0, 1] = Btn01;
            _buttons[0, 2] = Btn02;
            _buttons[1, 0] = Btn10;
            _buttons[1, 1] = Btn11;
            _buttons[1, 2] = Btn12;
            _buttons[2, 0] = Btn20;
            _buttons[2, 1] = Btn21;
            _buttons[2, 2] = Btn22;
        }

        private void ClearBoard()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    _buttons[i, j].Content = "";
                    _buttons[i, j].IsEnabled = false;
                    _buttons[i, j].Background = System.Windows.Media.Brushes.White;
                }
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_isConnected) return;

            try
            {
                LblStatus.Content = "Подключение...";
                _client = new TcpClient();
                await _client.ConnectAsync("127.0.0.1", 8888);
                _stream = _client.GetStream();
                _isConnected = true;

                LblStatus.Content = "Подключено! Ожидание игроков...";
                BtnConnect.IsEnabled = false;
                BtnDisconnect.IsEnabled = true;

                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();
            }
            catch
            {
                MessageBox.Show("Ошибка подключения! Запустите сервер.", "Ошибка");
                LblStatus.Content = "Ошибка подключения";
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[1024];

                while (_isConnected)
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Dispatcher.Invoke(() => ProcessMessage(message));
                }
            }
            catch { }
        }

        private void ProcessMessage(string message)
        {
            if (message == "X")
            {
                _myPlayer = 'X';
                LblPlayer.Content = "Вы: X";
                LblStatus.Content = "Ожидание второго игрока...";
            }
            else if (message == "O")
            {
                _myPlayer = 'O';
                LblPlayer.Content = "Вы: O";
                LblStatus.Content = "Ожидание хода...";
            }
            else if (message == "start")
            {
                ClearBoard();
                _gameOver = false;
                _myTurn = (_myPlayer == 'X');
                LblStatus.Content = _myTurn ? "Ваш ход!" : "Ожидание хода...";
                EnableButtons();
            }
            else if (message.StartsWith("move,"))
            {
                string[] parts = message.Split(',');
                int row = int.Parse(parts[1]);
                int col = int.Parse(parts[2]);
                char player = parts[3][0];

                _buttons[row, col].Content = player.ToString();
                _buttons[row, col].IsEnabled = false;

                if (player == _myPlayer)
                {
                    _myTurn = false;
                    LblStatus.Content = "Ожидание хода...";
                }
                else
                {
                    _myTurn = !_gameOver;
                    LblStatus.Content = _myTurn ? "Ваш ход!" : "Ожидание хода...";
                }
                EnableButtons();
            }
            else if (message.StartsWith("win,"))
            {
                _gameOver = true;
                _myTurn = false;
                char winner = message.Split(',')[1][0];
                LblStatus.Content = $"Игрок {winner} ПОБЕДИЛ!";
                LblInfo.Content = "Нажмите 'Новая игра'";
                EnableButtons();
            }
            else if (message == "draw")
            {
                _gameOver = true;
                _myTurn = false;
                LblStatus.Content = "НИЧЬЯ!";
                LblInfo.Content = "Нажмите 'Новая игра'";
                EnableButtons();
            }
            else if (message == "restart")
            {
                ClearBoard();
                _gameOver = false;
                _myTurn = (_myPlayer == 'X');
                LblStatus.Content = _myTurn ? "Ваш ход!" : "Ожидание хода...";
                LblInfo.Content = "";
                EnableButtons();
            }
            else if (message.StartsWith("turn,"))
            {
                char player = message.Split(',')[1][0];
                _myTurn = (player == _myPlayer);
                LblStatus.Content = _myTurn ? "Ваш ход!" : $"Ход: {player}";
                EnableButtons();
            }
            else
            {
                LblStatus.Content = message;
            }
        }

        private void EnableButtons()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (_buttons[i, j].Content.ToString() == "")
                    {
                        _buttons[i, j].IsEnabled = _myTurn && !_gameOver && _isConnected;
                    }
                }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!_myTurn || _gameOver || !_isConnected) return;

            Button button = sender as Button;
            if (button == null || button.Content.ToString() != "") return;

            int row = -1, col = -1;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (_buttons[i, j] == button)
                    {
                        row = i;
                        col = j;
                        break;
                    }

            if (row == -1) return;

            try
            {
                string move = $"{row},{col}";
                byte[] data = Encoding.UTF8.GetBytes(move);
                _stream.Write(data, 0, data.Length);

                button.IsEnabled = false;
                _myTurn = false;
                LblStatus.Content = "Ожидание ответа...";
            }
            catch
            {
                MessageBox.Show("Ошибка отправки хода", "Ошибка");
            }
        }

        private async void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConnected) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes("restart");
                await _stream.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                MessageBox.Show("Ошибка отправки запроса", "Ошибка");
            }
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            _isConnected = false;
            _stream?.Close();
            _client?.Close();

            ClearBoard();
            LblStatus.Content = "Отключено";
            LblPlayer.Content = "Вы: не подключены";
            LblInfo.Content = "";
            BtnConnect.IsEnabled = true;
            BtnDisconnect.IsEnabled = false;
            _myTurn = false;
            _gameOver = false;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Disconnect();
            base.OnClosing(e);
        }
    }
}