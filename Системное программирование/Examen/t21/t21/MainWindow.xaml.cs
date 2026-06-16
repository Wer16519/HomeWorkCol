using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace t21
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillComboBox(1);
        }

        private void TrackBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (comboBox != null)
                FillComboBox((int)trackBar.Value);
        }

        private void FillComboBox(int count)
        {
            comboBox.Items.Clear();
            for (int i = 1; i <= count; i++)
                comboBox.Items.Add($"Элемент {i}");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem == null) return;

            if (rbPanel.Children.Count >= 5)
            {
                MessageBox.Show("В GroupBox уже 5 RadioButton. Добавление прекращено.");
                return;
            }

            RadioButton rb = new RadioButton
            {
                Content = comboBox.SelectedItem.ToString(),
                Margin = new Thickness(0, 3, 0, 3)
            };
            rbPanel.Children.Add(rb);
        }
    }
}