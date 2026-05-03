namespace Shiny.Maui.Controls.Toast;

public static class ToastStyles
{
    public const string InfoStyleKey = "ShinyToastInfoStyle";
    public const string SuccessStyleKey = "ShinyToastSuccessStyle";
    public const string WarningStyleKey = "ShinyToastWarningStyle";
    public const string DangerStyleKey = "ShinyToastDangerStyle";
    public const string CriticalStyleKey = "ShinyToastCriticalStyle";

    internal static readonly Dictionary<ToastType, string> StyleKeys = new()
    {
        [ToastType.Info] = InfoStyleKey,
        [ToastType.Success] = SuccessStyleKey,
        [ToastType.Warning] = WarningStyleKey,
        [ToastType.Danger] = DangerStyleKey,
        [ToastType.Critical] = CriticalStyleKey,
    };

    // Default colors: (Background, Text, Border)
    // Bold, high-contrast colors suitable for toast notifications
    internal static readonly Dictionary<ToastType, (string Bg, string Text, string Border)> DefaultColors = new()
    {
        [ToastType.Info] = ("#1E40AF", "#FFFFFF", "#3B82F6"),       // Deep blue bg, white text
        [ToastType.Success] = ("#166534", "#FFFFFF", "#22C55E"),    // Deep green bg, white text
        [ToastType.Warning] = ("#854D0E", "#FFFFFF", "#F59E0B"),    // Deep amber bg, white text
        [ToastType.Danger] = ("#9A3412", "#FFFFFF", "#F97316"),     // Deep orange bg, white text
        [ToastType.Critical] = ("#991B1B", "#FFFFFF", "#EF4444"),   // Deep red bg, white text
    };

    /// <summary>
    /// Attempts to resolve a ToastConfig style from the application's resource dictionary.
    /// Style setters should target ToastConfig properties: BackgroundColor, TextColor, BorderColor, BorderThickness, CornerRadius.
    /// </summary>
    internal static bool TryApplyStyle(ToastType type, ToastConfig config)
    {
        if (!StyleKeys.TryGetValue(type, out var key))
            return false;

        if (Application.Current?.Resources.TryGetValue(key, out var value) != true)
            return false;

        if (value is not ToastTypeStyle style)
            return false;

        if (style.BackgroundColor is not null)
            config.BackgroundColor = style.BackgroundColor;
        if (style.TextColor is not null)
            config.TextColor = style.TextColor;
        if (style.BorderColor is not null)
            config.BorderColor = style.BorderColor;
        if (style.BorderThickness.HasValue)
            config.BorderThickness = style.BorderThickness.Value;
        if (style.CornerRadius.HasValue)
            config.CornerRadius = style.CornerRadius.Value;

        return config.BackgroundColor is not null;
    }

    internal static void ApplyDefaults(ToastType type, ToastConfig config)
    {
        var (bgHex, textHex, borderHex) = DefaultColors[type];
        config.BackgroundColor ??= Color.FromArgb(bgHex);
        config.TextColor ??= Color.FromArgb(textHex);
        config.BorderColor ??= Color.FromArgb(borderHex);
        if (config.BorderThickness == 0)
            config.BorderThickness = 1;
    }
}

/// <summary>
/// Define in App.xaml resources to override toast type colors.
/// </summary>
/// <example>
/// <![CDATA[
/// <ResourceDictionary>
///     <shiny:ToastTypeStyle x:Key="ShinyToastSuccessStyle"
///                           BackgroundColor="#065F46"
///                           TextColor="White"
///                           BorderColor="#10B981" />
/// </ResourceDictionary>
/// ]]>
/// </example>
public class ToastTypeStyle
{
    public Color? BackgroundColor { get; set; }
    public Color? TextColor { get; set; }
    public Color? BorderColor { get; set; }
    public double? BorderThickness { get; set; }
    public double? CornerRadius { get; set; }
}
