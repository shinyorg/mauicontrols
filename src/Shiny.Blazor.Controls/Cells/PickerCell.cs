using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

/// <summary>
/// Generic picker cell that shows the current selection and a chevron.
/// Binds to a string list; use <see cref="TextPickerCell"/> for concrete string selection.
/// </summary>
public class PickerCell : CellBase
{
    [Parameter] public IReadOnlyList<string>? Items { get; set; }
    [Parameter] public string? SelectedItem { get; set; }
    [Parameter] public EventCallback<string?> SelectedItemChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; } = "Select...";

    protected override async Task OnTapped()
    {
        if (Items is null || Items.Count == 0) return;

        // Advance to next item on tap (simple iOS-like behaviour).
        var currentIndex = SelectedItem is null ? -1 : Items.ToList().IndexOf(SelectedItem);
        var next = (currentIndex + 1) % Items.Count;
        SelectedItem = Items[next];
        if (SelectedItemChanged.HasDelegate)
            await SelectedItemChanged.InvokeAsync(SelectedItem);
        StateHasChanged();
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        builder.OpenElement(sequence, "span");
        builder.AddAttribute(sequence + 1, "class", "shiny-tv-value");
        builder.AddContent(sequence + 2, SelectedItem ?? Placeholder ?? "");
        builder.CloseElement();

        builder.OpenElement(sequence + 10, "svg");
        builder.AddAttribute(sequence + 11, "class", "shiny-tv-chevron");
        builder.AddAttribute(sequence + 12, "viewBox", "0 0 16 16");
        builder.AddAttribute(sequence + 13, "fill", "none");
        builder.OpenElement(sequence + 14, "path");
        builder.AddAttribute(sequence + 15, "d", "M4 6l4 4 4-4");
        builder.AddAttribute(sequence + 16, "stroke", "currentColor");
        builder.AddAttribute(sequence + 17, "stroke-width", "2");
        builder.AddAttribute(sequence + 18, "stroke-linecap", "round");
        builder.AddAttribute(sequence + 19, "stroke-linejoin", "round");
        builder.CloseElement();
        builder.CloseElement();
    }
}
