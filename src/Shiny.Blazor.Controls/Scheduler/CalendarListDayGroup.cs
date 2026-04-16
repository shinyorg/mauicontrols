namespace Shiny.Blazor.Controls.Scheduler;

public class CalendarListDayGroup : List<SchedulerEvent>
{
    public CalendarListDayGroup(DateOnly date, IEnumerable<SchedulerEvent> events) : base(events)
    {
        Date = date;
        DateDisplay = date.ToString("dddd, MMMM d, yyyy");
        IsToday = date == DateOnly.FromDateTime(DateTime.Today);
        EventCountDisplay = Count == 1 ? "1 event" : $"{Count} events";
    }

    public DateOnly Date { get; }
    public string DateDisplay { get; }
    public bool IsToday { get; }
    public string EventCountDisplay { get; }
}
