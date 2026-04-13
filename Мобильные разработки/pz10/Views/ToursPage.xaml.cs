using pz10.Models;
using System.Collections.ObjectModel;

namespace pz10.Views;

public partial class ToursPage : ContentPage
{
    public ObservableCollection<TourItem> Tours { get; set; }

    public ToursPage()
    {
        InitializeComponent();

        Tours = new ObservableCollection<TourItem>();

        var toursData = new[]
        {
            new { Title = "Каньёны и Каспий", Location = "Дагестан", Image = "white_mountain.jpg" },
            new { Title = "Мегоном, Таврида.Арт", Location = "Крым", Image = "mountain_card.jpg" },
            new { Title = "Палатки, рыбалка", Location = "Прибайкалье", Image = "camping_card.jpg" },
            new { Title = "Джакузи, Бассейн, Баня", Location = "Краснодар", Image = "city_fog.jpg" },
            new { Title = "Рафтинг, Скалолазание", Location = "Карелия", Image = "mountain_card.jpg" },
            new { Title = "Палатки, грибы", Location = "Прибайкалье", Image = "camping_card.jpg" }
        };

        foreach (var data in toursData)
        {
            Tours.Add(new TourItem
            {
                Title = data.Title,
                Location = data.Location,
                ImageSource = data.Image
            });
        }

        ToursCollectionView.ItemsSource = Tours;
    }

    private async void OnTourTapped(object sender, TappedEventArgs e)
    {
        var tour = e.Parameter as TourItem;
        if (tour != null)
        {
            await Navigation.PushAsync(new DetailToursPage(tour));
        }
    }
}