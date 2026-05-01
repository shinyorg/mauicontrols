using Shiny.Maui.Controls.FloatingPanel;

namespace Shiny.Maui.Controls.Pickers;

static class PickerHelper
{
    public static OverlayHost? FindOverlayHost(Element element)
    {
        Element? current = element.Parent;
        while (current != null)
        {
            if (current is ShinyContentPage scp)
                return scp.OverlayHost;

            if (current is Page page)
                return FindInVisualTree<OverlayHost>(page);

            current = current.Parent;
        }
        return null;
    }

    static T? FindInVisualTree<T>(Element root) where T : class
    {
        if (root is T match)
            return match;

        if (root is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is Element element)
                {
                    var result = FindInVisualTree<T>(element);
                    if (result != null) return result;
                }
            }
        }
        else if (root is ContentView cv && cv.Content is Element content)
        {
            return FindInVisualTree<T>(content);
        }
        else if (root is ContentPage cp && cp.Content is Element cpContent)
        {
            return FindInVisualTree<T>(cpContent);
        }

        return null;
    }
}
