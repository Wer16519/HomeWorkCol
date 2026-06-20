using System.Globalization;
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

namespace task2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalcBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseNumber(Num1Box.Text, out double a))
            {
                ShowError("Введите корректное первое число.");
                return;
            }

            if (!TryParseNumber(Num2Box.Text, out double b))
            {
                ShowError("Введите корректное второе число.");
                return;
            }

            string op = ((ComboBoxItem)OpBox.SelectedItem).Content.ToString();

            try
            {
                double result = Calculate(a, b, op);
                ResultBox.Foreground = System.Windows.Media.Brushes.DarkGreen;
                ResultBox.Text = $"Результат: {result}";
            }
            catch (DivideByZeroException)
            {
                ShowError("Деление на ноль невозможно.");
            }
            catch (Exception ex)
            {
                ShowError("Ошибка: " + ex.Message);
            }
        }

        private bool TryParseNumber(string text, out double value)
        {
            return double.TryParse(text?.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private double Calculate(double a, double b, string op)
        {
            return op switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                "/" => b == 0 ? throw new DivideByZeroException() : a / b,
                _ => throw new InvalidOperationException("Неизвестная операция")
            };
        }

        private void ShowError(string msg)
        {
            ResultBox.Foreground = System.Windows.Media.Brushes.Red;
            ResultBox.Text = msg;
        }
    }
}