using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Shiny.Blazor.Controls.Cells;

/// <summary>
/// Cell that shows a chevron indicating a navigation/action target.
/// </summary>
public class CommandCell : CellBase
{
    [Parameter] public bool ShowChevron { get; set; } = true;
    [Parameter] public string? AccessoryText { get; set; }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        if (!string.IsNullOrEmpty(AccessoryText))
        {
            builder.OpenElement(sequence, "span");
            builder.AddAttribute(sequence + 1, "class", "shiny-tv-value");
            builder.AddContent(sequence + 2, AccessoryText);
            builder.CloseElement();
        }

        if (!ShowChevron) return;

        builder.OpenElement(sequence + 10, "svg");
        builder.AddAttribute(sequence + 11, "class", "shiny-tv-chevron");
        builder.AddAttribute(sequence + 12, "viewBox", "0 0 16 16");
        builder.AddAttribute(sequence + 13, "fill", "none");
        builder.OpenElement(sequence + 14, "path");
        builder.AddAttribute(sequence + 15, "d", "M6 4l4 4-4 4");
        builder.AddAttribute(sequence + 16, "stroke", "currentColor");
        builder.AddAttribute(sequence + 17, "stroke-width", "2");
        builder.AddAttribute(sequence + 18, "stroke-linecap", "round");
        builder.AddAttribute(sequence + 19, "stroke-linejoin", "round");
        builder.CloseElement();
        builder.CloseElement();
    }
}
