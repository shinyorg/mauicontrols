# ImageEditor

An inline image editor with cropping, rotation, freehand drawing with color, text annotations, pinch-to-zoom, undo/redo, reset, and export to PNG/JPEG/WEBP at configurable resolutions. Every feature can be toggled on or off via properties, and the built-in toolbar can be replaced entirely with a `ToolbarTemplate`.

## Basic Usage

```xml
<shiny:ImageEditor Source="{Binding ImageData}"
                   CurrentToolMode="{Binding ToolMode}"
                   AllowCrop="True"
                   AllowRotate="True"
                   AllowDraw="True"
                   AllowTextAnnotation="True"
                   DrawStrokeColor="Red"
                   DrawStrokeWidth="3" />
```

## ImageEditor Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Source` | `byte[]?` | `null` | OneWay | Image data to edit |
| `CurrentToolMode` | `ImageEditorToolMode` | `None` | TwoWay | Active tool: None, Crop, Draw, Text |
| `AllowCrop` | `bool` | `true` | OneWay | Enable/disable crop tool and toolbar button |
| `AllowRotate` | `bool` | `true` | OneWay | Enable/disable rotate action and toolbar button |
| `AllowDraw` | `bool` | `true` | OneWay | Enable/disable freehand drawing tool and toolbar button |
| `AllowTextAnnotation` | `bool` | `true` | OneWay | Enable/disable text annotation tool and toolbar button |
| `AllowZoom` | `bool` | `true` | OneWay | Enable/disable pinch-to-zoom |
| `CanUndo` | `bool` | `false` | OneWayToSource | Whether undo is available |
| `CanRedo` | `bool` | `false` | OneWayToSource | Whether redo is available |
| `DrawStrokeColor` | `Color` | `Red` | OneWay | Drawing stroke color |
| `DrawStrokeWidth` | `double` | `3` | OneWay | Drawing stroke width |
| `TextFontSize` | `double` | `16` | OneWay | Text annotation font size |
| `AnnotationTextColor` | `Color` | `White` | OneWay | Text annotation color |
| `ToolbarTemplate` | `DataTemplate?` | `null` | OneWay | Custom toolbar (replaces the default toolbar entirely) |
| `ToolbarPosition` | `ToolbarPosition` | `Bottom` | OneWay | Toolbar placement: Top or Bottom |
| `UseFeedback` | `bool` | `true` | OneWay | Feedback on tool actions |

## ImageEditor Commands

| Command | Parameter | Description |
|---|---|---|
| `UndoCommand` | — | Undo the last edit action |
| `RedoCommand` | — | Redo the last undone action |
| `RotateCommand` | `float` (degrees) | Rotate the image |
| `ResetCommand` | — | Clear all edits and restore the original image |
| `CropCommand` | — | Toggle crop mode on/off |
| `DrawCommand` | — | Toggle draw mode on/off |
| `TextCommand` | — | Toggle text mode on/off |

## ImageEditor Methods

| Method | Returns | Description |
|---|---|---|
| `Undo()` | `void` | Undo the last action |
| `Redo()` | `void` | Redo the last undone action |
| `Rotate(float degrees)` | `void` | Rotate by the given angle |
| `Reset()` | `void` | Clear all edits |
| `ApplyCrop()` | `void` | Commit the active crop selection |
| `ExportAsync(ImageExportOptions?)` | `Task<Stream>` | Export the edited image |

## ImageEditor Features

- **Crop**: Drag-handle area selection starting at full image; areas outside the crop are dimmed, the selected area stays fully lit. 8 drag handles (4 corners + 4 midpoints) with rule-of-thirds grid overlay.
- **Rotate**: 90° increments or arbitrary angle rotation. Each rotation is an undoable action.
- **Freehand drawing**: Draw strokes with configurable color and width. Each completed stroke is one undoable action.
- **Text annotations**: Tap to place text on the image. Prompts for input, configurable font size and color.
- **Pinch-to-zoom**: View-only zoom that does not affect the exported image.
- **Undo/redo**: Every edit action (crop, rotate, draw stroke, text) is pushed to a stack. Undo pops to a redo stack; redo re-applies.
- **Reset**: Clears all actions and restores the original image.
- **Export**: Render the edited image to a stream (MAUI) or byte array (Blazor) in PNG, JPEG, or WEBP format at a target resolution.
- **Toolbar**: Default toolbar shows buttons for each enabled feature plus undo/redo/reset. Set `AllowX=false` to hide a button. Replace entirely with `ToolbarTemplate`.

## ImageEditor ViewModel Pattern

```csharp
public partial class ImageEditorViewModel : ObservableObject
{
    [ObservableProperty] byte[]? imageData;
    [ObservableProperty] bool canUndo;
    [ObservableProperty] bool canRedo;
    [ObservableProperty] ImageEditorToolMode currentToolMode;
    [ObservableProperty] Color drawColor = Colors.Red;

    [RelayCommand]
    async Task LoadImage()
    {
        var result = await MediaPicker.PickPhotoAsync();
        if (result == null) return;
        using var stream = await result.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ImageData = ms.ToArray();
    }
}
```

## ImageEditor Export

```csharp
// MAUI — get a Stream
var stream = await editor.ExportAsync(new ImageExportOptions
{
    Format = ImageExportFormat.Jpeg,
    Quality = 0.85f,
    Width = 1920,
    Height = 1080
});

// Blazor — get byte[]
var bytes = await editor.ExportAsync("jpeg", 0.85, 1920, 1080);
```
