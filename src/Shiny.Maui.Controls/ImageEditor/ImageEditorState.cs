using Shiny.Maui.Controls.ImageEditor.EditActions;

namespace Shiny.Maui.Controls.ImageEditor;

internal sealed class ImageEditorState
{
    readonly List<IEditAction> actions = [];
    readonly Stack<IEditAction> redoStack = new();

    public event Action? StateChanged;

    public IReadOnlyList<IEditAction> Actions => actions;
    public bool CanUndo => actions.Count > 0;
    public bool CanRedo => redoStack.Count > 0;

    public void Push(IEditAction action)
    {
        actions.Add(action);
        redoStack.Clear();
        StateChanged?.Invoke();
    }

    public IEditAction? Undo()
    {
        if (actions.Count == 0)
            return null;

        var action = actions[^1];
        actions.RemoveAt(actions.Count - 1);
        redoStack.Push(action);
        StateChanged?.Invoke();
        return action;
    }

    public IEditAction? Redo()
    {
        if (redoStack.Count == 0)
            return null;

        var action = redoStack.Pop();
        actions.Add(action);
        StateChanged?.Invoke();
        return action;
    }

    public void Reset()
    {
        actions.Clear();
        redoStack.Clear();
        StateChanged?.Invoke();
    }
}
