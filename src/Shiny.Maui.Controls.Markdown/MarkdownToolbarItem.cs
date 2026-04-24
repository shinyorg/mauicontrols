namespace Shiny.Maui.Controls.Markdown;

public record MarkdownToolbarItem(
    string Label,
    string Icon,
    string Prefix,
    string Suffix,
    bool IsBlockLevel = false);

public static class MarkdownToolbarItems
{
    public static MarkdownToolbarItem Bold { get; } = new("Bold", "\u0042", "**", "**");
    public static MarkdownToolbarItem Italic { get; } = new("Italic", "\u0049", "*", "*");
    public static MarkdownToolbarItem Strikethrough { get; } = new("Strikethrough", "\u0053\u0336", "~~", "~~");
    public static MarkdownToolbarItem InlineCode { get; } = new("Code", "\u2039\u203A", "`", "`");
    public static MarkdownToolbarItem Link { get; } = new("Link", "\u2197", "[", "](url)");
    public static MarkdownToolbarItem Image { get; } = new("Image", "\u25A3", "![", "](url)");
    public static MarkdownToolbarItem H1 { get; } = new("H1", "H\u2081", "# ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem H2 { get; } = new("H2", "H\u2082", "## ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem H3 { get; } = new("H3", "H\u2083", "### ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem BulletList { get; } = new("Bullet List", "\u2022", "- ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem NumberedList { get; } = new("Numbered List", "\u2116", "1. ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem TaskList { get; } = new("Task List", "\u2611", "- [ ] ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem Quote { get; } = new("Quote", "\u275D", "> ", "", IsBlockLevel: true);
    public static MarkdownToolbarItem CodeBlock { get; } = new("Code Block", "\u2263", "```\n", "\n```", IsBlockLevel: true);
    public static MarkdownToolbarItem HorizontalRule { get; } = new("Horizontal Rule", "\u2500", "\n---\n", "", IsBlockLevel: true);

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
