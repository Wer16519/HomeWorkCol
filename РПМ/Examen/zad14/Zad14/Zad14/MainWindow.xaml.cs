using Microsoft.Win32;
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

namespace Zad14
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage originalImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp";
            if (dialog.ShowDialog() == true)
            {
                originalImage = new BitmapImage();
                originalImage.BeginInit();
                originalImage.UriSource = new Uri(dialog.FileName);
                originalImage.CacheOption = BitmapCacheOption.OnLoad;
                originalImage.EndInit();

                imgPicture.Source = originalImage;
                ApplyScale();
            }
        }

        private void SliderScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (originalImage != null)
            {
                ApplyScale();
                lblPercent.Text = $"{(int)sliderScale.Value}%";
            }
        }

        private void ApplyScale()
        {
            double scale = sliderScale.Value / 100;
            viewboxImage.Width = originalImage.PixelWidth * scale;
            viewboxImage.Height = originalImage.PixelHeight * scale;
        }
    }
}