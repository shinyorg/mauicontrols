using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.Markdown;

public partial class MarkdownViewPage : ContentPage
{
    public MarkdownViewPage()
    {
        InitializeComponent();
    }
}

public partial class MarkdownViewViewModel : ObservableObject
{
    [ObservableProperty]
    string markdown = """
        # Markdown Viewer Demo

        This page demonstrates the **MarkdownView** control rendering markdown as native MAUI controls.

        ## Text Formatting

        You can use **bold**, *italic*, ~~strikethrough~~, and `inline code` in your text.

        ## Links

        Visit [Shiny on GitHub](https://github.com/shinyorg) for more info.

        ## Lists

        ### Unordered
        - First item
        - Second item
        - Third item

        ### Ordered
        1. Step one
        2. Step two
        3. Step three

        ### Task List
        - [x] Create MarkdownView
        - [x] Create MarkdownEditor
        - [ ] World domination

        ## Blockquote

        > "The best way to predict the future is to invent it."
        > — Alan Kay

        ## Code Block

        ```csharp
        var view = new MarkdownView
        {
            Markdown = "# Hello World",
            Theme = MarkdownTheme.Dark
        };
        ```

        ## Table

        | Feature | Status | Priority |
        |---------|--------|----------|
        | Headings | Done | High |
        | Lists | Done | High |
        | Tables | Done | Medium |
        | Images | Planned | Low |

        ---

        *End of demo*
        """;
}
