using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace t18
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(tbA.Text, out double a) ||
                !double.TryParse(tbB.Text, out double b))
            {
                MessageBox.Show("Введите корректные числа!");
                return;
            }

            if (listOps.SelectedItem is not ListBoxItem item)
            {
                MessageBox.Show("Выберите операцию!");
                return;
            }

            string op = item.Content.ToString();
            double result = 0;

            switch (op)
            {
                case "+": result = a + b; break;
                case "-": result = a - b; break;
                case "*": result = a * b; break;
                case "/":
                    if (b == 0)
                    {
                        MessageBox.Show("Деление на ноль!");
                        return;
                    }
                    result = a / b;
                    break;
            }

            string line = $"Ответ: {a} {op} {b} = {result}";
            richTextBox.AppendText(line + "\r");
            richTextBox.ScrollToEnd();
        }
    }
}