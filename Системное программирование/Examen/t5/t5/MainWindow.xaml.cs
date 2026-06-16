using System;
using System.Windows;
using System.Windows.Controls;

namespace t5
{
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenButton_Click(object sender, RoutedEventArgs e)
        {
            int count = int.Parse((sender as MenuItem).Header.ToString());
            for (int i = 0; i < count; i++)
            {
                Button b = new Button
                {
                    Content = "Button " + (i + 1),
                    Width = 80,
                    Height = 25
                };
                PlaceControl(b);
            }
        }

        private void GenLabel_Click(object sender, RoutedEventArgs e)
        {
            int count = int.Parse((sender as MenuItem).Header.ToString());
            for (int i = 0; i < count; i++)
            {
                Label l = new Label { Content = "Label " + (i + 1) };
                PlaceControl(l);
            }
        }

        private void GenTextBox_Click(object sender, RoutedEventArgs e)
        {
            int count = int.Parse((sender as MenuItem).Header.ToString());
            for (int i = 0; i < count; i++)
            {
                TextBox t = new TextBox
                {
                    Text = "TextBox " + (i + 1),
                    Width = 100,
                    Height = 23
                };
                PlaceControl(t);
            }
        }

        private void GenListBox_Click(object sender, RoutedEventArgs e)
        {
            int count = int.Parse((sender as MenuItem).Header.ToString());
            for (int i = 0; i < count; i++)
            {
                ListBox lb = new ListBox { Width = 100, Height = 80 };
                lb.Items.Add("Элемент 1");
                lb.Items.Add("Элемент 2");
                lb.Items.Add("Элемент 3");
                PlaceControl(lb);
            }
        }

        private void PlaceControl(Control c)
        {
            Canvas.SetLeft(c, rnd.Next(0, (int)mainCanvas.ActualWidth - 100));
            Canvas.SetTop(c, rnd.Next(0, (int)mainCanvas.ActualHeight - 80));
            mainCanvas.Children.Add(c);
        }
    }
}