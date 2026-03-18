using CardMaster.Models;
using CardMaster.Operations;

namespace CardMaster.Views;

public partial class AddDiscountPage : ContentPage
{
    public AddDiscountPage()
    {
        InitializeComponent();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(PromocodeEntry.Text) ||
            string.IsNullOrWhiteSpace(DescriptionEditor.Text))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "OK");
            return;
        }

        var newDiscount = new Discount
        {
            Id = DataStore.Discounts.Count + 1,
            Name = NameEntry.Text,
            Promocode = PromocodeEntry.Text,
            Description = DescriptionEditor.Text,
            Image = "discount.png"
        };

        DataStore.Discounts.Add(newDiscount);

        await DisplayAlert("Успешно", "Скидка добавлена", "OK");
        await Navigation.PopAsync();
    }
}