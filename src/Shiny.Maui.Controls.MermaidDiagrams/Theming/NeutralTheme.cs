namespace Shiny.Maui.Controls.MermaidDiagrams.Theming;

public sealed class NeutralTheme : DiagramTheme
{
    public override Color BackgroundColor => Color.FromArgb("#f5f5f5");

    public override Color TextColor => Color.FromArgb("#212121");

    public override float FontSize => 14f;

    public override NodeRenderStyle DefaultNodeStyle => new()
    {
        FillColor = Color.FromArgb("#e0e0e0"),
        StrokeColor = Color.FromArgb("#757575"),
        TextColor = Color.FromArgb("#212121"),
        StrokeWidth = 2,
        CornerRadius = 4
    };

    public override EdgeRenderStyle DefaultEdgeStyle => new()
    {
        StrokeColor = Color.FromArgb("#757575"),
        StrokeWidth = 2,
        LabelBackgroundColor = Color.FromArgb("#f5f5f5"),
        LabelTextColor = Color.FromArgb("#212121")
    };

    public override SubgraphRenderStyle DefaultSubgraphStyle => new()
    {
        FillColor = Color.FromArgb("#eeeeee"),
        StrokeColor = Color.FromArgb("#9e9e9e"),
        TitleColor = Color.FromArgb("#424242"),
        StrokeWidth = 1.5f,
        CornerRadius = 8
    };
}