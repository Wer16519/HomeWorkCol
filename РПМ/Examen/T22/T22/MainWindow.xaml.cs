using System.Collections.ObjectModel;
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

namespace T22
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Task> tasks = new ObservableCollection<Task>();
        Task selected;

        public MainWindow()
        {
            InitializeComponent();
            lstTasks.ItemsSource = tasks;
            tasks.Add(new Task("Сдать проект", "Высокий", "Запланировано"));
            tasks.Add(new Task("Купить продукты", "Средний", "В процессе"));
        }

        public class Task
        {
            public string Title { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public Task(string t, string p, string s) { Title = t; Priority = p; Status = s; }
            public string DisplayInfo => $"{Title} ({Priority}) - {Status}";
        }

        private void LstTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = lstTasks.SelectedItem as Task;
            if (selected != null)
            {
                txtTitle.Text = selected.Title;
                cmbPriority.SelectedItem = cmbPriority.Items.OfType<ComboBoxItem>()
                    .FirstOrDefault(x => x.Content.ToString() == selected.Priority);
                cmbStatus.SelectedItem = cmbStatus.Items.OfType<ComboBoxItem>()
                    .FirstOrDefault(x => x.Content.ToString() == selected.Status);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTitle.Text))
                tasks.Add(new Task(txtTitle.Text, GetPriority(), GetStatus()));
            Clear();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                selected.Title = txtTitle.Text;
                selected.Priority = GetPriority();
                selected.Status = GetStatus();
                Clear();
                lstTasks.Items.Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                tasks.Remove(selected);
                Clear();
            }
        }

        private void BtnSort_Click(object sender, RoutedEventArgs e)
        {
            var sorted = tasks.OrderByDescending(t =>
                t.Priority == "Высокий" ? 3 : t.Priority == "Средний" ? 2 : 1).ToList();
            tasks.Clear();
            foreach (var t in sorted) tasks.Add(t);
        }

        private string GetPriority() => (cmbPriority.SelectedItem as ComboBoxItem)?.Content.ToString();
        private string GetStatus() => (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
        private void Clear() { txtTitle.Text = ""; selected = null; }
    }
}