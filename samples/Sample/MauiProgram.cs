using Microsoft.Extensions.Logging;
using Sample.Features.BottomSheet;
using Sample.Features.Diagrams;
using Sample.Features.Home;
using Sample.Features.ImageViewer;
using Sample.Features.Markdown;
using Sample.Features.Pills;
using Sample.Features.Scheduler;
using Sample.Features.TableView;
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
            .UseShinyControls()
            .UseShinyShell(x => x
                .Add<HomePage, HomeViewModel>(registerRoute: false)
                .Add<BottomSheetPage, BottomSheetViewModel>(registerRoute: false)
                .Add<PillPage, PillViewModel>(registerRoute: false)
                .Add<BasicSettingsPage, BasicSettingsViewModel>(registerRoute: false)
                .Add<DragSortPage, DragSortViewModel>(registerRoute: false)
                .Add<DynamicSectionsPage, DynamicSectionsViewModel>(registerRoute: false)
                .Add<PickerDemoPage, PickerDemoViewModel>(registerRoute: false)
                .Add<CalendarPage, CalendarViewModel>(registerRoute: false)
                .Add<AgendaPage, AgendaViewModel>(registerRoute: false)
                .Add<CalendarListPage, CalendarListViewModel>(registerRoute: false)
                .Add<ImageViewerPage, ImageViewerViewModel>(registerRoute: false)
                .Add<MarkdownViewPage, MarkdownViewViewModel>(registerRoute: false)
                .Add<MarkdownEditorPage, MarkdownEditorViewModel>(registerRoute: false)
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
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
