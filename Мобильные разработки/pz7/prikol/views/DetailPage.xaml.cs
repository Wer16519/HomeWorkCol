using prikol.operations;
using QRCoder;

namespace prikol.views;

public partial class DetailPage : ContentPage
{
	public DetailPage(int Id)
	{
		InitializeComponent();
		var query = Operations.AllCards.FirstOrDefault(x => x.Id == Id);
		QRCodeGenerator codeGenerator = new QRCodeGenerator();
		QRCodeData qRCodeData = codeGenerator.CreateQrCode(query.Number, QRCodeGenerator.ECCLevel.Q);
		PngByteQRCode qRCode = new PngByteQRCode();
		byte[] qrCodeByte = qRCode.GetGraphic(20);
	}

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}