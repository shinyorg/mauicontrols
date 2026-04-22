namespace Shiny.Maui.Controls.ImageEditor;

public sealed class EditedImage
{
    readonly Microsoft.Maui.Graphics.IImage image;
    readonly ImageEditorState state;

    internal EditedImage(Microsoft.Maui.Graphics.IImage image, ImageEditorState state)
    {
        this.image = image;
        this.state = state;
    }

    public Task<Stream> ToStreamAsync(ImageExportFormat format = ImageExportFormat.Png, float quality = 0.92f, int? width = null, int? height = null)
    {
        return Task.Run(() => Render(format, quality, width, height));
    }

    Stream Render(ImageExportFormat format, float quality, int? width, int? height)
    {
        var targetWidth = width ?? (int)image.Width;
        var targetHeight = height ?? (int)image.Height;

        var exportDrawable = new ImageEditorDrawable
        {
            Image = image,
            ToolMode = ImageEditorToolMode.Move
        };

        foreach (var action in state.Actions)
            exportDrawable.State.Push(action);

#if IOS || MACCATALYST || ANDROID
        using var context = new Microsoft.Maui.Graphics.Platform.PlatformBitmapExportContext(targetWidth, targetHeight, 1f);
        exportDrawable.Draw(context.Canvas, new RectF(0, 0, targetWidth, targetHeight));

        var rendered = context.Image;
        var result = rendered.AsStream(format.ToBitmapFormat(), quality);
        return result;
#else
        return Stream.Null;
#endif
    }
}

internal static class ImageExportFormatExtensions
{
    public static Microsoft.Maui.Graphics.ImageFormat ToBitmapFormat(this ImageExportFormat format) => format switch
    {
        ImageExportFormat.Jpeg => Microsoft.Maui.Graphics.ImageFormat.Jpeg,
        ImageExportFormat.Png => Microsoft.Maui.Graphics.ImageFormat.Png,
        _ => Microsoft.Maui.Graphics.ImageFormat.Png
    };
}
