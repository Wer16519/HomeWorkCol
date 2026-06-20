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
using System.IO;

namespace Zad10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentFile = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            if (dialog.ShowDialog() == true)
            {
                currentFile = dialog.FileName;
                string text = File.ReadAllText(currentFile);
                rtbText.Document.Blocks.Clear();
                rtbText.Document.Blocks.Add(new Paragraph(new Run(text)));
                Title = $"Текстовый редактор - {System.IO.Path.GetFileName(currentFile)}";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFile))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                if (dialog.ShowDialog() == true)
                    currentFile = dialog.FileName;
                else
                    return;
            }

            string text = new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text;
            File.WriteAllText(currentFile, text.TrimEnd('\r', '\n'));
            MessageBox.Show("Сохранено!");
        }
    }
}