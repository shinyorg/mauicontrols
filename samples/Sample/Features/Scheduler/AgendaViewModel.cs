using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Shiny.Maui.Controls.Scheduler;

namespace Sample.Features.Scheduler;

public class AgendaViewModel(ISchedulerEventProvider provider) : INotifyPropertyChanged
{
    DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);
    int daysToShow = 1;
    bool showAdditionalTimezones;

    public ISchedulerEventProvider Provider => provider;

    public DateOnly SelectedDate
    {
        get => selectedDate;
        set { selectedDate = value; OnPropertyChanged(); }
    }

    public int DaysToShow
    {
        get => daysToShow;
        set { daysToShow = value; OnPropertyChanged(); OnPropertyChanged(nameof(DaysToggleText)); }
    }

    public bool ShowAdditionalTimezones
    {
        get => showAdditionalTimezones;
        set { showAdditionalTimezones = value; OnPropertyChanged(); OnPropertyChanged(nameof(TimezoneToggleText)); }
    }

    public string DaysToggleText => DaysToShow == 1 ? "3-Day" : "1-Day";
    public string TimezoneToggleText => ShowAdditionalTimezones ? "Hide TZ" : "Show TZ";

    public ICommand ToggleDaysCommand => new Command(() => DaysToShow = DaysToShow == 1 ? 3 : 1);
    public ICommand ToggleTimezonesCommand => new Command(() => ShowAdditionalTimezones = !ShowAdditionalTimezones);

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
