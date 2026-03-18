using CardMaster.Models;
using CardMaster.Operations;

namespace CardMaster.Views;

public partial class CardsPage : ContentPage
{
    public CardsPage()
    {
        InitializeComponent();
        LoadCards();
    }

    private async void LoadCards()
    {
        Refresh.IsRefreshing = true;
        ColletctionView.ItemsSource = DataStore.Cards;
        await Task.Delay(500);
        Refresh.IsRefreshing = false;
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(searchText))
        {
            ColletctionView.ItemsSource = DataStore.Cards;
        }
        else
        {
            var filtered = DataStore.Cards
                .Where(x => x.Name.ToLower().Contains(searchText))
                .ToList();
            ColletctionView.ItemsSource = filtered;
        }
    }

    private void Refresh_Refreshing(object sender, EventArgs e)
    {
        LoadCards();
    }

    private async void ColletctionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Card selectedCard)
        {
            ColletctionView.SelectedItem = null;
            await Navigation.PushAsync(new CardDetailPage(selectedCard.Id));
        }
    }

    // лернд дкъ ймнойх днаюбкемхъ
    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCardPage());
    }
}