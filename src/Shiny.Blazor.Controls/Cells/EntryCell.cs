using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

public class EntryCell : CellBase
{
    [Parameter] public string? ValueText { get; set; }
    [Parameter] public EventCallback<string> ValueTextChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool IsPassword { get; set; }
    [Parameter] public int MaxLength { get; set; } = -1;
    [Parameter] public string TextAlignment { get; set; } = "right";
    [Parameter] public string? ValueTextColor { get; set; }
    [Parameter] public double ValueTextFontSize { get; set; } = -1;

    [Parameter] public EventCallback<string?> Completed { get; set; }

    protected override Task OnTapped() => Task.CompletedTask;

    async Task HandleInput(ChangeEventArgs e)
    {
        var v = e.Value as string ?? string.Empty;
        ValueText = v;
        if (ValueTextChanged.HasDelegate)
            await ValueTextChanged.InvokeAsync(v);
    }

    async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && Completed.HasDelegate)
            await Completed.InvokeAsync(ValueText);
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        var color = ValueTextColor ?? ResolveValueColor();
        var size = ResolveDouble(ValueTextFontSize, ParentTableView?.CellValueTextFontSize ?? -1, 14);
        var sb = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(color)) sb.Append("color:").Append(color).Append(';');
        sb.Append("font-size:").Append(size).Append("px;");
        sb.Append("text-align:").Append(TextAlignment).Append(';');

        builder.OpenElement(sequence, "input");
        builder.AddAttribute(sequence + 1, "type", IsPassword ? "password" : "text");
        builder.AddAttribute(sequence + 2, "class", "shiny-tv-input");
        builder.AddAttribute(sequence + 3, "style", sb.ToString());
        builder.AddAttribute(sequence + 4, "value", ValueText ?? "");
        builder.AddAttribute(sequence + 5, "placeholder", Placeholder ?? "");
        if (MaxLength > 0)
            builder.AddAttribute(sequence + 6, "maxlength", MaxLength);
        builder.AddAttribute(sequence + 7, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleInput));
        builder.AddAttribute(sequence + 8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));
        builder.CloseElement();
    }
}
