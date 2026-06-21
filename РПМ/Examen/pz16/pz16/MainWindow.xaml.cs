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
using System.IO;
using Microsoft.Win32;

namespace pz16
{
    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Текстовый файл|*.txt" };
            if (dlg.ShowDialog() == true)
            {
                string text = File.ReadAllText(dlg.FileName);
                rtb.Document.Blocks.Clear();
                rtb.Document.Blocks.Add(new Paragraph(new Run(text)));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "Текстовый файл|*.txt" };
            if (dlg.ShowDialog() == true)
            {
                var range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                File.WriteAllText(dlg.FileName, range.Text);
            }
        }
    }
}