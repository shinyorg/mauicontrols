using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

public class CheckboxCell : CellBase
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public string? AccentColor { get; set; }

    protected override async Task OnTapped() => await SetChecked(!Checked);

    async Task HandleChanged(ChangeEventArgs e)
        => await SetChecked(e.Value is bool b && b);

    async Task SetChecked(bool value)
    {
        if (Checked == value) return;
        Checked = value;
        if (CheckedChanged.HasDelegate)
            await CheckedChanged.InvokeAsync(Checked);
        StateHasChanged();
    }

    protected override void BuildAccessory(RenderTreeBuilder builder, int sequence)
    {
        var accent = AccentColor ?? ResolveAccentColor();
        builder.OpenElement(sequence, "input");
        builder.AddAttribute(sequence + 1, "type", "checkbox");
        builder.AddAttribute(sequence + 2, "class", "shiny-tv-checkbox");
        builder.AddAttribute(sequence + 3, "style", $"--shiny-tv-accent:{accent};accent-color:{accent};");
        builder.AddAttribute(sequence + 4, "checked", Checked);
        builder.AddAttribute(sequence + 5, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChanged));
        builder.CloseElement();
    }
}
