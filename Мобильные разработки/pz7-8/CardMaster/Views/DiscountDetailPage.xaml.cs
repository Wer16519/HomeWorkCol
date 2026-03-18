using CardMaster.Models;
using CardMaster.Operations;

namespace CardMaster.Views;

public partial class DiscountDetailPage : ContentPage
{
    private Discount _discount;

    public DiscountDetailPage(int id)
    {
        InitializeComponent();

        _discount = DataStore.Discounts.FirstOrDefault(x => x.Id == id);

        if (_discount != null)
        {
            TitleLabel.Text = _discount.Name;
            DescriptionLabel.Text = _discount.Description;
            PromocodeLabel.Text = _discount.Promocode;
            DiscountImage.Source = _discount.Image;
        }
    }

    private async void CopyButton_Clicked(object sender, EventArgs e)
    {
        if (_discount != null && !string.IsNullOrEmpty(_discount.Promocode))
        {
            await Clipboard.Default.SetTextAsync(_discount.Promocode);
            await DisplayAlert("Успешно", "Промокод скопирован в буфер обмена", "OK");
        }
    }
}