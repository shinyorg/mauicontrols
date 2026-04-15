using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;

namespace Shiny.Maui.Controls.Markdown;

public static class MarkdownRenderer
{
    static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static View Render(string markdown, MarkdownTheme theme, Action<string>? linkHandler = null)
    {
        var document = Markdig.Markdown.Parse(markdown, Pipeline);
        var layout = new VerticalStackLayout { Spacing = theme.BlockSpacing };

        foreach (var block in document)
        {
            var view = RenderBlock(block, theme, linkHandler);
            if (view is not null)
                layout.Children.Add(view);
        }

        return layout;
    }

    static View? RenderBlock(Block block, MarkdownTheme theme, Action<string>? linkHandler) => block switch
    {
        HeadingBlock heading => RenderHeading(heading, theme, linkHandler),
        ParagraphBlock paragraph => RenderParagraph(paragraph, theme, linkHandler),
        FencedCodeBlock fenced => RenderCodeBlock(fenced.Lines.ToString(), theme),
        CodeBlock code => RenderCodeBlock(code.Lines.ToString(), theme),
        QuoteBlock quote => RenderBlockquote(quote, theme, linkHandler),
        ListBlock list => RenderList(list, theme, linkHandler),
        ThematicBreakBlock => RenderHorizontalRule(theme),
        Table table => RenderTable(table, theme, linkHandler),
        _ => null
    };

    static View RenderHeading(HeadingBlock heading, MarkdownTheme theme, Action<string>? linkHandler)
    {
        var fontSize = heading.Level switch
        {
            1 => theme.H1FontSize,
            2 => theme.H2FontSize,
            3 => theme.H3FontSize,
            4 => theme.H4FontSize,
            5 => theme.H5FontSize,
            _ => theme.H6FontSize
        };

        var label = new Label
        {
            FontSize = fontSize,
            FontAttributes = FontAttributes.Bold,
            TextColor = theme.TextColor,
            LineBreakMode = LineBreakMode.WordWrap
        };
        ApplyInlines(label, heading.Inline, theme, linkHandler);

        var stack = new VerticalStackLayout { Children = { label } };

        if (heading.Level <= 2)
        {
            stack.Children.Add(new BoxView
            {
                HeightRequest = 1,
                Color = theme.HorizontalRuleColor,
                Margin = new Thickness(0, 4, 0, 0)
            });
        }

        return stack;
    }

    static View RenderParagraph(ParagraphBlock paragraph, MarkdownTheme theme, Action<string>? linkHandler)
    {
        var label = new Label
        {
            FontSize = theme.BaseFontSize,
            TextColor = theme.TextColor,
            LineBreakMode = LineBreakMode.WordWrap
        };
        ApplyInlines(label, paragraph.Inline, theme, linkHandler);
        return label;
    }

    static View RenderCodeBlock(string code, MarkdownTheme theme)
    {
        return new Border
        {
            BackgroundColor = theme.CodeBlockBackgroundColor,
            Padding = new Thickness(16),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = new Label
            {
                Text = code.TrimEnd(),
                FontSize = theme.CodeFontSize,
                FontFamily = theme.CodeFontFamily,
                TextColor = theme.CodeBlockTextColor,
                FontAttributes = FontAttributes.None,
                LineBreakMode = LineBreakMode.WordWrap
            }
        };
    }

    static View RenderBlockquote(QuoteBlock quote, MarkdownTheme theme, Action<string>? linkHandler)
    {
        var inner = new VerticalStackLayout { Spacing = theme.BlockSpacing };
        foreach (var child in quote)
        {
            if (child is Block childBlock)
            {
                var view = RenderBlock(childBlock, theme, linkHandler);
                if (view is not null)
                    inner.Children.Add(view);
            }
        }

        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(4)),
                new ColumnDefinition(GridLength.Star)
            },
            Children =
            {
                new BoxView
                {
                    Color = theme.BlockquoteBorderColor,
                    WidthRequest = 4
                },
                new Border
                {
                    BackgroundColor = theme.BlockquoteBackgroundColor,
                    StrokeThickness = 0,
                    Padding = new Thickness(12, 8),
                    Content = inner
                }.Column(1)
            }
        };
    }

    static View RenderList(ListBlock list, MarkdownTheme theme, Action<string>? linkHandler)
    {
        var stack = new VerticalStackLayout { Spacing = 4 };
        var index = 1;

        foreach (var item in list)
        {
            if (item is not ListItemBlock listItem) continue;

            var bullet = list.IsOrdered ? $"{index++}." : "\u2022";

            var isTaskItem = listItem.Count > 0
                && listItem[0] is ParagraphBlock pb
                && pb.Inline?.FirstChild is TaskList;

            var itemContent = new VerticalStackLayout { Spacing = 2 };
            foreach (var child in listItem)
            {
                if (child is Block childBlock)
                {
                    var view = RenderBlock(childBlock, theme, linkHandler);
                    if (view is not null)
                        itemContent.Children.Add(view);
                }
            }

            if (isTaskItem)
            {
                stack.Children.Add(new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 4,
                    Margin = new Thickness(theme.ListIndent, 0, 0, 0),
                    Children = { itemContent }
                });
            }
            else
            {
                var row = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(new GridLength(theme.ListIndent)),
                        new ColumnDefinition(GridLength.Star)
                    },
                    Children =
                    {
                        new Label
                        {
                            Text = bullet,
                            FontSize = theme.BaseFontSize,
                            TextColor = theme.TextColor,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Start,
                            Margin = new Thickness(0, 0, 8, 0)
                        },
                        itemContent.Column(1)
                    }
                };
                stack.Children.Add(row);
            }
        }

        return stack;
    }

    static View RenderHorizontalRule(MarkdownTheme theme)
    {
        return new BoxView
        {
            HeightRequest = 1,
            Color = theme.HorizontalRuleColor,
            Margin = new Thickness(0, 8)
        };
    }

    static View RenderTable(Table table, MarkdownTheme theme, Action<string>? linkHandler)
    {
        var columnCount = table.ColumnDefinitions?.Count ?? 0;
        if (columnCount == 0) return new BoxView();

        var grid = new Grid
        {
            ColumnSpacing = 0,
            RowSpacing = 0
        };

        for (var c = 0; c < columnCount; c++)
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var rowIndex = 0;
        foreach (var rowObj in table)
        {
            if (rowObj is not TableRow row) continue;

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var isHeader = row.IsHeader;

            for (var c = 0; c < row.Count && c < columnCount; c++)
            {
                if (row[c] is not TableCell cell) continue;

                var label = new Label
                {
                    FontSize = theme.BaseFontSize,
                    TextColor = theme.TextColor,
                    FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
                    Padding = new Thickness(8, 6),
                    LineBreakMode = LineBreakMode.WordWrap,
                    VerticalTextAlignment = TextAlignment.Center
                };

                if (cell.Count > 0 && cell[0] is ParagraphBlock pb)
                    ApplyInlines(label, pb.Inline, theme, linkHandler);

                var border = new Border
                {
                    Stroke = theme.TableBorderColor,
                    StrokeThickness = 0.5,
                    BackgroundColor = isHeader ? theme.TableHeaderBackgroundColor : Colors.Transparent,
                    Content = label
                };
                grid.Add(border, c, rowIndex);
            }
            rowIndex++;
        }

        return new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
            Stroke = theme.TableBorderColor,
            StrokeThickness = 1,
            Content = grid
        };
    }

    static void ApplyInlines(Label label, ContainerInline? inlines, MarkdownTheme theme, Action<string>? linkHandler)
    {
        if (inlines is null)
            return;

        var span = new FormattedString();
        CollectInlines(span, inlines, theme, linkHandler, FontAttributes.None, false);
        label.FormattedText = span;
    }

    static void CollectInlines(
        FormattedString formatted,
        ContainerInline container,
        MarkdownTheme theme,
        Action<string>? linkHandler,
        FontAttributes attrs,
        bool isLink,
        string? linkUrl = null)
    {
        foreach (var inline in container)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    var span = new Span
                    {
                        Text = literal.Content.ToString(),
                        FontSize = theme.BaseFontSize,
                        FontAttributes = attrs,
                        TextColor = isLink ? theme.LinkColor : theme.TextColor
                    };
                    if (isLink && linkUrl is not null)
                    {
                        var gesture = new TapGestureRecognizer();
                        var url = linkUrl;
                        gesture.Tapped += (_, _) => linkHandler?.Invoke(url);
                        span.GestureRecognizers.Add(gesture);
                    }
                    formatted.Spans.Add(span);
                    break;

                case EmphasisInline emphasis:
                    var childAttrs = attrs;
                    if (emphasis.DelimiterChar is '*' or '_')
                    {
                        childAttrs |= emphasis.DelimiterCount == 2
                            ? FontAttributes.Bold
                            : FontAttributes.Italic;
                    }
                    CollectInlines(formatted, emphasis, theme, linkHandler, childAttrs, isLink, linkUrl);
                    break;

                case CodeInline code:
                    formatted.Spans.Add(new Span
                    {
                        Text = code.Content,
                        FontSize = theme.CodeFontSize,
                        FontFamily = theme.CodeFontFamily,
                        TextColor = theme.CodeTextColor,
                        BackgroundColor = theme.CodeBackgroundColor
                    });
                    break;

                case LinkInline link:
                    if (link.IsImage)
                    {
                        // Images are inline - we add a placeholder text
                        formatted.Spans.Add(new Span
                        {
                            Text = $"[Image: {link.FirstChild}]",
                            TextColor = theme.MutedTextColor,
                            FontAttributes = FontAttributes.Italic,
                            FontSize = theme.BaseFontSize
                        });
                    }
                    else
                    {
                        CollectInlines(formatted, link, theme, linkHandler, attrs, true, link.Url);
                    }
                    break;

                case LineBreakInline:
                    formatted.Spans.Add(new Span { Text = "\n" });
                    break;

                case TaskList taskList:
                    formatted.Spans.Add(new Span
                    {
                        Text = taskList.Checked ? "\u2611 " : "\u2610 ",
                        FontSize = theme.BaseFontSize,
                        TextColor = theme.TextColor
                    });
                    break;

                case HtmlInline html:
                    if (html.Tag == "<br>" || html.Tag == "<br/>" || html.Tag == "<br />")
                        formatted.Spans.Add(new Span { Text = "\n" });
                    break;

                case ContainerInline otherContainer:
                    CollectInlines(formatted, otherContainer, theme, linkHandler, attrs, isLink, linkUrl);
                    break;
            }
        }
    }
}

internal static class GridExtensions
{
    public static T Column<T>(this T view, int column) where T : View
    {
        Grid.SetColumn(view, column);
        return view;
    }

    public static T Row<T>(this T view, int row) where T : View
    {
        Grid.SetRow(view, row);
        return view;
    }
}
