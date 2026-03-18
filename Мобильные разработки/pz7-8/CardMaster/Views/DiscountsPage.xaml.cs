using CardMaster.Models;
using CardMaster.Operations;

namespace CardMaster.Views;

public partial class DiscountsPage : ContentPage
{
    public DiscountsPage()
    {
        InitializeComponent();
        LoadDiscounts();
    }

    private async void LoadDiscounts()
    {
        Refresh.IsRefreshing = true;
        DiscountsCollectionView.ItemsSource = DataStore.Discounts;
        await Task.Delay(500);
        Refresh.IsRefreshing = false;
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(searchText))
        {
            DiscountsCollectionView.ItemsSource = DataStore.Discounts;
        }
        else
        {
            var filtered = DataStore.Discounts
                .Where(x => x.Name.ToLower().Contains(searchText) ||
                           x.Description.ToLower().Contains(searchText))
                .ToList();
            DiscountsCollectionView.ItemsSource = filtered;
        }
    }

    private void Refresh_Refreshing(object sender, EventArgs e)
    {
        LoadDiscounts();
    }

    private async void DiscountsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Discount selectedDiscount)
        {
            DiscountsCollectionView.SelectedItem = null;
            await Navigation.PushAsync(new DiscountDetailPage(selectedDiscount.Id));
        }
    }

    // лернд дкъ ймнойх днаюбкемхъ
    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddDiscountPage());
    }
}