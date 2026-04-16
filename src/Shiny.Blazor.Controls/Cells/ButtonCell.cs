using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

/// <summary>
/// A full-width button-style cell. Ignores icon/description — renders a centered action label.
/// </summary>
public class ButtonCell : ComponentBase
{
    [CascadingParameter] public TableView? ParentTableView { get; set; }

    [Parameter] public string? Title { get; set; }
    [Parameter] public string? ButtonTextColor { get; set; }
    [Parameter] public string TitleAlignment { get; set; } = "center";
    [Parameter] public double TitleFontSize { get; set; } = 16;
    [Parameter] public bool IsEnabled { get; set; } = true;

    [Parameter] public EventCallback OnClick { get; set; }

    async Task HandleClick(MouseEventArgs e)
    {
        if (!IsEnabled) return;
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var color = ButtonTextColor ?? ParentTableView?.CellAccentColor ?? "#2196F3";
        var style = $"color:{color};text-align:{TitleAlignment};font-size:{TitleFontSize}px;";
        if (!IsEnabled) style += "opacity:0.4;cursor:not-allowed;";

        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "class", "shiny-tv-button");
        builder.AddAttribute(3, "style", style);
        builder.AddAttribute(4, "disabled", !IsEnabled);
        builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, HandleClick));
        builder.AddContent(6, Title ?? "");
        builder.CloseElement();
    }
}
