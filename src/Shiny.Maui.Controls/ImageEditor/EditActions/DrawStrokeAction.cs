namespace Shiny.Maui.Controls.ImageEditor.EditActions;

public sealed class DrawStrokeAction : IEditAction
{
    public string Description => "Draw";
    public required PointF[] Points { get; init; }
    public required Color StrokeColor { get; init; }
    public required float StrokeWidth { get; init; }
}
