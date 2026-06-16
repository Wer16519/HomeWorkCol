using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace t2
{
    public partial class MainWindow : Window
    {
        private Brush standardColor;

        public MainWindow()
        {
            InitializeComponent();
            standardColor = this.Background; // запоминаем исходный цвет
        }

        // === Изменение размера текста ===
        private void SizeChanged(object sender, RoutedEventArgs e)
        {
            if (lblText == null) return;
            RadioButton rb = sender as RadioButton;

            if (rb == rbSmall) lblText.FontSize = 10;
            else if (rb == rbMedium) lblText.FontSize = 14;
            else if (rb == rbLarge) lblText.FontSize = 22;
        }

        // === Изменение цвета окна ===
        private void FormColorChanged(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == rbStandard) this.Background = standardColor;
            else if (rb == rbWhite) this.Background = Brushes.White;
            else if (rb == rbBlack) this.Background = Brushes.Black;
        }

        // === Изменение цвета текста ===
        private void TextColorChanged(object sender, RoutedEventArgs e)
        {
            if (lblText == null) return;
            RadioButton rb = sender as RadioButton;

            Brush color = Brushes.Black;
            if (rb == rbTextWhite) color = Brushes.White;
            else if (rb == rbTextRed) color = Brushes.Red;

            lblText.Foreground = color;
        }

        // === Опции окна ===
        private void OptionsChanged(object sender, RoutedEventArgs e)
        {
            // Запрет изменения размера
            this.ResizeMode = cbResize.IsChecked == true
                ? ResizeMode.NoResize
                : ResizeMode.CanResize;

            // Запрет разворачивания/сворачивания — управляется через ResizeMode
            if (cbMaximize.IsChecked == true && cbMinimize.IsChecked == true)
                this.ResizeMode = ResizeMode.NoResize;
            else if (cbMaximize.IsChecked == true)
                this.ResizeMode = ResizeMode.CanMinimize;
        }
    }
}