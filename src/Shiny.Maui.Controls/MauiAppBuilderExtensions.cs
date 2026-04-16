using Microsoft.Maui.Handlers;
using Shiny.Maui.Controls;

namespace Shiny;

public static class ControlsMauiAppBuilderExtensions
{
    public static MauiAppBuilder UseShinyControls(this MauiAppBuilder builder)
    {
        EntryHandler.Mapper.AppendToMapping("ShinyBorderless", (handler, view) =>
        {
            if (view is not BorderlessEntry)
                return;

#if ANDROID
            handler.PlatformView.Background = null;
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

        return builder;
    }
}