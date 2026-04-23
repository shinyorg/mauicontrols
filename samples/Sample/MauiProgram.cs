using CommunityToolkit.Maui;
using MauiDevFlow.Agent;
using Microsoft.Extensions.Logging;
using Sample.Features.Sheet;
using Sample.Features.Diagrams;
using Sample.Features.Fab;
using Sample.Features.Home;
using Sample.Features.Chat;
using Sample.Features.ColorPicker;
using Sample.Features.KitchenSink;
using Sample.Features.ImageEditor;
using Sample.Features.ImageViewer;
using Sample.Features.Markdown;
using Sample.Features.Pills;
using Sample.Features.Scheduler;
using Sample.Features.SecurityPin;
using Sample.Features.AutoComplete;
using Sample.Features.CountryAddress;
using Sample.Features.TableView;
using Shiny;
using Shiny.Maui.Controls.Scheduler;
#if IOS
using Sample.Platforms.iOS;
using Microsoft.Maui.Controls.Handlers.Compatibility;
#endif

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseShinyControls()
            .UseShinyShell(x => x
                .Add<HomePage, HomeViewModel>(registerRoute: false)
                .Add<SheetPage, SheetViewModel>(registerRoute: false)
                .Add<MinimizedSheetPage, MinimizedSheetViewModel>(registerRoute: false)
                .Add<MinimizedSheetStandalonePage, MinimizedSheetViewModel>(registerRoute: false)
                .Add<TopSheetPage, TopSheetViewModel>(registerRoute: false)
                .Add<PillPage, PillViewModel>(registerRoute: false)
                .Add<SecurityPinPage, SecurityPinViewModel>(registerRoute: false)
                .Add<FabPage, FabViewModel>(registerRoute: false)
                .Add<BasicSettingsPage, BasicSettingsViewModel>(registerRoute: false)
                .Add<DragSortPage, DragSortViewModel>(registerRoute: false)
                .Add<DynamicSectionsPage, DynamicSectionsViewModel>(registerRoute: false)
                .Add<PickerDemoPage, PickerDemoViewModel>(registerRoute: false)
                .Add<CalendarPage, CalendarViewModel>(registerRoute: false)
                .Add<AgendaPage, AgendaViewModel>(registerRoute: false)
                .Add<AgendaCalendarPickerPage, AgendaCalendarPickerViewModel>(registerRoute: false)
                .Add<CalendarListPage, CalendarListViewModel>(registerRoute: false)
                .Add<ImageViewerPage, ImageViewerViewModel>(registerRoute: false)
                .Add<ImageEditorPage, ImageEditorViewModel>()
                .Add<ChatPage, ChatViewModel>(registerRoute: false)
                .Add<KitchenSinkPage, KitchenSinkViewModel>(registerRoute: false)
                .Add<ColorPickerPage, ColorPickerViewModel>(registerRoute: false)
                .Add<MarkdownViewPage, MarkdownViewViewModel>(registerRoute: false)
                .Add<MarkdownEditorPage, MarkdownEditorViewModel>(registerRoute: false)
                .Add<AutoCompletePage, AutoCompleteViewModel>(registerRoute: false)
                .Add<CountryAddressPage, CountryAddressViewModel>(registerRoute: false)
            )
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.ConfigureMauiHandlers(handlers =>
        {
#if IOS
            handlers.AddHandler<Shell, SolidTabBarRenderer>();
#endif
        });

        builder.Services.AddTransient<MusicBrowsePage>();
        builder.Services.AddTransient<MusicLibraryPage>();
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
        builder.AddMauiDevFlowAgent();
#endif

        return builder.Build();
    }
}
