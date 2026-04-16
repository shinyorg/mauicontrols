using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

public class NumberPickerCell : CellBase
{
    [Parameter] public double Value { get; set; }
    [Parameter] public EventCallback<double> ValueChanged { get; set; }
    [Parameter] public double Minimum { get; set; } = double.MinValue;
    [Parameter] public double Maximum { get; set; } = double.MaxValue;
    [Parameter] public double Step { get; set; } = 1;

    protected override Task OnTapped() => Task.CompletedTask;

    async Task HandleChange(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value as string, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v))
        {
            Value = Math.Clamp(v, Minimum, Maximum);
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
        }
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        builder.OpenElement(sequence, "input");
        builder.AddAttribute(sequence + 1, "type", "number");
        builder.AddAttribute(sequence + 2, "class", "shiny-tv-input");
        builder.AddAttribute(sequence + 3, "value", Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        builder.AddAttribute(sequence + 4, "step", Step.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (Minimum > double.MinValue)
            builder.AddAttribute(sequence + 5, "min", Minimum.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (Maximum < double.MaxValue)
            builder.AddAttribute(sequence + 6, "max", Maximum.ToString(System.Globalization.CultureInfo.InvariantCulture));
        builder.AddAttribute(sequence + 7, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));
        builder.CloseElement();
    }
}
