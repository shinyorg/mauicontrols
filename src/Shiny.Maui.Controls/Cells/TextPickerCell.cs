using System.Collections;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.Controls.Cells;

public class TextPickerCell : CellBase
{
    Label valueLabel = default!;
    Picker hiddenPicker = default!;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(TextPickerCell), null,
        propertyChanged: (b, o, n) => ((TextPickerCell)b).UpdatePickerItems());

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex), typeof(int), typeof(TextPickerCell), -1,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((TextPickerCell)b).OnSelectedIndexChanged());

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(TextPickerCell), null,
        BindingMode.TwoWay);

    public static readonly BindableProperty DisplayMemberProperty = BindableProperty.Create(
        nameof(DisplayMember), typeof(string), typeof(TextPickerCell), null);

    public static readonly BindableProperty PickerTitleProperty = BindableProperty.Create(
        nameof(PickerTitle), typeof(string), typeof(TextPickerCell), null,
        propertyChanged: (b, o, n) =>
        {
            var cell = (TextPickerCell)b;
            if (cell.hiddenPicker != null)
                cell.hiddenPicker.Title = (string?)n;
        });

    public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create(
        nameof(SelectedCommand), typeof(ICommand), typeof(TextPickerCell), null);

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(TextPickerCell), null,
        propertyChanged: (b, o, n) => ((TextPickerCell)b).UpdateValueColor());

    public IList? ItemsSource
    {
        get => (IList?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string? DisplayMember
    {
        get => (string?)GetValue(DisplayMemberProperty);
        set => SetValue(DisplayMemberProperty, value);
    }

    public string? PickerTitle
    {
        get => (string?)GetValue(PickerTitleProperty);
        set => SetValue(PickerTitleProperty, value);
    }

    public ICommand? SelectedCommand
    {
        get => (ICommand?)GetValue(SelectedCommandProperty);
        set => SetValue(SelectedCommandProperty, value);
    }

    public Color? ValueTextColor
    {
        get => (Color?)GetValue(ValueTextColorProperty);
        set => SetValue(ValueTextColorProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        valueLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        hiddenPicker = new Picker
        {
            Opacity = 0.01,
            Title = PickerTitle
        };
        hiddenPicker.SelectedIndexChanged += (s, e) =>
        {
            SelectedIndex = hiddenPicker.SelectedIndex;
            SelectedItem = hiddenPicker.SelectedItem;
            UpdateDisplayText();

            if (SelectedCommand?.CanExecute(SelectedItem) == true)
                SelectedCommand.Execute(SelectedItem);
        };
        hiddenPicker.Focused += (s, e) => ApplySelectionHighlight();
        hiddenPicker.Unfocused += (s, e) => ClearSelectionHighlight();

        // Overlay the transparent picker across the entire cell so tapping
        // anywhere opens the native picker dialog (Focus() is unreliable on Android)
        Grid.SetColumn(hiddenPicker, 0);
        Grid.SetColumnSpan(hiddenPicker, 3);
        Grid.SetRow(hiddenPicker, 0);
        Grid.SetRowSpan(hiddenPicker, 2);
        RootGrid.Children.Add(hiddenPicker);

        return valueLabel;
    }

    protected override void OnCellTapped(object? sender, TappedEventArgs e)
    {
        // Native picker overlay handles all touch interaction
    }

    void UpdatePickerItems()
    {
        if (hiddenPicker == null || ItemsSource == null) return;

        hiddenPicker.ItemsSource = ItemsSource;
        if (!string.IsNullOrEmpty(DisplayMember))
            hiddenPicker.ItemDisplayBinding = new Binding(DisplayMember);
    }

    void OnSelectedIndexChanged()
    {
        if (hiddenPicker != null && hiddenPicker.SelectedIndex != SelectedIndex)
            hiddenPicker.SelectedIndex = SelectedIndex;
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        if (valueLabel == null || hiddenPicker == null) return;

        if (hiddenPicker.SelectedItem != null)
        {
            if (!string.IsNullOrEmpty(DisplayMember))
            {
                var prop = hiddenPicker.SelectedItem.GetType().GetProperty(DisplayMember);
                valueLabel.Text = prop?.GetValue(hiddenPicker.SelectedItem)?.ToString() ?? hiddenPicker.SelectedItem.ToString();
            }
            else
            {
                valueLabel.Text = hiddenPicker.SelectedItem.ToString();
            }
        }
        else
        {
            valueLabel.Text = string.Empty;
        }
    }

    void UpdateValueColor()
    {
        var color = ValueTextColor ?? ParentTableView?.CellValueTextColor;
        if (color != null)
            valueLabel.TextColor = color;
        else
            valueLabel.ClearValue(Label.TextColorProperty);
    }
}