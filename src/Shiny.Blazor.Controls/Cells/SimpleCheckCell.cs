using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Shiny.Blazor.Controls.Cells;

/// <summary>
/// Cell that displays a checkmark when selected (no toggle control).
/// </summary>
public class SimpleCheckCell : CellBase
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public string? AccentColor { get; set; }

    protected override async Task OnTapped()
    {
        Checked = !Checked;
        if (CheckedChanged.HasDelegate)
            await CheckedChanged.InvokeAsync(Checked);
        StateHasChanged();
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        if (!Checked) return;

        var accent = AccentColor ?? ResolveAccentColor();
        builder.OpenElement(sequence, "svg");
        builder.AddAttribute(sequence + 1, "class", "shiny-tv-check-mark");
        builder.AddAttribute(sequence + 2, "viewBox", "0 0 20 20");
        builder.AddAttribute(sequence + 3, "fill", "none");
        builder.AddAttribute(sequence + 4, "style", $"color:{accent};");
        builder.OpenElement(sequence + 5, "path");
        builder.AddAttribute(sequence + 6, "d", "M5 10l3 3 7-7");
        builder.AddAttribute(sequence + 7, "stroke", "currentColor");
        builder.AddAttribute(sequence + 8, "stroke-width", "2.5");
        builder.AddAttribute(sequence + 9, "stroke-linecap", "round");
        builder.AddAttribute(sequence + 10, "stroke-linejoin", "round");
        builder.CloseElement();
        builder.CloseElement();
    }
}
