using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.Markdown;

public partial class MarkdownEditorPage : ContentPage
{
    public MarkdownEditorPage()
    {
        InitializeComponent();
    }
}

public partial class MarkdownEditorViewModel : ObservableObject
{
    [ObservableProperty]
    string markdown = """
        # Welcome to the Editor

        Try using the **toolbar** above to format text, or toggle the preview with the eye button.

        - Bold, italic, and code formatting
        - Headings (H1, H2, H3)
        - Lists and task lists
        - Links, quotes, and code blocks
        """;
}
