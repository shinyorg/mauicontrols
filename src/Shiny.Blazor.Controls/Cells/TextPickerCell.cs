using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

/// <summary>
/// String selection cell rendered using a native &lt;select&gt; in the accessory area.
/// </summary>
public class TextPickerCell : CellBase
{
    [Parameter] public IReadOnlyList<string>? Items { get; set; }
    [Parameter] public string? SelectedItem { get; set; }
    [Parameter] public EventCallback<string?> SelectedItemChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }

    protected override Task OnTapped() => Task.CompletedTask;

    async Task HandleChange(ChangeEventArgs e)
    {
        SelectedItem = e.Value as string;
        if (SelectedItemChanged.HasDelegate)
            await SelectedItemChanged.InvokeAsync(SelectedItem);
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        builder.OpenElement(sequence, "select");
        builder.AddAttribute(sequence + 1, "class", "shiny-tv-input");
        builder.AddAttribute(sequence + 2, "value", SelectedItem ?? "");
        builder.AddAttribute(sequence + 3, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));

        if (!string.IsNullOrEmpty(Placeholder))
        {
            builder.OpenElement(sequence + 4, "option");
            builder.AddAttribute(sequence + 5, "value", "");
            builder.AddContent(sequence + 6, Placeholder);
            builder.CloseElement();
        }

        if (Items is not null)
        {
            var i = 100;
            foreach (var item in Items)
            {
                builder.OpenElement(sequence + i, "option");
                builder.AddAttribute(sequence + i + 1, "value", item);
                builder.AddContent(sequence + i + 2, item);
                builder.CloseElement();
                i += 3;
            }
        }

        builder.CloseElement();
    }
}
