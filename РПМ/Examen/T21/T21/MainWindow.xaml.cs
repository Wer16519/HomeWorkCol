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

namespace T21
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Event> events = new ObservableCollection<Event>();
        Event selected;

        public MainWindow()
        {
            InitializeComponent();
            lstEvents.ItemsSource = events;
            lstSearch.ItemsSource = events;
            dpDate.SelectedDate = DateTime.Now;

            // Тестовые данные
            events.Add(new Event("Встреча", "С клиентом", DateTime.Now, "14:00"));
            events.Add(new Event("Звонок", "Маме", DateTime.Now.AddDays(1), "10:30"));
        }

        public class Event
        {
            public string Title { get; set; }
            public string Desc { get; set; }
            public DateTime Date { get; set; }
            public string Time { get; set; }
            public Event(string t, string d, DateTime dt, string tm) { Title = t; Desc = d; Date = dt; Time = tm; }
            public string DisplayInfo => $"{Date.ToShortDateString()} {Time} - {Title}";
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendar.SelectedDate.HasValue)
            {
                dpDate.SelectedDate = calendar.SelectedDate.Value;
                var filtered = events.Where(x => x.Date.Date == calendar.SelectedDate.Value.Date).ToList();
                lstEvents.ItemsSource = filtered;
            }
        }

        private void LstEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = lstEvents.SelectedItem as Event;
            if (selected != null)
            {
                txtTitle.Text = selected.Title;
                txtDesc.Text = selected.Desc;
                dpDate.SelectedDate = selected.Date;
                txtTime.Text = selected.Time;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dpDate.SelectedDate.HasValue && !string.IsNullOrEmpty(txtTitle.Text))
            {
                events.Add(new Event(txtTitle.Text, txtDesc.Text, dpDate.SelectedDate.Value, txtTime.Text));
                Clear();
                if (calendar.SelectedDate.HasValue)
                    Calendar_SelectedDatesChanged(null, null);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null && dpDate.SelectedDate.HasValue)
            {
                selected.Title = txtTitle.Text;
                selected.Desc = txtDesc.Text;
                selected.Date = dpDate.SelectedDate.Value;
                selected.Time = txtTime.Text;
                Clear();
                if (calendar.SelectedDate.HasValue)
                    Calendar_SelectedDatesChanged(null, null);
                lstEvents.Items.Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                events.Remove(selected);
                Clear();
                if (calendar.SelectedDate.HasValue)
                    Calendar_SelectedDatesChanged(null, null);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                lstSearch.ItemsSource = events;
            else
                lstSearch.ItemsSource = events.Where(x =>
                    x.Title.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
        }

        private void Clear()
        {
            txtTitle.Text = "";
            txtDesc.Text = "";
            txtTime.Text = "12:00";
            selected = null;
        }
    }
}