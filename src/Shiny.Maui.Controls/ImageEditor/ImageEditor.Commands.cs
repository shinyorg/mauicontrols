using System.Windows.Input;
using Shiny.Maui.Controls.ImageEditor.EditActions;

namespace Shiny.Maui.Controls.ImageEditor;

public partial class ImageEditor
{
    public ICommand UndoCommand { get; private set; } = null!;
    public ICommand RedoCommand { get; private set; } = null!;
    public ICommand RotateCommand { get; private set; } = null!;
    public ICommand ResetCommand { get; private set; } = null!;
    public ICommand CropCommand { get; private set; } = null!;
    public ICommand DrawCommand { get; private set; } = null!;
    public ICommand TextCommand { get; private set; } = null!;

    void SetupCommands()
    {
        UndoCommand = new Command(Undo, () => state.CanUndo);
        RedoCommand = new Command(Redo, () => state.CanRedo);
        RotateCommand = new Command<float>(Rotate);
        ResetCommand = new Command(Reset);
        CropCommand = new Command(() => CurrentToolMode = CurrentToolMode == ImageEditorToolMode.Crop
            ? ImageEditorToolMode.None
            : ImageEditorToolMode.Crop);
        DrawCommand = new Command(() => CurrentToolMode = CurrentToolMode == ImageEditorToolMode.Draw
            ? ImageEditorToolMode.None
            : ImageEditorToolMode.Draw);
        TextCommand = new Command(() => CurrentToolMode = CurrentToolMode == ImageEditorToolMode.Text
            ? ImageEditorToolMode.None
            : ImageEditorToolMode.Text);

        state.StateChanged += () =>
        {
            ((Command)UndoCommand).ChangeCanExecute();
            ((Command)RedoCommand).ChangeCanExecute();
        };
    }

    public void Undo()
    {
        state.Undo();
        Invalidate();
    }

    public void Redo()
    {
        state.Redo();
        Invalidate();
    }

    public void Rotate(float degrees)
    {
        state.Push(new RotateAction { AngleDegrees = degrees });
        Invalidate();
    }

    public void Reset()
    {
        state.Reset();
        drawable.ActiveCropRect = null;
        drawable.ActiveStrokePoints = null;
        CurrentToolMode = ImageEditorToolMode.None;
        ResetViewTransform();
        Invalidate();
    }

    public void ApplyCrop()
    {
        if (drawable.ActiveCropRect is not { } cropRect)
            return;

        // Don't commit if it's essentially the full image
        if (cropRect is { X: < 0.01f, Y: < 0.01f, Width: > 0.98f, Height: > 0.98f })
        {
            CurrentToolMode = ImageEditorToolMode.None;
            return;
        }

        state.Push(new CropAction { CropRect = cropRect });
        drawable.ActiveCropRect = null;
        CurrentToolMode = ImageEditorToolMode.None;
    }

    public async Task<Stream> ExportAsync(ImageExportOptions? options = null)
    {
        options ??= new ImageExportOptions();

        if (drawable.Image == null)
            return Stream.Null;

        var originalWidth = drawable.Image.Width;
        var originalHeight = drawable.Image.Height;

        // Determine output dimensions
        var targetWidth = (int)(options.Width ?? originalWidth);
        var targetHeight = (int)(options.Height ?? originalHeight);

        // Use the GraphicsView's canvas to render to a bitmap
        var exportDrawable = new ImageEditorDrawable
        {
            Image = drawable.Image,
            ViewScale = 1f,
            ViewOffsetX = 0f,
            ViewOffsetY = 0f,
            ToolMode = ImageEditorToolMode.None
        };

        // Copy all committed actions
        foreach (var action in state.Actions)
            exportDrawable.State.Push(action);

        // Render to a bitmap export context
        var stream = new MemoryStream();

#if IOS || MACCATALYST
        using var context = new Microsoft.Maui.Graphics.Platform.PlatformBitmapExportContext(targetWidth, targetHeight, 1f);
        exportDrawable.Draw(context.Canvas, new RectF(0, 0, targetWidth, targetHeight));
        context.WriteToStream(stream);
#elif ANDROID
        using var context = new Microsoft.Maui.Graphics.Platform.PlatformBitmapExportContext(targetWidth, targetHeight, 1f);
        exportDrawable.Draw(context.Canvas, new RectF(0, 0, targetWidth, targetHeight));
        context.WriteToStream(stream);
#else
        // Fallback: render using the platform's default export mechanism
        exportDrawable.Draw(null!, new RectF(0, 0, targetWidth, targetHeight));
#endif

        stream.Position = 0;
        return stream;
    }
}
