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

namespace Zad12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string[]> questions = new List<string[]>();
        private List<string> answers = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            questions.Add(new string[] { "Как вы относитесь к мясу?", "Обожаю", "Нормально", "Не ем" });
            questions.Add(new string[] { "Любите острую пищу?", "Да, очень", "Умеренно", "Нет" });
            questions.Add(new string[] { "Любите рыбу?", "Да", "Иногда", "Нет" });
            questions.Add(new string[] { "Как часто едите овощи?", "Каждый день", "Иногда", "Редко" });
            questions.Add(new string[] { "Любите сладкое?", "Очень", "Умеренно", "Не люблю" });
            questions.Add(new string[] { "Любите супы?", "Да", "Иногда", "Нет" });
            questions.Add(new string[] { "Что предпочитаете на завтрак?", "Каша", "Яичница", "Бутерброд" });
            questions.Add(new string[] { "Пьете кофе?", "Да, много", "Иногда", "Нет" });
            questions.Add(new string[] { "Любите молочные продукты?", "Да", "Иногда", "Нет" });
            questions.Add(new string[] { "Любите домашнюю еду?", "Да", "Иногда", "Нет" });
            questions.Add(new string[] { "Нравится фаст-фуд?", "Да", "Иногда", "Нет" });

            LoadQuestions();
            UpdateButtons();
        }

        private void LoadQuestions()
        {
            for (int i = 0; i < questions.Count; i++)
            {
                TabItem tab = new TabItem();
                tab.Header = $"Вопрос {i + 1}";

                StackPanel panel = new StackPanel();
                panel.Margin = new Thickness(15);

                TextBlock text = new TextBlock();
                text.Text = questions[i][0];
                text.FontSize = 15;
                text.FontWeight = FontWeights.Bold;
                text.Margin = new Thickness(0, 0, 0, 15);
                panel.Children.Add(text);

                for (int j = 1; j < questions[i].Length; j++)
                {
                    RadioButton rb = new RadioButton();
                    rb.Content = questions[i][j];
                    rb.FontSize = 13;
                    rb.Margin = new Thickness(0, 5, 0, 5);
                    rb.Tag = i;
                    rb.Checked += RadioButton_Checked;
                    panel.Children.Add(rb);
                }

                tab.Content = panel;
                tcQuestions.Items.Add(tab);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            int index = (int)rb.Tag;
            while (answers.Count <= index)
                answers.Add("");
            answers[index] = rb.Content.ToString();
            UpdateButtons();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (tcQuestions.SelectedIndex < tcQuestions.Items.Count - 1)
                tcQuestions.SelectedIndex++;
            UpdateButtons();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (tcQuestions.SelectedIndex > 0)
                tcQuestions.SelectedIndex--;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnBack.IsEnabled = tcQuestions.SelectedIndex > 0;
            btnNext.IsEnabled = tcQuestions.SelectedIndex < tcQuestions.Items.Count - 1;

            bool allAnswered = true;
            for (int i = 0; i < questions.Count; i++)
            {
                if (i >= answers.Count || string.IsNullOrEmpty(answers[i]))
                {
                    allAnswered = false;
                    break;
                }
            }
            btnResult.IsEnabled = allAnswered;
        }

        private void BtnResult_Click(object sender, RoutedEventArgs e)
        {
            int meat = 0, spicy = 0, sweet = 0, healthy = 0;

            for (int i = 0; i < answers.Count; i++)
            {
                if (i == 0)
                {
                    if (answers[i] == "Обожаю") meat = 2;
                    else if (answers[i] == "Нормально") meat = 1;
                }

                if (i == 1)
                {
                    if (answers[i] == "Да, очень") spicy = 2;
                    else if (answers[i] == "Умеренно") spicy = 1;
                }

                if (i == 4 && answers[i] == "Очень")
                    sweet = 2;

                if ((i == 3 && answers[i] == "Каждый день") || (i == 8 && answers[i] == "Да"))
                    healthy = 2;
            }

            string result;
            if (meat >= 2 && spicy >= 2)
                result = "Рекомендуем мексиканскую кухню!";
            else if (meat >= 2 && healthy >= 1)
                result = "Рекомендуем средиземноморскую кухню!";
            else if (sweet >= 2)
                result = "Рекомендуем французские десерты!";
            else if (healthy >= 2)
                result = "Рекомендуем вегетарианскую кухню!";
            else
                result = "Рекомендуем итальянскую кухню!";

            MessageBox.Show(result, "Рекомендация");
        }
    }
}