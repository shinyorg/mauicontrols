using System.Windows.Input;
using Shiny.Maui.Controls.ImageEditor.EditActions;
using Shiny.Maui.Controls.Infrastructure;

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
            ? ImageEditorToolMode.Move
            : ImageEditorToolMode.Crop);
        DrawCommand = new Command(() => CurrentToolMode = CurrentToolMode == ImageEditorToolMode.Draw
            ? ImageEditorToolMode.Move
            : ImageEditorToolMode.Draw);
        TextCommand = new Command(() => CurrentToolMode = CurrentToolMode == ImageEditorToolMode.Text
            ? ImageEditorToolMode.Move
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
        if (UseFeedback)
            FeedbackHelper.Execute(this, "Undo");
    }

    public void Redo()
    {
        state.Redo();
        Invalidate();
        if (UseFeedback)
            FeedbackHelper.Execute(this, "Redo");
    }

    public void Rotate(float degrees)
    {
        state.Push(new RotateAction { AngleDegrees = degrees });
        Invalidate();
        if (UseFeedback)
            FeedbackHelper.Execute(this, "Rotate");
    }

    public void Reset()
    {
        state.Reset();
        drawable.ActiveCropRect = null;
        drawable.ActiveStrokePoints = null;
        CurrentToolMode = ImageEditorToolMode.Move;
        ResetViewTransform();
        Invalidate();
        if (UseFeedback)
            FeedbackHelper.Execute(this, "Reset");
    }

    public void ApplyCrop()
    {
        if (drawable.ActiveCropRect is not { } cropRect)
            return;

        // Don't commit if it's essentially the full image
        if (cropRect is { X: < 0.01f, Y: < 0.01f, Width: > 0.98f, Height: > 0.98f })
        {
            CurrentToolMode = ImageEditorToolMode.Move;
            return;
        }

        state.Push(new CropAction { CropRect = cropRect });
        drawable.ActiveCropRect = null;
        CurrentToolMode = ImageEditorToolMode.Move;
        if (UseFeedback)
            FeedbackHelper.Execute(this, "CropApplied");
    }

    public EditedImage? GetEditedImage()
    {
        FinalizeCurrentOperation();

        if (drawable.Image == null)
            return null;

        return new EditedImage(drawable.Image, state);
    }

    void ExecuteSave()
    {
        var editedImage = GetEditedImage();
        if (editedImage == null)
            return;

        if (SaveCommand?.CanExecute(editedImage) == true)
        {
            SaveCommand.Execute(editedImage);
            if (UseFeedback)
                FeedbackHelper.Execute(this, "Saved");
        }
    }
}
