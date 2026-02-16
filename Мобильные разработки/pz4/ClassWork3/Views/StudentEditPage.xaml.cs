using ClassWork3.Models;

namespace ClassWork3.Views;

public partial class StudentEditPage : ContentPage
{
    public Student CurrentStudent { get; set; }
    public bool IsAddMode { get; set; }
    public bool IsEditMode => !IsAddMode;
    public string PageTitle => IsAddMode ? "Добавление студента" : "Редактирование студента";

    public string StudentName
    {
        get => CurrentStudent?.UserName;
        set { if (CurrentStudent != null) CurrentStudent.UserName = value; }
    }

    public string GroupName
    {
        get => CurrentStudent?.GroupName;
        set { if (CurrentStudent != null) CurrentStudent.GroupName = value; }
    }

    public string Email { get; set; }
    public string Password { get; set; }

    public event EventHandler<StudentEventArgs> StudentSaved;
    public event EventHandler<StudentEventArgs> StudentDeleted;

    public StudentEditPage(Student student = null)
    {
        InitializeComponent();

        IsAddMode = student == null;
        CurrentStudent = IsAddMode ? new Student { Photo = "icon.png" } : student;

        if (!IsAddMode && CurrentStudent != null)
        {
            Email = CurrentStudent.Email;
        }

        BindingContext = this;
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        CurrentStudent.Email = Email;

        StudentSaved?.Invoke(this, new StudentEventArgs
        {
            Student = CurrentStudent,
            IsAddMode = IsAddMode
        });

        await Navigation.PopAsync();
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Подтверждение", "Удалить студента?", "Да", "Нет");
        if (confirm)
        {
            StudentDeleted?.Invoke(this, new StudentEventArgs
            {
                Student = CurrentStudent
            });

            await Navigation.PopAsync();
        }
    }
}