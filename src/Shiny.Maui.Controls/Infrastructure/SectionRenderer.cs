using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.Controls.Cells;
using TvTableView = Shiny.Maui.Controls.TableView;
using TvTableSection = Shiny.Maui.Controls.Sections.TableSection;

namespace Shiny.Maui.Controls.Infrastructure;

static class SectionRenderer
{
    public static View Render(TvTableSection section, TvTableView parentTableView)
    {
        if (!section.IsVisible)
            return new ContentView { IsVisible = false };

        var sectionLayout = new VerticalStackLayout();

        // Header
        RenderHeader(sectionLayout, section, parentTableView);

        // Cells with separators
        var cells = section.GetVisibleCells();
        for (var i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.ParentTableView = parentTableView;
            cell.ParentSection = section;
            cell.ApplyCascadedStyles();

            if (section.UseDragSort)
            {
                var wrappedCell = WrapWithReorderControls(cell, section, parentTableView);
                sectionLayout.Children.Add(wrappedCell);
            }
            else
            {
                sectionLayout.Children.Add(cell);
            }

            // Separator between cells (not after last)
            if (i < cells.Count - 1)
            {
                sectionLayout.Children.Add(CreateSeparator(parentTableView));
            }
        }

        // Footer
        RenderFooter(sectionLayout, section, parentTableView);

        return sectionLayout;
    }

    static void RenderHeader(VerticalStackLayout layout, TvTableSection section, TvTableView tableView)
    {
        if (section.HeaderView != null)
        {
            layout.Children.Add(section.HeaderView);
            return;
        }

        if (string.IsNullOrEmpty(section.Title))
            return;

        var headerColor = section.HeaderBackgroundColor ?? tableView.HeaderBackgroundColor;
        var textColor = section.HeaderTextColor ?? tableView.HeaderTextColor;
        var fontSize = section.HeaderFontSize >= 0 ? section.HeaderFontSize
            : tableView.HeaderFontSize >= 0 ? tableView.HeaderFontSize
            : 14;
        var fontFamily = section.HeaderFontFamily ?? tableView.HeaderFontFamily;
        var fontAttributes = section.HeaderFontAttributes ?? tableView.HeaderFontAttributes;

        var headerHeight = section.HeaderHeight >= 0 ? section.HeaderHeight
            : tableView.HeaderHeight >= 0 ? tableView.HeaderHeight
            : -1;

        var headerContainer = new ContentView
        {
            Padding = tableView.HeaderPadding,
            BackgroundColor = headerColor ?? GetDefaultHeaderBackgroundColor()
        };

        if (headerHeight >= 0)
            headerContainer.HeightRequest = headerHeight;

        var verticalAlign = headerHeight >= 0
            ? ToLayoutOptions(tableView.HeaderTextVerticalAlign)
            : LayoutOptions.Center;

        var headerLabel = new Label
        {
            Text = section.Title,
            FontSize = fontSize,
            FontAttributes = fontAttributes,
            TextColor = textColor ?? GetDefaultHeaderTextColor(),
            VerticalOptions = verticalAlign
        };

        if (fontFamily != null)
            headerLabel.FontFamily = fontFamily;

        headerContainer.Content = headerLabel;
        layout.Children.Add(headerContainer);
    }

    static void RenderFooter(VerticalStackLayout layout, TvTableSection section, TvTableView tableView)
    {
        if (!section.FooterVisible)
            return;

        if (section.FooterView != null)
        {
            layout.Children.Add(section.FooterView);
            return;
        }

        if (string.IsNullOrEmpty(section.FooterText))
            return;

        var bgColor = section.FooterBackgroundColor ?? tableView.FooterBackgroundColor;
        var textColor = section.FooterTextColor ?? tableView.FooterTextColor;
        var fontSize = section.FooterFontSize >= 0 ? section.FooterFontSize
            : tableView.FooterFontSize >= 0 ? tableView.FooterFontSize
            : 12;
        var fontFamily = section.FooterFontFamily ?? tableView.FooterFontFamily;
        var fontAttributes = section.FooterFontAttributes ?? tableView.FooterFontAttributes;

        var footerContainer = new ContentView
        {
            Padding = tableView.FooterPadding
        };
        if (bgColor != null)
            footerContainer.BackgroundColor = bgColor;

        var footerLabel = new Label
        {
            Text = section.FooterText,
            FontSize = fontSize,
            FontAttributes = fontAttributes,
            TextColor = textColor ?? GetDefaultHeaderTextColor()
        };

        if (fontFamily != null)
            footerLabel.FontFamily = fontFamily;

        footerContainer.Content = footerLabel;
        layout.Children.Add(footerContainer);
    }

    // iOS system separator colors
    // Light: rgba(60, 60, 67, 0.29)  Dark: rgba(84, 84, 88, 0.6)
    static Color GetDefaultSeparatorColor()
    {
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        return isDark
            ? Color.FromRgba(84, 84, 88, 153)
            : Color.FromRgba(60, 60, 67, 74);
    }

    // iOS secondary label color (used for section header/footer text)
    // Light: rgba(60, 60, 67, 0.6)  Dark: rgba(235, 235, 245, 0.6)
    static Color GetDefaultHeaderTextColor()
    {
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        return isDark
            ? Color.FromRgba(235, 235, 245, 153)
            : Color.FromRgba(60, 60, 67, 153);
    }

    // iOS grouped table header background
    // Light: #F2F2F7  Dark: #1C1C1E
    static Color GetDefaultHeaderBackgroundColor()
    {
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        return isDark
            ? Color.FromRgb(28, 28, 30)
            : Color.FromRgb(242, 242, 247);
    }

    static BoxView CreateSeparator(TvTableView tableView)
    {
        return new BoxView
        {
            HeightRequest = tableView.SeparatorHeight >= 0 ? tableView.SeparatorHeight : 0.5,
            Margin = new Thickness(tableView.SeparatorPadding >= 0 ? tableView.SeparatorPadding : 16, 0, 0, 0),
            Color = tableView.SeparatorColor ?? GetDefaultSeparatorColor()
        };
    }

    static View WrapWithReorderControls(CellBase cell, TvTableSection section, TvTableView tableView)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };

        Grid.SetColumn(cell, 0);
        grid.Children.Add(cell);

        // Drag handle – long-press initiates drag, which disambiguates from ScrollView scrolling
        var dragHandle = new Label
        {
            Text = "\u2630",
            FontSize = 18,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(12, 8),
            Opacity = 0.4
        };

        var dragGesture = new DragGestureRecognizer { CanDrag = true };
        dragGesture.DragStarting += (s, e) =>
        {
            e.Data.Properties["DragCell"] = cell;
            e.Data.Properties["DragSection"] = section;
        };
        dragHandle.GestureRecognizers.Add(dragGesture);

        Grid.SetColumn(dragHandle, 1);
        grid.Children.Add(dragHandle);

        // Each row is a drop target
        var dropGesture = new DropGestureRecognizer { AllowDrop = true };
        dropGesture.DragOver += (s, e) =>
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        };
        dropGesture.Drop += (s, e) =>
        {
            if (e.Data.Properties.TryGetValue("DragCell", out var draggedObj) &&
                e.Data.Properties.TryGetValue("DragSection", out var sectionObj) &&
                draggedObj is CellBase draggedCell &&
                sectionObj is TvTableSection draggedSection &&
                draggedSection == section &&
                draggedCell != cell)
            {
                var fromIndex = section.Cells.IndexOf(draggedCell);
                var toIndex = section.Cells.IndexOf(cell);
                if (fromIndex >= 0 && toIndex >= 0)
                    MoveCell(tableView, section, draggedCell, fromIndex, toIndex);
            }
        };
        grid.GestureRecognizers.Add(dropGesture);

        return grid;
    }

    static void MoveCell(TvTableView tableView, TvTableSection section, CellBase cell, int fromIndex, int toIndex)
    {
        tableView.SuppressRender = true;
        section.Cells.Move(fromIndex, toIndex);
        tableView.SuppressRender = false;

        // Defer re-render to next frame so the current touch event completes first
        tableView.Dispatcher.Dispatch(() =>
        {
            tableView.RenderSections();
            tableView.RaiseItemDropped(section, cell, fromIndex, toIndex);
        });
    }

    static LayoutOptions ToLayoutOptions(LayoutAlignment alignment) => alignment switch
    {
        LayoutAlignment.Start => LayoutOptions.Start,
        LayoutAlignment.Center => LayoutOptions.Center,
        LayoutAlignment.End => LayoutOptions.End,
        LayoutAlignment.Fill => LayoutOptions.Fill,
        _ => LayoutOptions.End
    };
}