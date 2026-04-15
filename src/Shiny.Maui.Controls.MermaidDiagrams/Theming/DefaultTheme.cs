namespace Shiny.Maui.Controls.MermaidDiagrams.Theming;

public sealed class DefaultTheme : DiagramTheme
{
    public override Color BackgroundColor => Color.FromArgb("#ffffff");

    public override Color TextColor => Color.FromArgb("#333333");

    public override float FontSize => 14f;

    public override NodeRenderStyle DefaultNodeStyle => new()
    {
        FillColor = Color.FromArgb("#1f77b4"),
        StrokeColor = Color.FromArgb("#135a8a"),
        TextColor = Color.FromArgb("#ffffff"),
        StrokeWidth = 2,
        CornerRadius = 4
    };

    public override EdgeRenderStyle DefaultEdgeStyle => new()
    {
        StrokeColor = Color.FromArgb("#555555"),
        StrokeWidth = 2,
        LabelBackgroundColor = Color.FromArgb("#ffffff"),
        LabelTextColor = Color.FromArgb("#333333")
    };

    public override SubgraphRenderStyle DefaultSubgraphStyle => new()
    {
        FillColor = Color.FromArgb("#e8f4fd"),
        StrokeColor = Color.FromArgb("#1f77b4"),
        TitleColor = Color.FromArgb("#1f77b4"),
        StrokeWidth = 1.5f,
        CornerRadius = 8
    };
}