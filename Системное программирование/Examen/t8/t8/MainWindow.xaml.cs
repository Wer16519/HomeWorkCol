using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace t8
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenText_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                string text = File.ReadAllText(ofd.FileName);
                richTextBox.Document.Blocks.Clear();
                richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));

                MessageBox.Show("Открыт файл: " + Path.GetFileName(ofd.FileName));
            }
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы (*.*)|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                pictureBox.Source = new BitmapImage(new System.Uri(ofd.FileName));
                MessageBox.Show("Открыт файл: " + Path.GetFileName(ofd.FileName));
            }
        }
    }
}