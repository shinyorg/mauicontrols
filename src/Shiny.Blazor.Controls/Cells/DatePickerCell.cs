using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

public class DatePickerCell : CellBase
{
    [Parameter] public DateTime? Date { get; set; }
    [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }
    [Parameter] public DateTime? MinimumDate { get; set; }
    [Parameter] public DateTime? MaximumDate { get; set; }

    protected override Task OnTapped() => Task.CompletedTask;

    async Task HandleChange(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value as string, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var v))
        {
            Date = v;
            if (DateChanged.HasDelegate)
                await DateChanged.InvokeAsync(Date);
        }
        else
        {
            Date = null;
            if (DateChanged.HasDelegate)
                await DateChanged.InvokeAsync(null);
        }
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        builder.OpenElement(sequence, "input");
        builder.AddAttribute(sequence + 1, "type", "date");
        builder.AddAttribute(sequence + 2, "class", "shiny-tv-input");
        builder.AddAttribute(sequence + 3, "value", Date?.ToString("yyyy-MM-dd") ?? "");
        if (MinimumDate.HasValue)
            builder.AddAttribute(sequence + 4, "min", MinimumDate.Value.ToString("yyyy-MM-dd"));
        if (MaximumDate.HasValue)
            builder.AddAttribute(sequence + 5, "max", MaximumDate.Value.ToString("yyyy-MM-dd"));
        builder.AddAttribute(sequence + 6, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));
        builder.CloseElement();
    }
}
