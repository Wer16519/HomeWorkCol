using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string HistoryFile = "history.txt";
        private static readonly Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void ShuffleBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] array = ParseInput(InputBox.Text);
                if (array.Length == 0)
                {
                    MessageBox.Show("Введите хотя бы один элемент массива.");
                    return;
                }

                int[] shuffled = Shuffle(array);
                string result = string.Join(", ", shuffled);
                ResultBox.Text = $"Результат: [{result}]";

                string record = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | [{string.Join(", ", array)}] -> [{result}]";
                SaveToHistory(record);
                HistoryList.Items.Add(record);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
        }

        private int[] ParseInput(string text)
        {
            return text.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(int.Parse)
                       .ToArray();
        }

        private int[] Shuffle(int[] source)
        {
            int[] arr = (int[])source.Clone();
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = Rnd.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
            return arr;
        }

        private void SaveToHistory(string record)
        {
            File.AppendAllText(HistoryFile, record + Environment.NewLine);
        }

        private void LoadHistory()
        {
            if (!File.Exists(HistoryFile)) return;
            foreach (var line in File.ReadAllLines(HistoryFile))
                HistoryList.Items.Add(line);
        }
    }
}