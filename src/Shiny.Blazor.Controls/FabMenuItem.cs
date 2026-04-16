namespace Shiny.Blazor.Controls;

/// <summary>
/// Data record describing a menu item used by <see cref="FabMenu"/>.
/// </summary>
public class FabMenuItem
{
    public string? Icon { get; set; }
    public string? Text { get; set; }
    public string FabBackgroundColor { get; set; } = "#2196F3";
    public string TextColor { get; set; } = "#000000";
    public string LabelBackgroundColor { get; set; } = "#FFFFFF";
    public double Size { get; set; } = 44;
    public double IconSize { get; set; } = 20;
    public double FontSize { get; set; } = 13;
    public object? Tag { get; set; }
}
