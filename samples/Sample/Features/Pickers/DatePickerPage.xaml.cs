using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sample.Features.Pickers;

public partial class DatePickerPage : Shiny.Maui.Controls.FloatingPanel.ShinyContentPage
{
    public DatePickerPage()
    {
        InitializeComponent();
        BindingContext = new DatePickerViewModel();
    }
}

public class DatePickerViewModel : INotifyPropertyChanged
{
    DateOnly? selectedDate;
    DateOnly? constrainedDate;
    DateOnly? formattedDate;

    public DateOnly? SelectedDate { get => selectedDate; set => SetProperty(ref selectedDate, value); }
    public DateOnly? ConstrainedDate { get => constrainedDate; set => SetProperty(ref constrainedDate, value); }
    public DateOnly? FormattedDate { get => formattedDate; set => SetProperty(ref formattedDate, value); }

    public DateOnly MinDate => DateOnly.FromDateTime(DateTime.Today);
    public DateOnly MaxDate => DateOnly.FromDateTime(DateTime.Today.AddDays(30));

    public event PropertyChangedEventHandler? PropertyChanged;

    bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
