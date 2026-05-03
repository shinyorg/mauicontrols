# ImageViewer

A full-screen image overlay with pinch-to-zoom, pan (when zoomed), double-tap to toggle zoom, animated fade open/close, and a close button. Designed to overlay page content.

## Basic Usage

```xml
<Grid>
    <!-- Main page content -->
    <ScrollView>
        <VerticalStackLayout>
            <Image Source="photo.png">
                <Image.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding OpenViewerCommand}"
                                          CommandParameter="photo.png" />
                </Image.GestureRecognizers>
            </Image>
        </VerticalStackLayout>
    </ScrollView>

    <!-- ImageViewer overlays on top -->
    <shiny:ImageViewer Source="{Binding SelectedImage}"
                       IsOpen="{Binding IsViewerOpen}" />
</Grid>
```

## ImageViewer Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Source` | `ImageSource?` | `null` | OneWay | The image to display |
| `IsOpen` | `bool` | `false` | TwoWay | Opens/closes the viewer with fade animation |
| `Aspect` | `Aspect` | `AspectFit` | OneWay | Aspect ratio mode for the thumbnail image (MAUI only) |
| `OverlayAspect` | `Aspect` | `AspectFit` | OneWay | Aspect ratio mode for the full-screen overlay image (MAUI only) |
| `OpenViewerOnTap` | `bool` | `true` | OneWay | When true, tapping the thumbnail opens the full-screen viewer; set to false to control opening via code only |
| `MaxZoom` | `double` | `5.0` | OneWay | Maximum pinch zoom scale |
| `CloseButtonTemplate` | `DataTemplate?` | `null` | OneWay | Custom close button template (tapping the templated view closes the viewer) |
| `HeaderTemplate` | `DataTemplate?` | `null` | OneWay | Custom header overlay at the top of the viewer |
| `FooterTemplate` | `DataTemplate?` | `null` | OneWay | Custom footer overlay at the bottom of the viewer |
| `UseFeedback` | `bool` | `true` | OneWay | Feedback on double-tap zoom |

## ImageViewer Features

- **Pinch-to-zoom**: Two-finger pinch gesture scales around the pinch origin, clamped between 1x and MaxZoom
- **Pan when zoomed**: One-finger pan is enabled after zooming in, with translation clamped to image bounds
- **Double-tap to zoom**: Double-tap zooms to 2.5x centered on the tap point; double-tap again resets to 1x
- **Animated open/close**: Backdrop, image, and close button fade in/out together (250ms)
- **Close button**: "✕" button in the top-right corner (customizable via `CloseButtonTemplate`)
- **Header/Footer templates**: Optional overlays at the top/bottom of the viewer for custom UI
- **Backdrop**: Black overlay that swallows touches so nothing falls through to the page behind
- **Touch passthrough**: When `Source` is null, `InputTransparent` is automatically set to true so the viewer does not block touches on content underneath
- **Tap-to-open control**: Set `OpenViewerOnTap="False"` to prevent the thumbnail from opening the viewer on tap — useful when you want to control opening via code only (e.g., from a button or command)

## ImageViewer Placement

ImageViewer must be placed inside a Grid that fills the page so it overlays correctly:

```xml
<ContentPage>
    <Grid>
        <!-- Main page content -->
        <ScrollView>
            <!-- ... -->
        </ScrollView>

        <!-- ImageViewer overlays on top -->
        <shiny:ImageViewer Source="{Binding SelectedImage}"
                           IsOpen="{Binding IsViewerOpen}" />
    </Grid>
</ContentPage>
```

## ImageViewer ViewModel Pattern

```csharp
public partial class ImageViewerViewModel : ObservableObject
{
    [ObservableProperty] ImageSource? selectedImage;
    [ObservableProperty] bool isViewerOpen;

    [RelayCommand]
    void OpenViewer(string imageSource)
    {
        SelectedImage = imageSource;
        IsViewerOpen = true;
    }
}
```
