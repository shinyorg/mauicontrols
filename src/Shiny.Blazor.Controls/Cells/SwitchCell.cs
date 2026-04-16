using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

public class SwitchCell : CellBase
{
    [Parameter] public bool On { get; set; }
    [Parameter] public EventCallback<bool> OnChanged { get; set; }
    [Parameter] public string? OnColor { get; set; }

    protected override bool ShouldKeepSelectionHighlight() => false;

    protected override async Task OnTapped()
    {
        await SetOn(!On);
    }

    async Task HandleInputChanged(ChangeEventArgs e)
    {
        var value = e.Value is bool b ? b : false;
        await SetOn(value);
    }

    async Task SetOn(bool value)
    {
        if (On == value) return;
        On = value;
        if (OnChanged.HasDelegate)
            await OnChanged.InvokeAsync(On);
        StateHasChanged();
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        var color = OnColor ?? ResolveAccentColor();
        var style = $"--shiny-tv-switch-on:{color};";

        builder.OpenElement(sequence, "label");
        builder.AddAttribute(sequence + 1, "class", "shiny-tv-switch");
        builder.AddAttribute(sequence + 2, "style", style);

        builder.OpenElement(sequence + 3, "input");
        builder.AddAttribute(sequence + 4, "type", "checkbox");
        builder.AddAttribute(sequence + 5, "checked", On);
        builder.AddAttribute(sequence + 6, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleInputChanged));
        builder.CloseElement();

        builder.OpenElement(sequence + 7, "span");
        builder.AddAttribute(sequence + 8, "class", "shiny-tv-switch-slider");
        builder.CloseElement();

        builder.CloseElement();
    }
}
