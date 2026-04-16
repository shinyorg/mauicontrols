using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Shiny.Blazor.Controls.Cells;

public class RadioCell : CellBase
{
    [Parameter] public bool Checked { get; set; }
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
    [Parameter] public string? AccentColor { get; set; }
    [Parameter] public string? GroupName { get; set; }

    protected override async Task OnTapped() => await SetChecked(true);

    async Task HandleChange(ChangeEventArgs e) => await SetChecked(e.Value is bool b && b);

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
        builder.AddAttribute(sequence + 1, "type", "radio");
        builder.AddAttribute(sequence + 2, "class", "shiny-tv-radio");
        builder.AddAttribute(sequence + 3, "style", $"accent-color:{accent};");
        if (!string.IsNullOrEmpty(GroupName))
            builder.AddAttribute(sequence + 4, "name", GroupName);
        builder.AddAttribute(sequence + 5, "checked", Checked);
        builder.AddAttribute(sequence + 6, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleChange));
        builder.CloseElement();
    }
}
