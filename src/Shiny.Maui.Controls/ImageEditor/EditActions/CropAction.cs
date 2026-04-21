namespace Shiny.Maui.Controls.ImageEditor.EditActions;

public sealed class CropAction : IEditAction
{
    public string Description => "Crop";
    public required RectF CropRect { get; init; }
}
