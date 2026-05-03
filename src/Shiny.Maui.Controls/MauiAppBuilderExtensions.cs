using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Handlers;
using Shiny.Blazor.Controls;
using Shiny.Maui.Controls;
using Shiny.Maui.Controls.Infrastructure;
using Shiny.Maui.Controls.Toast;

namespace Shiny;

public static class ControlsMauiAppBuilderExtensions
{
    public static MauiAppBuilder UseShinyControls(
        this MauiAppBuilder builder, 
        Action<ShinyControlConfiguration>? configure = null
    )
    {
        if (configure != null)
        {
            var cfg = new ShinyControlConfiguration(builder.Services);
            configure.Invoke(cfg);
        }
        builder.Services.TryAddSingleton<IFeedbackService, HapticFeedbackService>();
        builder.Services.TryAddSingleton<IToaster, Toaster>();

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

public class ShinyControlConfiguration(IServiceCollection services)
{
    /// <summary>
    /// Set a custom feedback service implementation. Note that the default implementation is designed to work with various controls, so if you use a custom implementation, you may need to ensure it integrates properly with the Blazor component or provides its own mechanism for providing feedback (e.g., haptic feedback, sound, etc.).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ShinyControlConfiguration SetCustomFeedback<T>() where T : class, IFeedbackService
    {
        services.TryAddSingleton<IFeedbackService, T>();
        return this;
    }

    /// <summary>
    /// Set a custom toaster implementation. Note that the default implementation is designed to work with the Shiny Blazor Toast component, so if you use a custom implementation, you may need to ensure it integrates properly with the Blazor component or provides its own mechanism for displaying toasts.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ShinyControlConfiguration SetCustomToaster<T>() where T : class, IToaster
    {
        services.TryAddSingleton<IToaster, Toaster>();
        return this;
    }


    /// <summary>
    /// Integrate the default Maui control feedback service. This will ensure that the default feedback mechanisms (such as haptic feedback on supported devices) are properly integrated with the controls in your application. By adding this service, you can leverage the built-in feedback capabilities of the Maui framework, providing a more responsive and engaging user experience when interacting with the controls.
    /// </summary>
    /// <returns></returns>
    public ShinyControlConfiguration AddDefaultMauiControlFeedback()
    {
        services.AddSingleton<IMauiInitializeService, MauiControlFeedbackIntegrator>();
        return this;
    }

    /// <summary>
    /// Disable all feedback (haptic, sound, etc.) for the controls. This will replace the default feedback service with a no-op implementation, effectively silencing any feedback that would normally be triggered by user interactions with the controls.
    /// </summary>
    /// <returns></returns>
    public ShinyControlConfiguration DisableFeedback()
    {
        services.AddSingleton<IFeedbackService, NoFeedbackService>();
        return this;
    }
}