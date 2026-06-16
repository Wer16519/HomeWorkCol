using System.Windows;
using System.Windows.Controls;

namespace t4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddToCb1_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBox1.SelectedItems)
            {
                ListBoxItem lbi = item as ListBoxItem;
                cb1.Items.Add(lbi.Content.ToString());
            }
            UpdateCount();
        }

        private void AddToCb2_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listBox1.SelectedItems)
            {
                ListBoxItem lbi = item as ListBoxItem;
                cb2.Items.Add(lbi.Content.ToString());
            }
            UpdateCount();
        }

        private void UpdateCount()
        {
            lblCount.Content = $"cB1: {cb1.Items.Count} | cB2: {cb2.Items.Count}";
        }
    }
}