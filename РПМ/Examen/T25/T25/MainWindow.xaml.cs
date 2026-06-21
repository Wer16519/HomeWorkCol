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

namespace T25
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Movie> movies = new ObservableCollection<Movie>();
        Movie selected;

        public MainWindow()
        {
            InitializeComponent();
            lstMovies.ItemsSource = movies;
            lstSearch.ItemsSource = movies;
            movies.Add(new Movie("Побег из Шоушенка", "Драма", "Роббинс, Фриман", "Дарабонт"));
            movies.Add(new Movie("Крёстный отец", "Криминал", "Брандо, Пачино", "Коппола"));
        }

        public class Movie
        {
            public string Title { get; set; }
            public string Genre { get; set; }
            public string Actors { get; set; }
            public string Director { get; set; }
            public Movie(string t, string g, string a, string d) { Title = t; Genre = g; Actors = a; Director = d; }
            public string DisplayInfo => $"{Title} - {Genre}";
        }

        private void LstMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = lstMovies.SelectedItem as Movie;
            if (selected != null)
            {
                txtTitle.Text = selected.Title;
                txtGenre.Text = selected.Genre;
                txtActors.Text = selected.Actors;
                txtDirector.Text = selected.Director;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTitle.Text))
                movies.Add(new Movie(txtTitle.Text, txtGenre.Text, txtActors.Text, txtDirector.Text));
            Clear();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                selected.Title = txtTitle.Text;
                selected.Genre = txtGenre.Text;
                selected.Actors = txtActors.Text;
                selected.Director = txtDirector.Text;
                Clear();
                lstMovies.Items.Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                movies.Remove(selected);
                Clear();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                lstSearch.ItemsSource = movies;
            else
                lstSearch.ItemsSource = movies.Where(x =>
                    x.Genre.ToLower().Contains(txtSearch.Text.ToLower()) ||
                    x.Director.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
        }

        private void Clear() { txtTitle.Text = ""; txtGenre.Text = ""; txtActors.Text = ""; txtDirector.Text = ""; selected = null; }
    }
}