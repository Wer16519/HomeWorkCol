using Microsoft.Win32;
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

namespace pz19
{
    public partial class MainWindow : Window
    {
        private BitmapImage _bitmap;
        public MainWindow() { InitializeComponent(); }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Изображения|*.jpg;*.png;*.bmp" };
            if (dlg.ShowDialog() == true)
            {
                _bitmap = new BitmapImage(new System.Uri(dlg.FileName));
                img.Source = _bitmap;
                ApplyScale();
            }
        }

        private void Scale_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (lblPct != null) lblPct.Content = $"{(int)sldScale.Value}%";
            ApplyScale();
        }

        private void ApplyScale()
        {
            if (_bitmap == null) return;
            double k = sldScale.Value / 100.0;
            img.Width = _bitmap.PixelWidth * k;
            img.Height = _bitmap.PixelHeight * k;
        }
    }
}