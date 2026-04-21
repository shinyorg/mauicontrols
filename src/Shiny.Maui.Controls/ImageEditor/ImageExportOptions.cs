namespace Shiny.Maui.Controls.ImageEditor;

public sealed class ImageExportOptions
{
    public ImageExportFormat Format { get; set; } = ImageExportFormat.Png;
    public float Quality { get; set; } = 0.92f;
    public int? Width { get; set; }
    public int? Height { get; set; }
}
