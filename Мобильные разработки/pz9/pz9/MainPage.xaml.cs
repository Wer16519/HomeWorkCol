using System;
using System.Timers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace pz9
{
    public partial class MainPage : ContentPage
    {
        private System.Timers.Timer timer;
        private int timeLeft = 10;      
        private int score = 0;
        private int number1, number2;
        private bool isGameActive = true;
        private bool isAnswerChecked = false;

        public MainPage()
        {
            InitializeComponent();
            GenerateNewExample();
        }

        private void GenerateNewExample()
        {
            Random rand = new Random();

            number1 = rand.Next(1, 100);     
            number2 = rand.Next(1, 100);     

            ExampleLabel.Text = $"{number1} + {number2} = ?";

            AnswerEntry.Text = string.Empty;
            ResultLabel.Text = string.Empty;

            ResetTimer();

            isAnswerChecked = false;
        }

        private void ResetTimer()
        {
            timer?.Stop();
            timer?.Dispose();

            timeLeft = 10;
            UpdateTimerDisplay();

            timer = new System.Timers.Timer(1000);    
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            timeLeft--;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateTimerDisplay();

                if (timeLeft <= 0 && !isAnswerChecked && isGameActive)
                {
                    timer.Stop();
                    ResultLabel.Text = "Время вышло!";
                    ResultLabel.TextColor = Colors.Red;
                    isAnswerChecked = true;

                    ResetScore();
                }
            });
        }

        private void UpdateTimerDisplay()
        {
            TimerText.Text = $"Осталось времени: {timeLeft} сек";

            double progress = timeLeft / 10.0;
            timeProgressBar.Progress = Math.Max(0, progress);

            if (timeLeft <= 3)
            {
                timeProgressBar.ProgressColor = Colors.Red;
            }
            else if (timeLeft <= 5)
            {
                timeProgressBar.ProgressColor = Colors.Orange;
            }
            else
            {
                timeProgressBar.ProgressColor = Colors.Green;
            }
        }

        private void CheckButton_Clicked(object sender, EventArgs e)
        {
            if (!isGameActive || isAnswerChecked)
                return;

            if (string.IsNullOrWhiteSpace(AnswerEntry.Text))
            {
                ResultLabel.Text = "Введите ответ!";
                ResultLabel.TextColor = Colors.Orange;
                return;
            }

            if (int.TryParse(AnswerEntry.Text, out int userAnswer))
            {
                int correctAnswer = number1 + number2;

                if (userAnswer == correctAnswer)
                {
                    ResultLabel.Text = "Правильно!";
                    ResultLabel.TextColor = Colors.Green;

                    score++;
                    UpdateScore();

                    isAnswerChecked = true;

                    timer.Stop();

                    Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            GenerateNewExample();
                        });
                        return false;   
                    });
                }
                else
                {
                    ResultLabel.Text = $"Неправильно! Правильный ответ: {correctAnswer}";
                    ResultLabel.TextColor = Colors.Red;

                    isAnswerChecked = true;

                    timer.Stop();

                    ResetScore();
                }
            }
            else
            {
                ResultLabel.Text = "Введите число!";
                ResultLabel.TextColor = Colors.Orange;
            }
        }

        private void UpdateScore()
        {
            ScoreLabel.Text = $"Счет: {score}";
        }

        private void ResetScore()
        {
            score = 0;
            UpdateScore();
            isGameActive = false;
        }

        private void NewGameButton_Clicked(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            score = 0;
            UpdateScore();
            isGameActive = true;
            isAnswerChecked = false;
            GenerateNewExample();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            timer?.Stop();
            timer?.Dispose();
            timer = null;
        }
    }
}