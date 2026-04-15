namespace Shiny.Maui.Controls.MermaidDiagrams.Theming;

public sealed class DarkTheme : DiagramTheme
{
    public override Color BackgroundColor => Color.FromArgb("#1a1a2e");

    public override Color TextColor => Color.FromArgb("#eaeaea");

    public override float FontSize => 14f;

    public override NodeRenderStyle DefaultNodeStyle => new()
    {
        FillColor = Color.FromArgb("#0f3460"),
        StrokeColor = Color.FromArgb("#e94560"),
        TextColor = Color.FromArgb("#eaeaea"),
        StrokeWidth = 2,
        CornerRadius = 4
    };

    public override EdgeRenderStyle DefaultEdgeStyle => new()
    {
        StrokeColor = Color.FromArgb("#e94560"),
        StrokeWidth = 2,
        LabelBackgroundColor = Color.FromArgb("#16213e"),
        LabelTextColor = Color.FromArgb("#eaeaea")
    };

    public override SubgraphRenderStyle DefaultSubgraphStyle => new()
    {
        FillColor = Color.FromArgb("#16213e"),
        StrokeColor = Color.FromArgb("#e94560"),
        TitleColor = Color.FromArgb("#e94560"),
        StrokeWidth = 1.5f,
        CornerRadius = 8
    };
}