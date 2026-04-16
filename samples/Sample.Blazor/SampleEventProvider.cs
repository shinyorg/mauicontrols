using Shiny.Blazor.Controls.Scheduler;

namespace Sample.Blazor;

public class SampleEventProvider : ISchedulerEventProvider
{
    readonly List<SchedulerEvent> events;

    public SampleEventProvider()
    {
        var today = DateTime.Today;
        var rnd = new Random(42);
        var palette = new[] { "#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#8B5CF6", "#EC4899" };
        var titles = new[]
        {
            "Standup", "Design review", "1:1 with manager", "Lunch", "Code review",
            "Client call", "Team sync", "Demo prep", "Planning", "Retro", "Workshop"
        };

        events = new List<SchedulerEvent>();
        for (var d = -10; d <= 30; d++)
        {
            var day = today.AddDays(d);
            var count = rnd.Next(0, 4);
            for (var i = 0; i < count; i++)
            {
                var hour = rnd.Next(8, 18);
                var len = rnd.Next(1, 3);
                events.Add(new SchedulerEvent
                {
                    Title = titles[rnd.Next(titles.Length)],
                    Description = "Auto-generated demo event",
                    Color = palette[rnd.Next(palette.Length)],
                    Start = new DateTimeOffset(day.AddHours(hour)),
                    End = new DateTimeOffset(day.AddHours(hour + len))
                });
            }
            if (rnd.NextDouble() < 0.15)
            {
                events.Add(new SchedulerEvent
                {
                    Title = "All-day event",
                    Color = "#6366F1",
                    IsAllDay = true,
                    Start = new DateTimeOffset(day),
                    End = new DateTimeOffset(day.AddDays(1))
                });
            }
        }
    }

    public string? LastSelectedEvent { get; private set; }
    public DateOnly? LastSelectedCalendarDate { get; private set; }
    public DateTimeOffset? LastSelectedAgendaTime { get; private set; }

    public Task<IReadOnlyList<SchedulerEvent>> GetEvents(DateTimeOffset start, DateTimeOffset end)
    {
        var inRange = events
            .Where(e => e.End >= start && e.Start < end)
            .ToList();
        return Task.FromResult<IReadOnlyList<SchedulerEvent>>(inRange);
    }

    public void OnEventSelected(SchedulerEvent selectedEvent)
        => LastSelectedEvent = $"{selectedEvent.Title} ({selectedEvent.Start.LocalDateTime:g})";

    public bool CanCalendarSelect(DateOnly selectedDate) => true;
    public void OnCalendarDateSelected(DateOnly selectedDate) => LastSelectedCalendarDate = selectedDate;

    public void OnAgendaTimeSelected(DateTimeOffset selectedTime) => LastSelectedAgendaTime = selectedTime;
    public bool CanSelectAgendaTime(DateTimeOffset selectedTime) => true;
}
