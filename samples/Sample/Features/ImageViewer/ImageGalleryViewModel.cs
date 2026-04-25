using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.ImageViewer;

public partial class GalleryImage : ObservableObject
{
    public required string Source { get; init; }
    public required string Title { get; init; }

    [ObservableProperty]
    bool isOpen;
}

public partial class ImageGalleryViewModel : ObservableObject
{
    public List<GalleryImage> Images { get; } = new()
    {
        new GalleryImage { Source = "dotnet_bot.png", Title = ".NET Bot" },
        new GalleryImage { Source = "dotnet_bot.png", Title = "Bot Again" },
        new GalleryImage { Source = "dotnet_bot.png", Title = "Third Bot" },
        new GalleryImage { Source = "dotnet_bot.png", Title = "Fourth Bot" },
        new GalleryImage { Source = "dotnet_bot.png", Title = "Fifth Bot" },
        new GalleryImage { Source = "dotnet_bot.png", Title = "Sixth Bot" }
    };
}
