using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace task6
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FontSize_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            if (RbSmall.IsChecked == true) this.FontSize = 10;
            if (RbMedium.IsChecked == true) this.FontSize = 14;
            if (RbLarge.IsChecked == true) this.FontSize = 20;
        }

        private void FormColor_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            if (RbBgDefault.IsChecked == true)
                this.Background = SystemColors.WindowBrush;
            else if (RbBgWhite.IsChecked == true)
                this.Background = Brushes.White;
            else if (RbBgBlack.IsChecked == true)
                this.Background = Brushes.Black;
        }

        private void TextColor_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            Brush color = Brushes.Black;
            if (RbFgWhite.IsChecked == true) color = Brushes.White;
            if (RbFgRed.IsChecked == true) color = Brushes.Red;
            this.Foreground = color;
        }

        private void Option_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            this.ResizeMode = ChkNoResize.IsChecked == true
                ? ResizeMode.NoResize
                : ResizeMode.CanResize;

            bool canMin = ChkNoMinimize.IsChecked != true;
            bool canMax = ChkNoMaximize.IsChecked != true;

            if (!canMin && !canMax)
                this.WindowStyle = WindowStyle.SingleBorderWindow;

            this.MinimizeBox(canMin);
            this.MaximizeBox(canMax);
        }

        private void ApplySize_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WidthBox.Text, out int w) && w > 100)
                this.Width = w;
            if (int.TryParse(HeightBox.Text, out int h) && h > 100)
                this.Height = h;
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }

    internal static class WindowExtensions
    {
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowLong(System.IntPtr hwnd, int index);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(System.IntPtr hwnd, int index, int value);

        public static void MinimizeBox(this Window w, bool enabled) =>
            SetButton(w, WS_MINIMIZEBOX, enabled);

        public static void MaximizeBox(this Window w, bool enabled) =>
            SetButton(w, WS_MAXIMIZEBOX, enabled);

        private static void SetButton(Window w, int flag, bool enabled)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(w).Handle;
            if (hwnd == System.IntPtr.Zero) return;
            int style = GetWindowLong(hwnd, GWL_STYLE);
            style = enabled ? style | flag : style & ~flag;
            SetWindowLong(hwnd, GWL_STYLE, style);
        }
    }
}