using Microsoft.Extensions.Logging;
using Sample.Pages;
using Sample.Services;
using Sample.ViewModels;
using Shiny;
using Shiny.Maui.Controls.Scheduler;

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShinyTableView()
            .UseShinyShell(x => x
                .Add<MainPage, MainViewModel>(registerRoute: false)
                .Add<PillPage, PillViewModel>(registerRoute: false)
                .Add<BasicSettingsPage, BasicSettingsViewModel>(registerRoute: false)
                .Add<DragSortPage, DragSortViewModel>(registerRoute: false)
                .Add<DynamicSectionsPage, DynamicSectionsViewModel>(registerRoute: false)
                .Add<PickerDemoPage, PickerDemoViewModel>(registerRoute: false)
                .Add<CalendarPage, CalendarViewModel>(registerRoute: false)
                .Add<AgendaPage, AgendaViewModel>(registerRoute: false)
                .Add<CalendarListPage, CalendarListViewModel>(registerRoute: false)
            )
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<StylingPage>();
        builder.Services.AddTransient<BasicFlowchartPage>();
        builder.Services.AddTransient<DirectionsPage>();
        builder.Services.AddTransient<ThemesPage>();
        builder.Services.AddTransient<SubgraphsPage>();
        builder.Services.AddTransient<InteractiveEditorPage>();
        builder.Services.AddSingleton<ISchedulerEventProvider, SampleSchedulerProvider>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}