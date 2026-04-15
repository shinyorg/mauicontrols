namespace Shiny.Maui.Controls.Scheduler;

public class DatePickerItemContext
{
    public DateOnly Date { get; set; }
    public string DayNumber { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public string MonthName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public bool IsToday { get; set; }
}