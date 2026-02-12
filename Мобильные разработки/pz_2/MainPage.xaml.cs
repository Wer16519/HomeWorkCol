namespace pz_2
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ProgressBarButton(object sender, EventArgs e)
        {
            Progress.Progress = 0;
            for(int i = 0; i <= 100; i++)
            {
                Progress.IsVisible = true;
                Progress.Progress = i / 100.0;
                label1.Text = $"Состояние процесса: {i} %";
                await Task.Delay(30);
            }
        }
    }
}
