namespace Shiny.Maui.Controls.ImageEditor.EditActions;

public sealed class RotateAction : IEditAction
{
    public string Description => "Rotate";
    public required float AngleDegrees { get; init; }
}
