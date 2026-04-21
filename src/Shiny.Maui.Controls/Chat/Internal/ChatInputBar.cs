namespace Shiny.Maui.Controls.Chat.Internal;

class ChatInputBar : ContentView
{
    readonly BorderlessEntry entry;
    readonly Button sendButton;
    readonly Button attachButton;
    readonly Grid rootGrid;

    public event Action<string>? SendRequested;
    public event Action? AttachRequested;

    public ChatInputBar()
    {
        entry = new BorderlessEntry
        {
            Placeholder = "Type a message...",
            FontSize = 15,
            ReturnType = ReturnType.Send,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(4, 0)
        };
        entry.Completed += OnEntryCompleted;

        sendButton = new Button
        {
            Text = "Send",
            FontSize = 14,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#007AFF"),
            CornerRadius = 18,
            HeightRequest = 36,
            Padding = new Thickness(16, 0),
            VerticalOptions = LayoutOptions.Center
        };
        sendButton.Clicked += OnSendClicked;

        attachButton = new Button
        {
            Text = "+",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#007AFF"),
            BackgroundColor = Colors.Transparent,
            WidthRequest = 44,
            HeightRequest = 44,
            Padding = 0,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };
        attachButton.Clicked += OnAttachClicked;

        rootGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 4,
            Padding = new Thickness(8, 6),
            BackgroundColor = Color.FromArgb("#F5F5F5")
        };

        // Top border line
        var separator = new BoxView
        {
            HeightRequest = 0.5,
            Color = Color.FromArgb("#E0E0E0"),
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(-8, -6, -8, 0)
        };

        var wrapper = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };

        rootGrid.Add(attachButton, 0, 0);
        rootGrid.Add(entry, 1, 0);
        rootGrid.Add(sendButton, 2, 0);

        wrapper.Add(separator, 0, 0);
        wrapper.Add(rootGrid, 0, 1);

        Content = wrapper;
    }

    public string PlaceholderText
    {
        get => entry.Placeholder ?? string.Empty;
        set => entry.Placeholder = value;
    }

    public string SendButtonText
    {
        get => sendButton.Text ?? string.Empty;
        set => sendButton.Text = value;
    }

    public bool ShowAttachButton
    {
        get => attachButton.IsVisible;
        set => attachButton.IsVisible = value;
    }

    public void ClearText() => entry.Text = string.Empty;

    void OnEntryCompleted(object? sender, EventArgs e)
    {
        TrySend();
    }

    void OnSendClicked(object? sender, EventArgs e)
    {
        TrySend();
    }

    void OnAttachClicked(object? sender, EventArgs e)
    {
        AttachRequested?.Invoke();
    }

    void TrySend()
    {
        var text = entry.Text?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            SendRequested?.Invoke(text);
            ClearText();
        }
    }
}
