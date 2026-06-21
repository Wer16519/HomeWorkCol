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

namespace T23
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();
        Contact selected;

        public MainWindow()
        {
            InitializeComponent();
            lstContacts.ItemsSource = contacts;
            lstSearch.ItemsSource = contacts;
            contacts.Add(new Contact("Иван", "+79991234567", "ivan@mail.ru"));
            contacts.Add(new Contact("Мария", "+79882345678", "maria@mail.ru"));
        }

        public class Contact
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public Contact(string n, string p, string e) { Name = n; Phone = p; Email = e; }
            public string DisplayInfo => $"{Name} - {Phone}";
        }

        private void LstContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = lstContacts.SelectedItem as Contact;
            if (selected != null)
            {
                txtName.Text = selected.Name;
                txtPhone.Text = selected.Phone;
                txtEmail.Text = selected.Email;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
                contacts.Add(new Contact(txtName.Text, txtPhone.Text, txtEmail.Text));
            Clear();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                selected.Name = txtName.Text;
                selected.Phone = txtPhone.Text;
                selected.Email = txtEmail.Text;
                Clear();
                lstContacts.Items.Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                contacts.Remove(selected);
                Clear();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                lstSearch.ItemsSource = contacts;
            else
                lstSearch.ItemsSource = contacts.Where(x =>
                    x.Name.ToLower().Contains(txtSearch.Text.ToLower()) ||
                    x.Phone.Contains(txtSearch.Text)).ToList();
        }

        private void Clear() { txtName.Text = ""; txtPhone.Text = ""; txtEmail.Text = ""; selected = null; }
    }
}