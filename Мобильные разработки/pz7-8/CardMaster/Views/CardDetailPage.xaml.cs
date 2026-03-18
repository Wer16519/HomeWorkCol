using CardMaster.Models;
using CardMaster.Operations;
using Microsoft.Maui.Controls;
using QRCoder;
using System.IO;

namespace CardMaster.Views;

public partial class CardDetailPage : ContentPage
{
    private Card _card;

    public CardDetailPage(int id)
    {
        InitializeComponent();

        _card = DataStore.Cards.FirstOrDefault(x => x.Id == id);

        if (_card != null)
        {
            TitleBox.Text = _card.Name;
            NumberLabel.Text = _card.Number;
            GenerateQRCode(_card.Number);
        }
    }

    private void GenerateQRCode(string text)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        QrCode.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (_card != null)
        {
            DataStore.Cards.Remove(_card);
            await Navigation.PopAsync();
        }
    }
}