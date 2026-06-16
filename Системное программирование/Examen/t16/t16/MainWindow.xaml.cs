using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace t16
{
    public partial class MainWindow : Window
    {
        private double originalWidth;
        private double originalHeight;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (ofd.ShowDialog() == true)
            {
                BitmapImage bmp = new BitmapImage(new Uri(ofd.FileName));
                pictureBox.Source = bmp;

                originalWidth = bmp.PixelWidth;
                originalHeight = bmp.PixelHeight;

                ApplyScale();
            }
        }

        private void Scale_Changed(object sender, TextChangedEventArgs e)
        {
            ApplyScale();
        }

        private void ApplyScale()
        {
            if (pictureBox?.Source == null) return;

            if (int.TryParse(tbScale.Text, out int scale) && scale > 0)
            {
                pictureBox.Width = originalWidth * scale / 100.0;
                pictureBox.Height = originalHeight * scale / 100.0;
            }
        }
    }
}