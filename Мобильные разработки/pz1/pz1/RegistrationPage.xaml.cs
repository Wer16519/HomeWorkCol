namespace pz1;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage()
	{
		InitializeComponent();
	}

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string firstName = FirstName.Text;
        string login = Login.Text;
        string password = Password.Text;
        string pas_en = Password_enother.Text;

        string message = $"Имя: {firstName}\n" +
                         $"Логин: {login}\n" +
                         $"Пароль: {password}\n" +
                         $"Пароль ещё раз: {pas_en}";

        if (password == pas_en)
        {
            await DisplayAlert("Данные регистрации", message, "OK");
        }
        else
        {
            await DisplayAlert("Ошибка", "Данные не сходны", "ОК");
        }
    }

    private async void OnGoBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}