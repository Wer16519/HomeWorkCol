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
using System.Collections.ObjectModel;

namespace pz20
{
    public class Employee
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Display => $"{Name} — {Position}";
    }

    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Employee> _emps = new ObservableCollection<Employee>();

        public MainWindow()
        {
            InitializeComponent();
            lst.ItemsSource = _emps;
            _emps.Add(new Employee { Name = "Иванов", Position = "Менеджер", Department = "Продажи" });
            _emps.Add(new Employee { Name = "Петров", Position = "Программист", Department = "ИТ" });
        }

        private void Lst_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lst.SelectedItem is Employee emp)
            {
                txtName.Text = emp.Name;
                txtPos.Text = emp.Position;
                txtDept.Text = emp.Department;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            _emps.Add(new Employee { Name = txtName.Text, Position = txtPos.Text, Department = txtDept.Text });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (lst.SelectedItem is Employee emp)
            {
                emp.Name = txtName.Text;
                emp.Position = txtPos.Text;
                emp.Department = txtDept.Text;
                lst.Items.Refresh();
            }
        }

        private void Del_Click(object sender, RoutedEventArgs e)
        {
            if (lst.SelectedItem is Employee emp) _emps.Remove(emp);
        }
    }
}