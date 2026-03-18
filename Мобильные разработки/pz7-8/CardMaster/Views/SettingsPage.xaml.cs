using CardMaster.Operations;

namespace CardMaster.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadStatistics();

        ThemeSwitch.IsToggled = Application.Current.UserAppTheme == AppTheme.Dark;
    }

    private void LoadStatistics()
    {
        TotalCardsLabel.Text = DataStore.Cards.Count.ToString();
        TotalDiscountsLabel.Text = DataStore.Discounts.Count.ToString();
    }

    private void ThemeSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            App.ChangeTheme(AppTheme.Dark);
        }
        else
        {
            App.ChangeTheme(AppTheme.Light);
        }
    }
}