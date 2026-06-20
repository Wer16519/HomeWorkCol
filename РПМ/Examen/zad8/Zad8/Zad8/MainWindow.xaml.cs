using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zad8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> items = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            lbItems.ItemsSource = items;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNewItem.Text))
            {
                items.Add(txtNewItem.Text);
                txtNewItem.Clear();
                RefreshListBox();
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(dialog.FileName);
                    foreach (string line in lines)
                        if (!string.IsNullOrWhiteSpace(line))
                            items.Add(line.Trim());
                    RefreshListBox();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            var selected = lbItems.SelectedItems;
            if (selected.Count == 0) return;

            foreach (var item in selected)
            {
                cbItems.Items.Add(item.ToString());
                items.Remove(item.ToString());
            }
            RefreshListBox();
            lblCount.Content = $"Количество: {cbItems.Items.Count}";
        }

        private void RefreshListBox()
        {
            lbItems.ItemsSource = null;
            lbItems.ItemsSource = items;
        }
    }
}