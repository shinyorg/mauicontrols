using Microsoft.Extensions.DependencyInjection;

namespace Shiny.Blazor.Controls.Toast;

public static class ToastServiceExtensions
{
    public static IServiceCollection AddShinyToast(this IServiceCollection services)
    {
        services.AddSingleton<ToastService>();
        services.AddSingleton<IToastService>(sp => sp.GetRequiredService<ToastService>());
        return services;
    }
}
