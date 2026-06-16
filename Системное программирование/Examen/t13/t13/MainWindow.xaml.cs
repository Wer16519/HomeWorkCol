using System;
using System.Windows;
using System.Windows.Controls;

namespace t13
{
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();

            try
            {
                int count = int.Parse(tbCount.Text);
                double a = double.Parse(tbA.Text);
                double b = double.Parse(tbB.Text);

                System.Windows.Media.Brush color = System.Windows.Media.Brushes.Black;
                if (cbColor.SelectedItem is ComboBoxItem item)
                {
                    switch (item.Content.ToString())
                    {
                        case "Красный": color = System.Windows.Media.Brushes.Red; break;
                        case "Зелёный": color = System.Windows.Media.Brushes.Green; break;
                        case "Синий": color = System.Windows.Media.Brushes.Blue; break;
                        case "Жёлтый": color = System.Windows.Media.Brushes.Yellow; break;
                        case "Чёрный": color = System.Windows.Media.Brushes.Black; break;
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                    {
                        Width = a,
                        Height = b,
                        Fill = color,
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = 1
                    };

                    double maxX = Math.Max(10, canvas.ActualWidth - a);
                    double maxY = Math.Max(10, canvas.ActualHeight - b);

                    Canvas.SetLeft(rect, rnd.Next(0, (int)maxX));
                    Canvas.SetTop(rect, rnd.Next(0, (int)maxY));

                    canvas.Children.Add(rect);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода: " + ex.Message);
            }
        }
    }
}