using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pz15
{
    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtA.Text, out double a) ||
                !double.TryParse(txtB.Text, out double b))
            { MessageBox.Show("Введите числа!"); return; }

            ListBoxItem item = lstOp.SelectedItem as ListBoxItem;
            if (item == null)
            { MessageBox.Show("Выберите операцию!"); return; }

            string op = item.Content.ToString();
            double res = 0;
            switch (op)
            {
                case "+": res = a + b; break;
                case "-": res = a - b; break;
                case "*": res = a * b; break;
                case "/":
                    if (b == 0) { MessageBox.Show("Деление на 0!"); return; }
                    res = a / b; break;
            }
            rtbResult.AppendText($"Ответ: {a} {op} {b} = {res}\r");
        }
    }
}