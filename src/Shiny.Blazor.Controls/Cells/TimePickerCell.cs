using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

public class TimePickerCell : CellBase
{
    [Parameter] public TimeSpan? Time { get; set; }
    [Parameter] public EventCallback<TimeSpan?> TimeChanged { get; set; }

    protected override Task OnTapped() => Task.CompletedTask;

    async Task HandleChange(ChangeEventArgs e)
    {
        if (TimeSpan.TryParseExact(e.Value as string, "hh\\:mm", System.Globalization.CultureInfo.InvariantCulture, out var v)
            || TimeSpan.TryParse(e.Value as string, System.Globalization.CultureInfo.InvariantCulture, out v))
        {
            Time = v;
        }
        else
        {
            Time = null;
        }
        if (TimeChanged.HasDelegate)
            await TimeChanged.InvokeAsync(Time);
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        builder.OpenElement(sequence, "input");
        builder.AddAttribute(sequence + 1, "type", "time");
        builder.AddAttribute(sequence + 2, "class", "shiny-tv-input");
        builder.AddAttribute(sequence + 3, "value", Time.HasValue ? $"{Time.Value.Hours:D2}:{Time.Value.Minutes:D2}" : "");
        builder.AddAttribute(sequence + 4, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));
        builder.CloseElement();
    }
}
