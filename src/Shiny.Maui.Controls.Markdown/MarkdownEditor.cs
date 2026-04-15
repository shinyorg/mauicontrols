namespace Shiny.Maui.Controls.Markdown;

public class MarkdownEditor : ContentView
{
    readonly Grid rootGrid;
    readonly Editor editor;
    readonly ScrollView toolbarScroll;
    readonly HorizontalStackLayout toolbar;
    readonly MarkdownView previewView;
    readonly Grid editorContainer;
    readonly Button toggleButton;
    bool isPreviewMode;
    bool isUpdating;

    public MarkdownEditor()
    {
        editor = new Editor
        {
            AutoSize = EditorAutoSizeOption.TextChanges,
            Placeholder = "Write markdown here...",
            FontFamily = "",
            FontSize = 15
        };
        editor.TextChanged += OnEditorTextChanged;

        toolbar = new HorizontalStackLayout { Spacing = 2 };
        toolbarScroll = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            Content = toolbar,
            HeightRequest = 44
        };

        toggleButton = new Button
        {
            Text = "\uD83D\uDC41",
            WidthRequest = 44,
            HeightRequest = 44,
            Padding = 0,
            CornerRadius = 8,
            BackgroundColor = Colors.Transparent,
            FontSize = 20
        };
        toggleButton.Clicked += OnTogglePreview;

        var toolbarRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(44))
            },
            Children = { toolbarScroll }
        };
        Grid.SetColumn(toggleButton, 1);
        toolbarRow.Children.Add(toggleButton);

        previewView = new MarkdownView
        {
            IsVisible = false
        };

        editorContainer = new Grid
        {
            Children = { new ScrollView { Content = editor } }
        };

        rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            },
            Children = { toolbarRow }
        };
        Grid.SetRow(editorContainer, 1);
        Grid.SetRow(previewView, 1);
        rootGrid.Children.Add(editorContainer);
        rootGrid.Children.Add(previewView);

        Content = rootGrid;
        BuildToolbar(MarkdownToolbarItems.Default);
    }

    public static readonly BindableProperty MarkdownProperty = BindableProperty.Create(
        nameof(Markdown),
        typeof(string),
        typeof(MarkdownEditor),
        string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) =>
        {
            var me = (MarkdownEditor)b;
            if (me.isUpdating) return;

            me.isUpdating = true;
            me.editor.Text = (string)n;
            me.isUpdating = false;

            if (me.isPreviewMode)
                me.previewView.Markdown = (string)n;
        });

    public string Markdown
    {
        get => (string)GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
        nameof(Theme),
        typeof(MarkdownTheme),
        typeof(MarkdownEditor),
        null,
        propertyChanged: (b, _, n) => ((MarkdownEditor)b).previewView.Theme = (MarkdownTheme?)n);

    public MarkdownTheme? Theme
    {
        get => (MarkdownTheme?)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(MarkdownEditor),
        "Write markdown here...",
        propertyChanged: (b, _, n) => ((MarkdownEditor)b).editor.Placeholder = (string)n);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty ToolbarItemsProperty = BindableProperty.Create(
        nameof(ToolbarItems),
        typeof(IReadOnlyList<MarkdownToolbarItem>),
        typeof(MarkdownEditor),
        null,
        propertyChanged: (b, _, n) =>
        {
            var me = (MarkdownEditor)b;
            me.BuildToolbar(n as IReadOnlyList<MarkdownToolbarItem> ?? MarkdownToolbarItems.Default);
        });

    public IReadOnlyList<MarkdownToolbarItem>? ToolbarItems
    {
        get => (IReadOnlyList<MarkdownToolbarItem>?)GetValue(ToolbarItemsProperty);
        set => SetValue(ToolbarItemsProperty, value);
    }

    public static readonly BindableProperty IsPreviewVisibleProperty = BindableProperty.Create(
        nameof(IsPreviewVisible),
        typeof(bool),
        typeof(MarkdownEditor),
        false,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((MarkdownEditor)b).SetPreviewMode((bool)n));

    public bool IsPreviewVisible
    {
        get => (bool)GetValue(IsPreviewVisibleProperty);
        set => SetValue(IsPreviewVisibleProperty, value);
    }

    public static readonly BindableProperty ToolbarBackgroundColorProperty = BindableProperty.Create(
        nameof(ToolbarBackgroundColor),
        typeof(Color),
        typeof(MarkdownEditor),
        null,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((MarkdownEditor)b).toolbarScroll.BackgroundColor = c;
        });

    public Color? ToolbarBackgroundColor
    {
        get => (Color?)GetValue(ToolbarBackgroundColorProperty);
        set => SetValue(ToolbarBackgroundColorProperty, value);
    }

    public static readonly BindableProperty EditorBackgroundColorProperty = BindableProperty.Create(
        nameof(EditorBackgroundColor),
        typeof(Color),
        typeof(MarkdownEditor),
        null,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((MarkdownEditor)b).editor.BackgroundColor = c;
        });

    public Color? EditorBackgroundColor
    {
        get => (Color?)GetValue(EditorBackgroundColorProperty);
        set => SetValue(EditorBackgroundColorProperty, value);
    }

    public event EventHandler<LinkTappedEventArgs>? LinkTapped
    {
        add => previewView.LinkTapped += value;
        remove => previewView.LinkTapped -= value;
    }

    public event EventHandler<TextChangedEventArgs>? TextChanged;

    public void InsertFormatting(MarkdownToolbarItem item)
    {
        var text = editor.Text ?? string.Empty;
        var cursorPos = editor.CursorPosition;
        var selLength = editor.SelectionLength;

        if (item.IsBlockLevel)
        {
            var selectedText = selLength > 0 ? text.Substring(cursorPos, selLength) : "";
            var newText = item.Prefix + selectedText + item.Suffix;

            isUpdating = true;
            editor.Text = text.Remove(cursorPos, selLength).Insert(cursorPos, newText);
            editor.CursorPosition = cursorPos + item.Prefix.Length + selectedText.Length;
            isUpdating = false;

            Markdown = editor.Text;
        }
        else
        {
            if (selLength > 0)
            {
                var selected = text.Substring(cursorPos, selLength);
                var wrapped = item.Prefix + selected + item.Suffix;

                isUpdating = true;
                editor.Text = text.Remove(cursorPos, selLength).Insert(cursorPos, wrapped);
                editor.CursorPosition = cursorPos + wrapped.Length;
                isUpdating = false;

                Markdown = editor.Text;
            }
            else
            {
                var inserted = item.Prefix + item.Suffix;
                isUpdating = true;
                editor.Text = text.Insert(cursorPos, inserted);
                editor.CursorPosition = cursorPos + item.Prefix.Length;
                isUpdating = false;

                Markdown = editor.Text;
            }
        }
    }

    void BuildToolbar(IReadOnlyList<MarkdownToolbarItem> items)
    {
        toolbar.Children.Clear();

        foreach (var item in items)
        {
            var button = new Button
            {
                Text = item.Icon,
                WidthRequest = 40,
                HeightRequest = 40,
                Padding = 0,
                CornerRadius = 6,
                BackgroundColor = Colors.Transparent,
                FontSize = 15
            };

            var captured = item;
            button.Clicked += (_, _) => InsertFormatting(captured);
            toolbar.Children.Add(button);
        }
    }

    void OnEditorTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (isUpdating) return;

        isUpdating = true;
        Markdown = editor.Text ?? string.Empty;
        isUpdating = false;

        TextChanged?.Invoke(this, e);
    }

    void OnTogglePreview(object? sender, EventArgs e)
    {
        IsPreviewVisible = !isPreviewMode;
    }

    void SetPreviewMode(bool preview)
    {
        isPreviewMode = preview;

        if (preview)
        {
            previewView.Markdown = Markdown;
            previewView.IsVisible = true;
            editorContainer.IsVisible = false;
            toggleButton.Text = "\u270F\uFE0F";
        }
        else
        {
            previewView.IsVisible = false;
            editorContainer.IsVisible = true;
            toggleButton.Text = "\uD83D\uDC41";
        }
    }
}
