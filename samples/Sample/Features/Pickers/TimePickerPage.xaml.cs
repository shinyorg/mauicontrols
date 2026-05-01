using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sample.Features.Pickers;

public partial class TimePickerPage : Shiny.Maui.Controls.FloatingPanel.ShinyContentPage
{
    public TimePickerPage()
    {
        InitializeComponent();
        BindingContext = new TimePickerViewModel();
    }
}

public class TimePickerViewModel : INotifyPropertyChanged
{
    TimeSpan? selectedTime;
    TimeSpan? militaryTime;
    TimeSpan? intervalTime;

    public TimeSpan? SelectedTime { get => selectedTime; set => SetProperty(ref selectedTime, value); }
    public TimeSpan? MilitaryTime { get => militaryTime; set => SetProperty(ref militaryTime, value); }
    public TimeSpan? IntervalTime { get => intervalTime; set => SetProperty(ref intervalTime, value); }

    public event PropertyChangedEventHandler? PropertyChanged;

    bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
