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

namespace pz18
{
    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); }

        private void Slider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (cb == null) return;
            cb.Items.Clear();
            int x = (int)slider.Value;
            for (int i = 1; i <= x; i++)
                cb.Items.Add($"Элемент {i}");
        }

        private void Cb_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (cb.SelectedItem == null) return;
            if (gbPanel.Children.Count >= 5)
            {
                MessageBox.Show("Достигнут лимит 5 элементов!");
                return;
            }
            gbPanel.Children.Add(new RadioButton { Content = cb.SelectedItem.ToString() });
        }
    }
}