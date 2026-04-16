using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Shiny.Blazor.Controls.Cells;

/// <summary>
/// Read-only cell that displays a value to the right of the title (iOS settings style).
/// </summary>
public class LabelCell : CellBase
{
    [Parameter] public string? ValueText { get; set; }
    [Parameter] public string? ValueTextColor { get; set; }
    [Parameter] public double ValueTextFontSize { get; set; } = -1;

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        if (string.IsNullOrEmpty(ValueText)) return;

        var color = ValueTextColor ?? ResolveValueColor();
        var size = ResolveDouble(ValueTextFontSize, ParentTableView?.CellValueTextFontSize ?? -1, 14);
        var sb = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(color)) sb.Append("color:").Append(color).Append(';');
        sb.Append("font-size:").Append(size).Append("px;");

        builder.OpenElement(sequence, "span");
        builder.AddAttribute(sequence + 1, "class", "shiny-tv-value");
        builder.AddAttribute(sequence + 2, "style", sb.ToString());
        builder.AddContent(sequence + 3, ValueText);
        builder.CloseElement();
    }
}
