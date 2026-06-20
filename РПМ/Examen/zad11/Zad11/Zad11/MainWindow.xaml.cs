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
using Microsoft.Win32;

namespace Zad11
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

        private void BtnOpenText_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            if (dialog.ShowDialog() == true)
            {
                string text = File.ReadAllText(dialog.FileName);
                rtbText.Document.Blocks.Clear();
                rtbText.Document.Blocks.Add(new Paragraph(new Run(text)));
                MessageBox.Show($"Открыт: {System.IO.Path.GetFileName(dialog.FileName)}");
            }
        }

        private void BtnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp";
            if (dialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgPicture.Source = bitmap;
                MessageBox.Show($"Открыто: {System.IO.Path.GetFileName(dialog.FileName)}");
            }
        }
    }
}