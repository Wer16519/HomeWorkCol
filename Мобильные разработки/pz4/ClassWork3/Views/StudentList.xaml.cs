using ClassWork3.Models;
using System.Collections.ObjectModel;

namespace ClassWork3.Views;

public partial class StudentList : ContentPage
{
    private ObservableCollection<Student> _allStudents;
    private ObservableCollection<Student> _filteredStudents;

    public StudentList()
    {
        InitializeComponent();
        _allStudents = new ObservableCollection<Student>();
        _filteredStudents = new ObservableCollection<Student>();
        LoadStudents();
    }

    private async void LoadStudents()
    {
        Refresh.IsRefreshing = true;
        StatusLabel.Text = "Загрузка...";

        _allStudents = new ObservableCollection<Student>
        {
            new Student {
                UserName = "Иван Иванов",
                Photo = "icon.png",
                GroupName = "ИТ-101",
                Email = "ivan.ivanov@student.ru"
            },
            new Student {
                UserName = "Мария Петрова",
                Photo = "icon.png",
                GroupName = "ИТ-102",
                Email = "maria.petrova@student.ru"
            },
            new Student {
                UserName = "Алексей Сидоров",
                Photo = "icon.png",
                GroupName = "ИТ-101",
                Email = "alexey.sidorov@student.ru"
            },
            new Student {
                UserName = "Екатерина Смирнова",
                Photo = "icon.png",
                GroupName = "ИТ-103",
                Email = "ekaterina.smirnova@student.ru"
            },
            new Student {
                UserName = "Дмитрий Кузнецов",
                Photo = "icon.png",
                GroupName = "ИТ-101",
                Email = "dmitry.kuznetsov@student.ru"
            },
            new Student {
                UserName = "Анна Волкова",
                Photo = "icon.png",
                GroupName = "ИТ-102",
                Email = "anna.volkova@student.ru"
            },
            new Student {
                UserName = "Сергей Попов",
                Photo = "icon.png",
                GroupName = "ИТ-104",
                Email = "sergey.popov@student.ru"
            },
            new Student {
                UserName = "Ольга Васильева",
                Photo = "icon.png",
                GroupName = "ИТ-103",
                Email = "olga.vasilyeva@student.ru"
            },
            new Student {
                UserName = "Павел Михайлов",
                Photo = "icon.png",
                GroupName = "ИТ-101",
                Email = "pavel.mikhailov@student.ru"
            },
            new Student {
                UserName = "Наталья Новикова",
                Photo = "icon.png",
                GroupName = "ИТ-102",
                Email = "natalya.novikova@student.ru"
            },
            new Student {
                UserName = "Андрей Федоров",
                Photo = "icon.png",
                GroupName = "ИТ-104",
                Email = "andrey.fedorov@student.ru"
            },
            new Student {
                UserName = "Татьяна Морозова",
                Photo = "icon.png",
                GroupName = "ИТ-103",
                Email = "tatyana.morozova@student.ru"
            },
            new Student {
                UserName = "Игорь Козлов",
                Photo = "icon.png",
                GroupName = "ИТ-101",
                Email = "igor.kozlov@student.ru"
            },
            new Student {
                UserName = "Елена Зайцева",
                Photo = "icon.png",
                GroupName = "ИТ-102",
                Email = "elena.zaytseva@student.ru"
            },
            new Student {
                UserName = "Владимир Павлов",
                Photo = "icon.png",
                GroupName = "ИТ-105",
                Email = "vladimir.pavlov@student.ru"
            },
            new Student {
                UserName = "Светлана Семенова",
                Photo = "icon.png",
                GroupName = "ИТ-104",
                Email = "svetlana.semenova@student.ru"
            },
            new Student {
                UserName = "Михаил Голубев",
                Photo = "icon.png",
                GroupName = "ИТ-103",
                Email = "mikhail.golubev@student.ru"
            },
            new Student {
                UserName = "Юлия Виноградова",
                Photo = "icon.png",
                GroupName = "ИТ-101",
                Email = "yulia.vinogradova@student.ru"
            },
            new Student {
                UserName = "Артем Богданов",
                Photo = "icon.png",
                GroupName = "ИТ-105",
                Email = "artem.bogdanov@student.ru"
            },
            new Student {
                UserName = "Алина Воробьева",
                Photo = "icon.png",
                GroupName = "ИТ-102",
                Email = "alina.vorobyova@student.ru"
            },
            new Student {
                UserName = "Константин Фролов",
                Photo = "icon.png",
                GroupName = "ИТ-104",
                Email = "konstantin.frolov@student.ru"
            },
            new Student {
                UserName = "Лариса Лебедева",
                Photo = "icon.png",
                GroupName = "ИТ-103",
                Email = "larisa.lebedeva@student.ru"
            },
            new Student {
                UserName = "Роман Егоров",
                Photo = "icon.png",
                GroupName = "ИТ-101",
                Email = "roman.egorov@student.ru"
            }
        };

        foreach (var student in _allStudents)
        {
            _filteredStudents.Add(student);
        }

        Collection1.ItemsSource = _filteredStudents;
        UpdateStatusLabel();

        await Task.Delay(200);
        Refresh.IsRefreshing = false;
    }

    private void SearchBar1_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower() ?? "";

        _filteredStudents.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var student in _allStudents)
            {
                _filteredStudents.Add(student);
            }
        }
        else
        {
            foreach (var student in _allStudents)
            {
                if (student.UserName.ToLower().Contains(searchText) ||
                    student.GroupName.ToLower().Contains(searchText))
                {
                    _filteredStudents.Add(student);
                }
            }
        }

        UpdateStatusLabel();
    }

    private void Refresh_Refreshing(object sender, EventArgs e)
    {
        LoadStudents();
    }

    private void UpdateStatusLabel()
    {
        StatusLabel.Text = $"Загружено записей: {_filteredStudents.Count} из {_allStudents.Count}";
    }

    private async void BtnAdd_Clicked(object sender, EventArgs e)
    {
        var editPage = new StudentEditPage();
        editPage.StudentSaved += OnStudentSaved;
        await Navigation.PushAsync(editPage);
    }

    private async void BtnHelp_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("О программе", "Левин Кирилл\n23-ИСП3-9", "OK");
    }

    private async void OnStudentTapped(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Student selectedStudent)
        {
            var editPage = new StudentEditPage(selectedStudent);
            editPage.StudentSaved += OnStudentSaved;
            editPage.StudentDeleted += OnStudentDeleted;
            await Navigation.PushAsync(editPage);
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private void OnStudentSaved(object sender, StudentEventArgs e)
    {
        if (e.IsAddMode)
        {
            _allStudents.Add(e.Student);
            _filteredStudents.Add(e.Student);
        }
        else
        {
            var index = _allStudents.IndexOf(e.Student);
            if (index >= 0)
            {
                _allStudents[index] = e.Student;

                var filteredIndex = _filteredStudents.IndexOf(e.Student);
                if (filteredIndex >= 0)
                {
                    _filteredStudents[filteredIndex] = e.Student;
                }
            }
        }
        UpdateStatusLabel();
    }

    private void OnStudentDeleted(object sender, StudentEventArgs e)
    {
        _allStudents.Remove(e.Student);
        _filteredStudents.Remove(e.Student);
        UpdateStatusLabel();
    }
}

public class StudentEventArgs : EventArgs
{
    public Student Student { get; set; }
    public bool IsAddMode { get; set; }
}