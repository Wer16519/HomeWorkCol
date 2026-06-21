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

namespace T24
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Book> books = new ObservableCollection<Book>();
        Book selected;

        public MainWindow()
        {
            InitializeComponent();
            lstBooks.ItemsSource = books;
            lstSearch.ItemsSource = books;
            books.Add(new Book("Толстой", "Война и мир", "Роман"));
            books.Add(new Book("Достоевский", "Преступление", "Роман"));
        }

        public class Book
        {
            public string Author { get; set; }
            public string Title { get; set; }
            public string Genre { get; set; }
            public Book(string a, string t, string g) { Author = a; Title = t; Genre = g; }
            public string DisplayInfo => $"{Author} - {Title}";
        }

        private void LstBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = lstBooks.SelectedItem as Book;
            if (selected != null)
            {
                txtAuthor.Text = selected.Author;
                txtTitle.Text = selected.Title;
                txtGenre.Text = selected.Genre;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTitle.Text))
                books.Add(new Book(txtAuthor.Text, txtTitle.Text, txtGenre.Text));
            Clear();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                selected.Author = txtAuthor.Text;
                selected.Title = txtTitle.Text;
                selected.Genre = txtGenre.Text;
                Clear();
                lstBooks.Items.Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                books.Remove(selected);
                Clear();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                lstSearch.ItemsSource = books;
            else
                lstSearch.ItemsSource = books.Where(x =>
                    x.Author.ToLower().Contains(txtSearch.Text.ToLower()) ||
                    x.Title.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
        }

        private void Clear() { txtAuthor.Text = ""; txtTitle.Text = ""; txtGenre.Text = ""; selected = null; }
    }
}