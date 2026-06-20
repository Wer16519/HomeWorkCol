using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Zad9
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public int Count;
        public int Spacing;
        public Color Color;
        public int Size;

        public SettingsWindow()
        {
            InitializeComponent();

            sliderCount.ValueChanged += (s, e) =>
                lblCount.Text = ((int)sliderCount.Value).ToString();

            sliderSpacing.ValueChanged += (s, e) =>
                lblSpacing.Text = ((int)sliderSpacing.Value).ToString();

            sliderSize.ValueChanged += (s, e) =>
                lblSize.Text = ((int)sliderSize.Value).ToString();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Count = (int)sliderCount.Value;
            Spacing = (int)sliderSpacing.Value;
            Size = (int)sliderSize.Value;

            string color = ((ComboBoxItem)cmbColor.SelectedItem).Tag.ToString();
            if (color == "Red") Color = Colors.Red;
            else if (color == "Green") Color = Colors.Green;
            else if (color == "Blue") Color = Colors.Blue;
            else Color = Colors.Gray;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}