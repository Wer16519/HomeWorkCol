using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zad9
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateButtons_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow win = new SettingsWindow();
            if (win.ShowDialog() == true)
            {
                for (int i = 0; i < win.Count; i++)
                {
                    Button btn = new Button();
                    btn.Content = "Кнопка";
                    btn.Width = win.Size;
                    btn.Height = 30;
                    btn.Background = new SolidColorBrush(win.Color);
                    btn.Margin = new Thickness(win.Spacing);
                    pnlContainer.Children.Add(btn);
                }
            }
        }

        private void GenerateLabels_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow win = new SettingsWindow();
            if (win.ShowDialog() == true)
            {
                for (int i = 0; i < win.Count; i++)
                {
                    Label lbl = new Label();
                    lbl.Content = "Метка";
                    lbl.FontSize = win.Size;
                    lbl.Foreground = new SolidColorBrush(win.Color);
                    lbl.Margin = new Thickness(win.Spacing);
                    pnlContainer.Children.Add(lbl);
                }
            }
        }

        private void GenerateTextBoxes_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow win = new SettingsWindow();
            if (win.ShowDialog() == true)
            {
                for (int i = 0; i < win.Count; i++)
                {
                    TextBox txt = new TextBox();
                    txt.Width = 100;
                    txt.Height = 25;
                    txt.Background = new SolidColorBrush(win.Color);
                    txt.Margin = new Thickness(win.Spacing);
                    txt.Text = "Текст";
                    pnlContainer.Children.Add(txt);
                }
            }
        }

        private void GenerateLists_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow win = new SettingsWindow();
            if (win.ShowDialog() == true)
            {
                for (int i = 0; i < win.Count; i++)
                {
                    ListBox lb = new ListBox();
                    lb.Width = 120;
                    lb.Height = 80;
                    lb.Background = new SolidColorBrush(win.Color);
                    lb.Margin = new Thickness(win.Spacing);
                    lb.Items.Add("Элемент 1");
                    lb.Items.Add("Элемент 2");
                    pnlContainer.Children.Add(lb);
                }
            }
        }
    }
}