using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace t10
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex < tabControl.Items.Count - 1)
                tabControl.SelectedIndex++;
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Рекомендации на основе ваших ответов:\n");

            if (q2_yes.IsChecked == true)
                sb.AppendLine("• Попробуйте мексиканскую или индийскую кухню — они известны острыми блюдами.");
            else
                sb.AppendLine("• Вам подойдут блюда европейской кухни с мягкими вкусами.");

            if (q3_veg.IsChecked == true)
                sb.AppendLine("• Рекомендуем больше блюд из бобовых, овощей и круп.");
            else
                sb.AppendLine("• Включайте в рацион разные виды мяса для разнообразия белка.");

            if (q5_yes.IsChecked == true)
                sb.AppendLine("• Не забывайте про умеренность в сладком — попробуйте десерты с фруктами.");

            if (q7_yes.IsChecked == true)
                sb.AppendLine("• Отличный выбор! Рыба богата омега-3, полезна для здоровья.");
            else
                sb.AppendLine("• Стоит добавить рыбу в рацион хотя бы 1-2 раза в неделю.");

            sb.AppendLine("\nСпасибо за прохождение опроса!");

            lblResult.Text = sb.ToString();
            tabControl.SelectedIndex = tabControl.Items.Count - 1;
        }
    }
}