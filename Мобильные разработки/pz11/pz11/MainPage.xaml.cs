using pz11.Models;

namespace pz11
{
    public partial class MainPage : ContentPage
    {
        private RadioButton? _selectedRadioButton = null;
        private List<Question> _questions = new();
        private int _currentQuestionIndex = 0;
        private int _score = 0;

        public MainPage()
        {
            InitializeComponent();
            LoadQuestions();
            DisplayQuestion(_currentQuestionIndex);
        }

        private void LoadQuestions()
        {
            _questions = new List<Question>
            {
                new Question
                {
                    Text = "Сколько планет в Солнечной системе?",
                    Options = new List<string> { "7", "8", "9", "10" },
                    CorrectAnswer = "8"
                },
                new Question
                {
                    Text = "Какой сейчас месяц?",
                    Options = new List<string> { "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" },
                    CorrectAnswer = "Апрель"
                },
                new Question
                {
                    Text = "Сколько дней в феврале в високосный год?",
                    Options = new List<string> { "28", "29", "30", "31" },
                    CorrectAnswer = "29"
                },
                new Question
                {
                    Text = "Земля является ...",
                    Options = new List<string> { "Планетой", "Звездой", "Спутником", "Астероидом" },
                    CorrectAnswer = "Планетой"
                },
                new Question
                {
                    Text = "Сколько океанов на Земле?",
                    Options = new List<string> { "3", "4", "5", "6" },
                    CorrectAnswer = "5"
                }
            };
        }

        private void DisplayQuestion(int index)
        {
            if (index >= _questions.Count)
            {
                FinishTest();
                return;
            }

            _selectedRadioButton = null;
            ValidationMessage.IsVisible = false;

            var currentQuestion = _questions[index];

            ProgressLabel.Text = $"Вопрос {index + 1} из {_questions.Count}";
            QuestionLabel.Text = currentQuestion.Text;

            OptionsLayout.Children.Clear();
            var radioGroup = new VerticalStackLayout { Spacing = 8 };

            foreach (var option in currentQuestion.Options)
            {
                var radioButton = new RadioButton
                {
                    Content = option,
                    FontSize = 16,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                radioButton.CheckedChanged += OnRadioButtonCheckedChanged;
                radioGroup.Children.Add(radioButton);
            }

            OptionsLayout.Children.Add(radioGroup);

            NextButton.Text = (index == _questions.Count - 1) ? "Завершить" : "Далее";
        }

        private void OnRadioButtonCheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton rb && e.Value)
            {
                _selectedRadioButton = rb;
            }
        }

        private async void OnNextButtonClicked(object? sender, EventArgs e)
        {
            if (_selectedRadioButton == null)
            {
                ValidationMessage.IsVisible = true;
                return;
            }

            ValidationMessage.IsVisible = false;

            if (_selectedRadioButton.Content?.ToString() == _questions[_currentQuestionIndex].CorrectAnswer)
            {
                _score++;
            }

            _currentQuestionIndex++;

            if (_currentQuestionIndex < _questions.Count)
            {
                DisplayQuestion(_currentQuestionIndex);
            }
            else
            {
                FinishTest();
            }
        }

        private async void FinishTest()
        {
            double percent = Math.Round((double)_score / _questions.Count * 100, 1);

            string title = "Тест завершен";
            string message = $"📚 Учись лучше!\n\nВаш результат: {_score} из {_questions.Count}\nПроцент правильных ответов: {percent}\n\nХотите пройти тест заново?";

            bool restart = await DisplayAlertAsync(title, message, "Да", "Нет");

            if (restart)
            {
                _currentQuestionIndex = 0;
                _score = 0;
                DisplayQuestion(_currentQuestionIndex);
            }
            else
            {
                Application.Current?.Quit();
            }
        }
    }
}