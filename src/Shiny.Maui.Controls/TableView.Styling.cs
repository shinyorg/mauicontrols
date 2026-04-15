namespace Shiny.Maui.Controls;

public partial class TableView
{
    static void OnGlobalStyleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((TableView)bindable).ReapplyCascadedStyles();
    }

    void ReapplyCascadedStyles()
    {
        foreach (var section in GetAllSections())
        {
            foreach (var cell in section.GetVisibleCells())
            {
                cell.ParentTableView = this;
                cell.ApplyCascadedStyles();
            }
        }
    }


    public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(
        nameof(HeaderBackgroundColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create(
        nameof(HeaderTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderFontSizeProperty = BindableProperty.Create(
        nameof(HeaderFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderFontFamilyProperty = BindableProperty.Create(
        nameof(HeaderFontFamily), typeof(string), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderFontAttributesProperty = BindableProperty.Create(
        nameof(HeaderFontAttributes), typeof(FontAttributes), typeof(TableView), FontAttributes.Bold,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderPaddingProperty = BindableProperty.Create(
        nameof(HeaderPadding), typeof(Thickness), typeof(TableView), new Thickness(14, 8, 8, 8),
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create(
        nameof(HeaderHeight), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderTextVerticalAlignProperty = BindableProperty.Create(
        nameof(HeaderTextVerticalAlign), typeof(LayoutAlignment), typeof(TableView), LayoutAlignment.End,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public Color? HeaderBackgroundColor
    {
        get => (Color?)GetValue(HeaderBackgroundColorProperty);
        set => SetValue(HeaderBackgroundColorProperty, value);
    }

    public Color? HeaderTextColor
    {
        get => (Color?)GetValue(HeaderTextColorProperty);
        set => SetValue(HeaderTextColorProperty, value);
    }

    public double HeaderFontSize
    {
        get => (double)GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    public string? HeaderFontFamily
    {
        get => (string?)GetValue(HeaderFontFamilyProperty);
        set => SetValue(HeaderFontFamilyProperty, value);
    }

    public FontAttributes HeaderFontAttributes
    {
        get => (FontAttributes)GetValue(HeaderFontAttributesProperty);
        set => SetValue(HeaderFontAttributesProperty, value);
    }

    public Thickness HeaderPadding
    {
        get => (Thickness)GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }

    public double HeaderHeight
    {
        get => (double)GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    public LayoutAlignment HeaderTextVerticalAlign
    {
        get => (LayoutAlignment)GetValue(HeaderTextVerticalAlignProperty);
        set => SetValue(HeaderTextVerticalAlignProperty, value);
    }



    public static readonly BindableProperty FooterTextColorProperty = BindableProperty.Create(
        nameof(FooterTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty FooterFontSizeProperty = BindableProperty.Create(
        nameof(FooterFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty FooterFontFamilyProperty = BindableProperty.Create(
        nameof(FooterFontFamily), typeof(string), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty FooterFontAttributesProperty = BindableProperty.Create(
        nameof(FooterFontAttributes), typeof(FontAttributes), typeof(TableView), FontAttributes.None,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty FooterPaddingProperty = BindableProperty.Create(
        nameof(FooterPadding), typeof(Thickness), typeof(TableView), new Thickness(14, 8, 8, 8),
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty FooterBackgroundColorProperty = BindableProperty.Create(
        nameof(FooterBackgroundColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public Color? FooterTextColor
    {
        get => (Color?)GetValue(FooterTextColorProperty);
        set => SetValue(FooterTextColorProperty, value);
    }

    public double FooterFontSize
    {
        get => (double)GetValue(FooterFontSizeProperty);
        set => SetValue(FooterFontSizeProperty, value);
    }

    public string? FooterFontFamily
    {
        get => (string?)GetValue(FooterFontFamilyProperty);
        set => SetValue(FooterFontFamilyProperty, value);
    }

    public FontAttributes FooterFontAttributes
    {
        get => (FontAttributes)GetValue(FooterFontAttributesProperty);
        set => SetValue(FooterFontAttributesProperty, value);
    }

    public Thickness FooterPadding
    {
        get => (Thickness)GetValue(FooterPaddingProperty);
        set => SetValue(FooterPaddingProperty, value);
    }

    public Color? FooterBackgroundColor
    {
        get => (Color?)GetValue(FooterBackgroundColorProperty);
        set => SetValue(FooterBackgroundColorProperty, value);
    }



    public static readonly BindableProperty CellTitleColorProperty = BindableProperty.Create(
        nameof(CellTitleColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellTitleFontSizeProperty = BindableProperty.Create(
        nameof(CellTitleFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellTitleFontFamilyProperty = BindableProperty.Create(
        nameof(CellTitleFontFamily), typeof(string), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellTitleFontAttributesProperty = BindableProperty.Create(
        nameof(CellTitleFontAttributes), typeof(FontAttributes?), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellTitleColor
    {
        get => (Color?)GetValue(CellTitleColorProperty);
        set => SetValue(CellTitleColorProperty, value);
    }

    public double CellTitleFontSize
    {
        get => (double)GetValue(CellTitleFontSizeProperty);
        set => SetValue(CellTitleFontSizeProperty, value);
    }

    public string? CellTitleFontFamily
    {
        get => (string?)GetValue(CellTitleFontFamilyProperty);
        set => SetValue(CellTitleFontFamilyProperty, value);
    }

    public FontAttributes? CellTitleFontAttributes
    {
        get => (FontAttributes?)GetValue(CellTitleFontAttributesProperty);
        set => SetValue(CellTitleFontAttributesProperty, value);
    }



    public static readonly BindableProperty CellDescriptionColorProperty = BindableProperty.Create(
        nameof(CellDescriptionColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellDescriptionFontSizeProperty = BindableProperty.Create(
        nameof(CellDescriptionFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellDescriptionFontFamilyProperty = BindableProperty.Create(
        nameof(CellDescriptionFontFamily), typeof(string), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellDescriptionFontAttributesProperty = BindableProperty.Create(
        nameof(CellDescriptionFontAttributes), typeof(FontAttributes?), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellDescriptionColor
    {
        get => (Color?)GetValue(CellDescriptionColorProperty);
        set => SetValue(CellDescriptionColorProperty, value);
    }

    public double CellDescriptionFontSize
    {
        get => (double)GetValue(CellDescriptionFontSizeProperty);
        set => SetValue(CellDescriptionFontSizeProperty, value);
    }

    public string? CellDescriptionFontFamily
    {
        get => (string?)GetValue(CellDescriptionFontFamilyProperty);
        set => SetValue(CellDescriptionFontFamilyProperty, value);
    }

    public FontAttributes? CellDescriptionFontAttributes
    {
        get => (FontAttributes?)GetValue(CellDescriptionFontAttributesProperty);
        set => SetValue(CellDescriptionFontAttributesProperty, value);
    }



    public static readonly BindableProperty CellHintTextColorProperty = BindableProperty.Create(
        nameof(CellHintTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellHintTextFontSizeProperty = BindableProperty.Create(
        nameof(CellHintTextFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellHintFontFamilyProperty = BindableProperty.Create(
        nameof(CellHintFontFamily), typeof(string), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellHintFontAttributesProperty = BindableProperty.Create(
        nameof(CellHintFontAttributes), typeof(FontAttributes?), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellHintTextColor
    {
        get => (Color?)GetValue(CellHintTextColorProperty);
        set => SetValue(CellHintTextColorProperty, value);
    }

    public double CellHintTextFontSize
    {
        get => (double)GetValue(CellHintTextFontSizeProperty);
        set => SetValue(CellHintTextFontSizeProperty, value);
    }

    public string? CellHintFontFamily
    {
        get => (string?)GetValue(CellHintFontFamilyProperty);
        set => SetValue(CellHintFontFamilyProperty, value);
    }

    public FontAttributes? CellHintFontAttributes
    {
        get => (FontAttributes?)GetValue(CellHintFontAttributesProperty);
        set => SetValue(CellHintFontAttributesProperty, value);
    }



    public static readonly BindableProperty CellIconSizeProperty = BindableProperty.Create(
        nameof(CellIconSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellIconRadiusProperty = BindableProperty.Create(
        nameof(CellIconRadius), typeof(double), typeof(TableView), 6d,
        propertyChanged: OnGlobalStyleChanged);

    public double CellIconSize
    {
        get => (double)GetValue(CellIconSizeProperty);
        set => SetValue(CellIconSizeProperty, value);
    }

    public double CellIconRadius
    {
        get => (double)GetValue(CellIconRadiusProperty);
        set => SetValue(CellIconRadiusProperty, value);
    }



    public static readonly BindableProperty CellBackgroundColorProperty = BindableProperty.Create(
        nameof(CellBackgroundColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellSelectedColorProperty = BindableProperty.Create(
        nameof(CellSelectedColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellBackgroundColor
    {
        get => (Color?)GetValue(CellBackgroundColorProperty);
        set => SetValue(CellBackgroundColorProperty, value);
    }

    public Color? CellSelectedColor
    {
        get => (Color?)GetValue(CellSelectedColorProperty);
        set => SetValue(CellSelectedColorProperty, value);
    }



    public static readonly BindableProperty CellValueTextColorProperty = BindableProperty.Create(
        nameof(CellValueTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellValueTextFontSizeProperty = BindableProperty.Create(
        nameof(CellValueTextFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellValueTextFontFamilyProperty = BindableProperty.Create(
        nameof(CellValueTextFontFamily), typeof(string), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellValueTextFontAttributesProperty = BindableProperty.Create(
        nameof(CellValueTextFontAttributes), typeof(FontAttributes?), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellValueTextColor
    {
        get => (Color?)GetValue(CellValueTextColorProperty);
        set => SetValue(CellValueTextColorProperty, value);
    }

    public double CellValueTextFontSize
    {
        get => (double)GetValue(CellValueTextFontSizeProperty);
        set => SetValue(CellValueTextFontSizeProperty, value);
    }

    public string? CellValueTextFontFamily
    {
        get => (string?)GetValue(CellValueTextFontFamilyProperty);
        set => SetValue(CellValueTextFontFamilyProperty, value);
    }

    public FontAttributes? CellValueTextFontAttributes
    {
        get => (FontAttributes?)GetValue(CellValueTextFontAttributesProperty);
        set => SetValue(CellValueTextFontAttributesProperty, value);
    }



    public static readonly BindableProperty CellAccentColorProperty = BindableProperty.Create(
        nameof(CellAccentColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellAccentColor
    {
        get => (Color?)GetValue(CellAccentColorProperty);
        set => SetValue(CellAccentColorProperty, value);
    }



    public static readonly BindableProperty CellPaddingProperty = BindableProperty.Create(
        nameof(CellPadding), typeof(Thickness?), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Thickness? CellPadding
    {
        get => (Thickness?)GetValue(CellPaddingProperty);
        set => SetValue(CellPaddingProperty, value);
    }



    public static readonly BindableProperty CellBorderColorProperty = BindableProperty.Create(
        nameof(CellBorderColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellBorderWidthProperty = BindableProperty.Create(
        nameof(CellBorderWidth), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellBorderRadiusProperty = BindableProperty.Create(
        nameof(CellBorderRadius), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellBorderColor
    {
        get => (Color?)GetValue(CellBorderColorProperty);
        set => SetValue(CellBorderColorProperty, value);
    }

    public double CellBorderWidth
    {
        get => (double)GetValue(CellBorderWidthProperty);
        set => SetValue(CellBorderWidthProperty, value);
    }

    public double CellBorderRadius
    {
        get => (double)GetValue(CellBorderRadiusProperty);
        set => SetValue(CellBorderRadiusProperty, value);
    }

}