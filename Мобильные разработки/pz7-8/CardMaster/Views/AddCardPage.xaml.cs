using CardMaster.Models;
using CardMaster.Operations;

namespace CardMaster.Views;

public partial class AddCardPage : ContentPage
{
    public AddCardPage()
    {
        InitializeComponent();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(NumberEntry.Text))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "OK");
            return;
        }

        var newCard = new Card
        {
            Id = DataStore.Cards.Count + 1,
            Name = NameEntry.Text,
            Number = NumberEntry.Text,
            Image = "card.png"
        };

        DataStore.Cards.Add(newCard);

        await DisplayAlert("Успешно", "Карта добавлена", "OK");
        await Navigation.PopAsync();
    }
}