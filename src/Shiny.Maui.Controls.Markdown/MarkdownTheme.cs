namespace Shiny.Maui.Controls.Markdown;

public class MarkdownTheme
{
    public Color TextColor { get; set; } = Colors.Black;
    public Color MutedTextColor { get; set; } = Color.FromArgb("#6B7280");
    public Color LinkColor { get; set; } = Color.FromArgb("#2563EB");
    public Color CodeBackgroundColor { get; set; } = Color.FromArgb("#F3F4F6");
    public Color CodeTextColor { get; set; } = Color.FromArgb("#D946EF");
    public Color CodeBlockBackgroundColor { get; set; } = Color.FromArgb("#1F2937");
    public Color CodeBlockTextColor { get; set; } = Color.FromArgb("#E5E7EB");
    public Color BlockquoteBorderColor { get; set; } = Color.FromArgb("#D1D5DB");
    public Color BlockquoteBackgroundColor { get; set; } = Color.FromArgb("#F9FAFB");
    public Color HorizontalRuleColor { get; set; } = Color.FromArgb("#E5E7EB");
    public Color TableBorderColor { get; set; } = Color.FromArgb("#E5E7EB");
    public Color TableHeaderBackgroundColor { get; set; } = Color.FromArgb("#F3F4F6");

    public double BaseFontSize { get; set; } = 16;
    public double H1FontSize { get; set; } = 32;
    public double H2FontSize { get; set; } = 24;
    public double H3FontSize { get; set; } = 20;
    public double H4FontSize { get; set; } = 18;
    public double H5FontSize { get; set; } = 16;
    public double H6FontSize { get; set; } = 14;
    public double CodeFontSize { get; set; } = 14;
    public double BlockSpacing { get; set; } = 12;
    public double ListIndent { get; set; } = 24;
    public string CodeFontFamily { get; set; } = "";

    public static MarkdownTheme Light => new();

    public static MarkdownTheme Dark => new()
    {
        TextColor = Color.FromArgb("#E5E7EB"),
        MutedTextColor = Color.FromArgb("#9CA3AF"),
        LinkColor = Color.FromArgb("#60A5FA"),
        CodeBackgroundColor = Color.FromArgb("#374151"),
        CodeTextColor = Color.FromArgb("#F472B6"),
        CodeBlockBackgroundColor = Color.FromArgb("#111827"),
        CodeBlockTextColor = Color.FromArgb("#D1D5DB"),
        BlockquoteBorderColor = Color.FromArgb("#4B5563"),
        BlockquoteBackgroundColor = Color.FromArgb("#1F2937"),
        HorizontalRuleColor = Color.FromArgb("#374151"),
        TableBorderColor = Color.FromArgb("#374151"),
        TableHeaderBackgroundColor = Color.FromArgb("#1F2937")
    };
}
