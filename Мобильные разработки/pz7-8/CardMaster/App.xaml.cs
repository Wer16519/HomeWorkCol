using Microsoft.Extensions.DependencyInjection;

namespace CardMaster;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    public static void ChangeTheme(AppTheme theme)
    {
        Current.UserAppTheme = theme;
    }
}