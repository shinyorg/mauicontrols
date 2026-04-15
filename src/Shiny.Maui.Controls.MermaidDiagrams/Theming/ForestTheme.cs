namespace Shiny.Maui.Controls.MermaidDiagrams.Theming;

public sealed class ForestTheme : DiagramTheme
{
    public override Color BackgroundColor => Color.FromArgb("#ffffff");

    public override Color TextColor => Color.FromArgb("#1b4332");

    public override float FontSize => 14f;

    public override NodeRenderStyle DefaultNodeStyle => new()
    {
        FillColor = Color.FromArgb("#2d6a4f"),
        StrokeColor = Color.FromArgb("#1b4332"),
        TextColor = Color.FromArgb("#ffffff"),
        StrokeWidth = 2,
        CornerRadius = 4
    };

    public override EdgeRenderStyle DefaultEdgeStyle => new()
    {
        StrokeColor = Color.FromArgb("#40916c"),
        StrokeWidth = 2,
        LabelBackgroundColor = Color.FromArgb("#d8f3dc"),
        LabelTextColor = Color.FromArgb("#1b4332")
    };

    public override SubgraphRenderStyle DefaultSubgraphStyle => new()
    {
        FillColor = Color.FromArgb("#d8f3dc"),
        StrokeColor = Color.FromArgb("#2d6a4f"),
        TitleColor = Color.FromArgb("#2d6a4f"),
        StrokeWidth = 1.5f,
        CornerRadius = 8
    };
}