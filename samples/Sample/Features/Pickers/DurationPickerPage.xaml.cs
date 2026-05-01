using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sample.Features.Pickers;

public partial class DurationPickerPage : Shiny.Maui.Controls.FloatingPanel.ShinyContentPage
{
    public DurationPickerPage()
    {
        InitializeComponent();
        BindingContext = new DurationPickerViewModel();
    }
}

public class DurationPickerViewModel : INotifyPropertyChanged
{
    TimeSpan? selectedDuration;
    TimeSpan? constrainedDuration;
    TimeSpan? preciseDuration;

    public TimeSpan? SelectedDuration { get => selectedDuration; set => SetProperty(ref selectedDuration, value); }
    public TimeSpan? ConstrainedDuration { get => constrainedDuration; set => SetProperty(ref constrainedDuration, value); }
    public TimeSpan? PreciseDuration { get => preciseDuration; set => SetProperty(ref preciseDuration, value); }

    public event PropertyChangedEventHandler? PropertyChanged;

    bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
