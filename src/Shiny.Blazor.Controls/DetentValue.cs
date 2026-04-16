namespace Shiny.Blazor.Controls;

/// <summary>
/// Represents a snap point for the bottom sheet as a ratio of the available height (0.0 to 1.0).
/// </summary>
public readonly record struct DetentValue(double Ratio)
{
    public static DetentValue Quarter => new(0.25);
    public static DetentValue Half => new(0.50);
    public static DetentValue ThreeQuarters => new(0.75);
    public static DetentValue Full => new(1.0);
}
