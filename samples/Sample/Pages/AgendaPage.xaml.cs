namespace Sample.Pages;

public partial class AgendaPage : ContentPage
{
    public AgendaPage()
    {
        InitializeComponent();

        // Add a timezone ~3 hours offset from local for demo
        var localOffset = TimeZoneInfo.Local.BaseUtcOffset;
        var targetOffset = localOffset + TimeSpan.FromHours(3);
        var tz = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(t => t.BaseUtcOffset == targetOffset)
            ?? TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(t => Math.Abs((t.BaseUtcOffset - targetOffset).TotalMinutes))
                .First();
        AgendaView.AdditionalTimezones.Add(tz);
    }
}