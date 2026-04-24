namespace Shiny.Maui.Controls.ImageEditor.EditActions;

public sealed class TextAnnotationAction : IEditAction
{
    public string Description => "Text";
    public required string Text { get; init; }
    public required PointF Position { get; init; }
    public required float FontSize { get; init; }
    public required Color TextColor { get; init; }
    public string? FontFamily { get; init; }
}
