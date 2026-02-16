using ClassWork3.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ClassWork3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new StudentList());
        }
    }
}