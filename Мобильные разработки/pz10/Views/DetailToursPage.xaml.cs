using Microsoft.Maui.Controls;
using pz10.Models;

namespace pz10.Views;

public partial class DetailToursPage : ContentPage
{
    public TourItem SelectedTour { get; set; }

    public DetailToursPage(TourItem tour)
    {
        InitializeComponent();

        SelectedTour = tour;
        BindingContext = this;

        LoadAlternativeDates();
    }

    private void LoadAlternativeDates()
    {
        var dates = new[]
        {
            new { Date = "2026-06-24", Price = SelectedTour.Price },
            new { Date = "2026-07-12", Price = SelectedTour.Price },
            new { Date = "2026-08-10", Price = SelectedTour.Price }
        };

        foreach (var item in dates)
        {
            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                BorderColor = Color.FromArgb("#333333"),
                CornerRadius = 40,
                Padding = new Thickness(20, 15)
            };

            var verticalStack = new VerticalStackLayout { Spacing = 10 };

            var dateStack = new HorizontalStackLayout
            {
                Spacing = 8,
                HorizontalOptions = LayoutOptions.Start
            };
            dateStack.Children.Add(new Label
            {
                Text = "🗓",
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center
            });
            dateStack.Children.Add(new Label
            {
                Text = item.Date,
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center
            });

            var priceStack = new HorizontalStackLayout
            {
                Spacing = 8,
                HorizontalOptions = LayoutOptions.Start
            };
            priceStack.Children.Add(new Label
            {
                Text = "₽",
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center
            });
            priceStack.Children.Add(new Label
            {
                Text = item.Price,
                FontSize = 18,
                TextColor = Colors.Black,
                VerticalOptions = LayoutOptions.Center
            });

            verticalStack.Children.Add(dateStack);
            verticalStack.Children.Add(priceStack);

            frame.Content = verticalStack;
            AlternativeDatesContainer.Children.Add(frame);
        }
    }
}