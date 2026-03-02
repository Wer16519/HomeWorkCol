using prikol.models;
using prikol.operations;

namespace prikol.views;

public partial class Cards : ContentPage
{
	public Cards()
	{
		InitializeComponent();
        LoadCard();
	}

    private async void LoadCard()
    {
        Refresh.IsRefreshing = true;
        ColletctionView.ItemsSource = Operations.AllCards;
        await Task.Delay(200);
        Refresh.IsRefreshing = false;
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void Refresh_Refreshing(object sender, EventArgs e)
    {

    }

    private async Task ColletctionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(((CollectionView)sender).SelectedItem != null)
        {
            ((CollectionView)sender).SelectedItem = null;
            Cards? resources = ColletctionView.SelectedItem as Cards;
            await Navigation.PushAsync(new DetailPage(resources.Id));
        }
    }
}