namespace Shiny.Maui.Controls.Markdown;

public record MarkdownToolbarItem(
    string Label,
    string Icon,
    string Prefix,
    string Suffix,
    bool IsBlockLevel = false);

public static class MarkdownToolbarItems
{
    public static MarkdownToolbarItem Bold { get; } = new("Bold", "\uD83D\uDDD4", "**", "**");
    public static MarkdownToolbarItem Italic { get; } = new("Italic", "\uD835\uDC3C", "*", "*");
    public static MarkdownToolbarItem Strikethrough { get; } = new("Strikethrough", "S\u0336", "~~", "~~");
    public static MarkdownToolbarItem InlineCode { get; } = new("Code", "</>", "`", "`");
    public static MarkdownToolbarItem Link { get; } = new("Link", "\uD83D\uDD17", "[", "](url)");
    public static MarkdownToolbarItem Image { get; } = new("Image", "\uD83D\uDDBC", "![", "](url)");
    public static MarkdownToolbarItem H1 { get; } = new("H1", "H1", "# ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem H2 { get; } = new("H2", "H2", "## ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem H3 { get; } = new("H3", "H3", "### ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem BulletList { get; } = new("Bullet List", "\u2022", "- ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem NumberedList { get; } = new("Numbered List", "1.", "1. ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem TaskList { get; } = new("Task List", "\u2611", "- [ ] ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem Quote { get; } = new("Quote", "\u201C", "> ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem CodeBlock { get; } = new("Code Block", "{}", "```\n", "\n```", IsBlockLevel: true);
    public static MarkdownToolbarItem HorizontalRule { get; } = new("Horizontal Rule", "\u2015", "\n---\n", "", IsBlockLevel: true);

    public static IReadOnlyList<MarkdownToolbarItem> All { get; } =
    [
        Bold, Italic, Strikethrough, InlineCode,
        H1, H2, H3,
        BulletList, NumberedList, TaskList,
        Link, Image, Quote, CodeBlock, HorizontalRule
    ];

    public static IReadOnlyList<MarkdownToolbarItem> Default { get; } =
    [
        Bold, Italic, InlineCode,
        H1, H2, H3,
        BulletList, NumberedList, TaskList,
        Link, Quote, CodeBlock
    ];
}
