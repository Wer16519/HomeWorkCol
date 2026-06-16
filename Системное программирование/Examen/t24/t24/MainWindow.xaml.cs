using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace t24
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

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ApplyScale();
        }

        private void ApplyScale()
        {
            if (pictureBox?.Source == null) return;

            int scale = numericUpDown.Value ?? 100;
            pictureBox.Width = originalWidth * scale / 100.0;
            pictureBox.Height = originalHeight * scale / 100.0;
        }
    }
}