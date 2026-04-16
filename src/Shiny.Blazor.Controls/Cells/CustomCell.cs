using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

/// <summary>
/// A cell that hands the entire row body over to a user-supplied <see cref="ChildContent"/> render fragment.
/// Useful when you need fully custom layout but still want the row hover/selection behaviour.
/// </summary>
public class CustomCell : ComponentBase
{
    [CascadingParameter] public TableView? ParentTableView { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool IsSelectable { get; set; } = true;
    [Parameter] public bool IsEnabled { get; set; } = true;
    [Parameter] public string? CellBackgroundColor { get; set; }
    [Parameter] public EventCallback Tapped { get; set; }

    async Task HandleClick(MouseEventArgs e)
    {
        if (!IsEnabled || !IsSelectable) return;
        if (Tapped.HasDelegate)
            await Tapped.InvokeAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var sb = new System.Text.StringBuilder();
        var bg = CellBackgroundColor ?? ParentTableView?.CellBackgroundColor;
        if (!string.IsNullOrEmpty(bg)) sb.Append("background-color:").Append(bg).Append(';');
        if (!IsEnabled) sb.Append("opacity:0.4;pointer-events:none;");

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "shiny-tv-cell");
        builder.AddAttribute(2, "style", sb.ToString());
        builder.AddAttribute(3, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, HandleClick));
        if (ChildContent is not null)
            builder.AddContent(4, ChildContent);
        builder.CloseElement();
    }
}
