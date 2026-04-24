namespace Shiny.Maui.Controls.ImageEditor.EditActions;

public sealed class LineAction : IEditAction
{
    public string Description => IsArrow ? "Arrow" : "Line";
    public required PointF Start { get; init; }
    public required PointF End { get; init; }
    public required Color StrokeColor { get; init; }
    public required float StrokeWidth { get; init; }
    public bool IsArrow { get; init; }
}
