using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.Controls.Cells;

public class ButtonCell : CellBase
{
    Label buttonLabel = default!;

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(ButtonCell), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(ButtonCell), null);

    public static readonly BindableProperty ButtonTextColorProperty = BindableProperty.Create(
        nameof(ButtonTextColor), typeof(Color), typeof(ButtonCell), null,
        propertyChanged: (b, o, n) => ((ButtonCell)b).UpdateButtonColor());

    public static readonly BindableProperty TitleAlignmentProperty = BindableProperty.Create(
        nameof(TitleAlignment), typeof(TextAlignment), typeof(ButtonCell), TextAlignment.Center,
        propertyChanged: (b, o, n) => ((ButtonCell)b).UpdateTitleAlignment());

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public Color? ButtonTextColor
    {
        get => (Color?)GetValue(ButtonTextColorProperty);
        set => SetValue(ButtonTextColorProperty, value);
    }

    public TextAlignment TitleAlignment
    {
        get => (TextAlignment)GetValue(TitleAlignmentProperty);
        set => SetValue(TitleAlignmentProperty, value);
    }

    public ButtonCell()
    {
        BuildButtonLayout();
    }

    void BuildButtonLayout()
    {
        buttonLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            Padding = new Thickness(16, 12),
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Fill
        };
        buttonLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        Content = buttonLabel;
    }

    void UpdateButtonColor()
    {
        buttonLabel.TextColor = ButtonTextColor ?? ParentTableView?.CellAccentColor ?? Colors.Blue;
    }

    void UpdateTitleAlignment()
    {
        buttonLabel.HorizontalTextAlignment = TitleAlignment;
    }

    protected override void OnTapped()
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }
}