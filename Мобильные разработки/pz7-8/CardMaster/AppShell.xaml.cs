using CardMaster.Views;

namespace CardMaster;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(CardDetailPage), typeof(CardDetailPage));
        Routing.RegisterRoute(nameof(DiscountDetailPage), typeof(DiscountDetailPage));
        Routing.RegisterRoute(nameof(AddCardPage), typeof(AddCardPage));
        Routing.RegisterRoute(nameof(AddDiscountPage), typeof(AddDiscountPage));
    }
}