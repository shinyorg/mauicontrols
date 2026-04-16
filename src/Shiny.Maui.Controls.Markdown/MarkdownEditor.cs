namespace Shiny.Maui.Controls.Markdown;

public class MarkdownEditor : ContentView
{
    readonly Grid rootGrid;
    readonly Grid toolbarRow;
    readonly Editor editor;
    readonly ScrollView toolbarScroll;
    readonly HorizontalStackLayout toolbar;
    readonly MarkdownView previewView;
    readonly Grid editorContainer;
    readonly Button toggleButton;
    readonly BoxView toolbarSeparator;
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

        toolbar = new HorizontalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };
        toolbarScroll = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            Content = toolbar,
            Padding = new Thickness(8, 0, 0, 0)
        };

        toggleButton = new Button
        {
            Text = "\uD83D\uDC41\uFE0F",
            WidthRequest = 40,
            HeightRequest = 40,
            Padding = 0,
            CornerRadius = 8,
            BackgroundColor = Colors.Transparent,
            FontSize = 18,
            VerticalOptions = LayoutOptions.Center
        };
        toggleButton.Clicked += OnTogglePreview;

        toolbarRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(48))
            },
            Padding = new Thickness(4, 6),
            ColumnSpacing = 0,
            HeightRequest = 52,
            Children = { toolbarScroll }
        };
        Grid.SetColumn(toggleButton, 1);
        toolbarRow.Children.Add(toggleButton);

        toolbarSeparator = new BoxView
        {
            HeightRequest = 1,
            HorizontalOptions = LayoutOptions.Fill
        };

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
                new RowDefinition(new GridLength(1)),
                new RowDefinition(GridLength.Star)
            },
            Children = { toolbarRow }
        };
        Grid.SetRow(toolbarSeparator, 1);
        Grid.SetRow(editorContainer, 2);
        Grid.SetRow(previewView, 2);
        rootGrid.Children.Add(toolbarSeparator);
        rootGrid.Children.Add(editorContainer);
        rootGrid.Children.Add(previewView);

        Content = rootGrid;
        ApplyThemeColors();
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
                ((MarkdownEditor)b).toolbarRow.BackgroundColor = c;
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

    void ApplyThemeColors()
    {
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        toolbarRow.BackgroundColor = isDark
            ? Color.FromArgb("#1F2937")
            : Color.FromArgb("#F9FAFB");

        toolbarSeparator.Color = isDark
            ? Color.FromArgb("#374151")
            : Color.FromArgb("#E5E7EB");

        toggleButton.TextColor = isDark
            ? Color.FromArgb("#D1D5DB")
            : Color.FromArgb("#6B7280");
    }

    void BuildToolbar(IReadOnlyList<MarkdownToolbarItem> items)
    {
        toolbar.Children.Clear();

        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var buttonBg = isDark ? Color.FromArgb("#374151") : Color.FromArgb("#E5E7EB");
        var buttonText = isDark ? Color.FromArgb("#E5E7EB") : Color.FromArgb("#374151");
        var separatorColor = isDark ? Color.FromArgb("#4B5563") : Color.FromArgb("#D1D5DB");

        string? lastGroup = null;

        foreach (var item in items)
        {
            var group = GetToolbarGroup(item);
            if (lastGroup != null && group != lastGroup)
            {
                toolbar.Children.Add(new BoxView
                {
                    WidthRequest = 1,
                    HeightRequest = 24,
                    Color = separatorColor,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(2, 0)
                });
            }
            lastGroup = group;

            var button = new Button
            {
                Text = item.Icon,
                WidthRequest = 38,
                HeightRequest = 38,
                MinimumWidthRequest = 38,
                Padding = 0,
                CornerRadius = 8,
                BorderWidth = 0,
                FontSize = 14,
                BackgroundColor = buttonBg,
                TextColor = buttonText
            };

            if (ReferenceEquals(item, MarkdownToolbarItems.Bold))
                button.FontAttributes = FontAttributes.Bold;
            else if (ReferenceEquals(item, MarkdownToolbarItems.Italic))
                button.FontAttributes = FontAttributes.Italic;

            var captured = item;
            button.Clicked += (_, _) => InsertFormatting(captured);
            toolbar.Children.Add(button);
        }
    }

    static string GetToolbarGroup(MarkdownToolbarItem item)
    {
        if (ReferenceEquals(item, MarkdownToolbarItems.Bold) ||
            ReferenceEquals(item, MarkdownToolbarItems.Italic) ||
            ReferenceEquals(item, MarkdownToolbarItems.Strikethrough) ||
            ReferenceEquals(item, MarkdownToolbarItems.InlineCode))
            return "format";

        if (ReferenceEquals(item, MarkdownToolbarItems.H1) ||
            ReferenceEquals(item, MarkdownToolbarItems.H2) ||
            ReferenceEquals(item, MarkdownToolbarItems.H3))
            return "heading";

        if (ReferenceEquals(item, MarkdownToolbarItems.BulletList) ||
            ReferenceEquals(item, MarkdownToolbarItems.NumberedList) ||
            ReferenceEquals(item, MarkdownToolbarItems.TaskList))
            return "list";

        if (ReferenceEquals(item, MarkdownToolbarItems.Link) ||
            ReferenceEquals(item, MarkdownToolbarItems.Image) ||
            ReferenceEquals(item, MarkdownToolbarItems.Quote) ||
            ReferenceEquals(item, MarkdownToolbarItems.CodeBlock) ||
            ReferenceEquals(item, MarkdownToolbarItems.HorizontalRule))
            return "insert";

        return "other";
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
            toggleButton.Text = "\uD83D\uDC41\uFE0F";
        }
    }
}
