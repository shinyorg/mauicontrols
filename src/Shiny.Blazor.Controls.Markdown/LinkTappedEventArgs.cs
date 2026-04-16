namespace Shiny.Blazor.Controls.Markdown;

public class LinkTappedEventArgs : EventArgs
{
    public LinkTappedEventArgs(string url) => Url = url;

    public string Url { get; }
    public bool Handled { get; set; }
}
