namespace Shiny.Maui.Controls.Markdown;

public class MarkdownView : ContentView
{
    ScrollView? scrollView;

    public MarkdownView()
    {
        scrollView = new ScrollView();
        base.Content = scrollView;
    }

    public static readonly BindableProperty MarkdownProperty = BindableProperty.Create(
        nameof(Markdown),
        typeof(string),
        typeof(MarkdownView),
        string.Empty,
        propertyChanged: (b, _, _) => ((MarkdownView)b).Rebuild());

    public string Markdown
    {
        get => (string)GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
        nameof(Theme),
        typeof(MarkdownTheme),
        typeof(MarkdownView),
        null,
        propertyChanged: (b, _, _) => ((MarkdownView)b).Rebuild());

    public MarkdownTheme? Theme
    {
        get => (MarkdownTheme?)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly BindableProperty IsScrollEnabledProperty = BindableProperty.Create(
        nameof(IsScrollEnabled),
        typeof(bool),
        typeof(MarkdownView),
        true,
        propertyChanged: (b, _, n) =>
        {
            var view = (MarkdownView)b;
            if (view.scrollView is not null)
                view.scrollView.IsEnabled = (bool)n;
        });

    public bool IsScrollEnabled
    {
        get => (bool)GetValue(IsScrollEnabledProperty);
        set => SetValue(IsScrollEnabledProperty, value);
    }

    public event EventHandler<LinkTappedEventArgs>? LinkTapped;

    MarkdownTheme ResolveTheme() => Theme ?? (Application.Current?.RequestedTheme == AppTheme.Dark
        ? MarkdownTheme.Dark
        : MarkdownTheme.Light);

    void Rebuild()
    {
        if (string.IsNullOrWhiteSpace(Markdown))
        {
            if (scrollView is not null)
                scrollView.Content = null;
            return;
        }

        var theme = ResolveTheme();
        var rendered = MarkdownRenderer.Render(Markdown, theme, OnLinkTapped);

        if (scrollView is not null)
            scrollView.Content = rendered;
    }

    void OnLinkTapped(string url)
    {
        var args = new LinkTappedEventArgs(url);
        LinkTapped?.Invoke(this, args);

        if (!args.Handled)
            _ = Launcher.Default.OpenAsync(url);
    }
}

public class LinkTappedEventArgs : EventArgs
{
    public LinkTappedEventArgs(string url) => Url = url;
    public string Url { get; }
    public bool Handled { get; set; }
}
