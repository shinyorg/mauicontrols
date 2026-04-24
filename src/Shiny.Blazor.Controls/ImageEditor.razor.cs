using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Shiny.Blazor.Controls;

public partial class ImageEditor : IAsyncDisposable
{
    IJSObjectReference? module;
    DotNetObjectReference<ImageEditor>? selfRef;
    ElementReference rootEl;
    ElementReference canvasEl;
    bool initialized;
    bool canUndo;
    bool canRedo;
    string currentMode = "none";

    [Parameter] public string? Source { get; set; }
    [Parameter] public byte[]? ImageData { get; set; }
    [Parameter] public bool AllowCrop { get; set; } = true;
    [Parameter] public bool AllowRotate { get; set; } = true;
    [Parameter] public bool AllowDraw { get; set; } = true;
    [Parameter] public bool AllowTextAnnotation { get; set; } = true;
    [Parameter] public bool AllowLine { get; set; } = true;
    [Parameter] public bool AllowArrow { get; set; } = true;
    [Parameter] public bool AllowZoom { get; set; } = true;
    [Parameter] public bool AllowFontSelection { get; set; }
    [Parameter] public bool AllowFontSizeSelection { get; set; }
    [Parameter] public string DrawStrokeColor { get; set; } = "#ff0000";
    [Parameter] public double DrawStrokeWidth { get; set; } = 3;
    [Parameter] public double TextFontSize { get; set; } = 16;
    [Parameter] public string TextColor { get; set; } = "#ffffff";
    [Parameter] public string TextFontFamily { get; set; } = "Arial";
    [Parameter] public IEnumerable<string>? AvailableFonts { get; set; }
    [Parameter] public IEnumerable<double>? AvailableFontSizes { get; set; }
    [Parameter] public EventCallback<string> TextFontFamilyChanged { get; set; }
    [Parameter] public EventCallback<double> TextFontSizeChanged { get; set; }
    [Parameter] public string ToolbarPosition { get; set; } = "bottom";
    [Parameter] public RenderFragment? ToolbarTemplate { get; set; }
    [Parameter] public EventCallback<bool> CanUndoChanged { get; set; }
    [Parameter] public EventCallback<bool> CanRedoChanged { get; set; }

    string? previousSource;
    byte[]? previousImageData;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/Shiny.Blazor.Controls/image-editor.js");

            selfRef = DotNetObjectReference.Create(this);

            await module.InvokeVoidAsync("init", rootEl, canvasEl, selfRef, new
            {
                drawColor = DrawStrokeColor,
                drawWidth = DrawStrokeWidth,
                textColor = TextColor,
                textSize = TextFontSize,
                textFont = TextFontFamily,
                allowZoom = AllowZoom
            });

            initialized = true;
            await LoadImageAsync();
        }
        else if (initialized)
        {
            await SyncParametersAsync();
        }
    }

    async Task SyncParametersAsync()
    {
        if (module == null)
            return;

        // Check if source changed
        if (Source != previousSource || ImageData != previousImageData)
            await LoadImageAsync();

        await module.InvokeVoidAsync("updateDrawSettings", rootEl, DrawStrokeColor, DrawStrokeWidth);
        await module.InvokeVoidAsync("updateTextSettings", rootEl, TextColor, TextFontSize, TextFontFamily);
        await module.InvokeVoidAsync("updateAllowZoom", rootEl, AllowZoom);
    }

    async Task LoadImageAsync()
    {
        if (module == null) return;

        previousSource = Source;
        previousImageData = ImageData;

        if (ImageData is { Length: > 0 })
            await module.InvokeVoidAsync("loadImageData", rootEl, ImageData);
        else if (!string.IsNullOrEmpty(Source))
            await module.InvokeVoidAsync("loadImage", rootEl, Source);
    }

    // Public methods callable via @ref
    public async ValueTask UndoAsync()
    {
        if (module != null)
            await module.InvokeVoidAsync("undo", rootEl);
    }

    public async ValueTask RedoAsync()
    {
        if (module != null)
            await module.InvokeVoidAsync("redo", rootEl);
    }

    public async ValueTask RotateAsync(float degrees)
    {
        if (module != null)
            await module.InvokeVoidAsync("rotate", rootEl, degrees);
    }

    public async ValueTask ResetAsync()
    {
        if (module != null)
        {
            await module.InvokeVoidAsync("reset", rootEl);
            currentMode = "none";
            StateHasChanged();
        }
    }

    public async ValueTask SetModeAsync(string mode)
    {
        if (module != null)
        {
            await module.InvokeVoidAsync("setMode", rootEl, mode);
            currentMode = mode;
            StateHasChanged();
        }
    }

    public async ValueTask ApplyCropAsync()
    {
        if (module != null)
        {
            await module.InvokeVoidAsync("applyCrop", rootEl);
            currentMode = "none";
            StateHasChanged();
        }
    }

    public async Task<byte[]> ExportAsync(string format = "png", double quality = 0.92, int? width = null, int? height = null)
    {
        if (module == null)
            return [];

        return await module.InvokeAsync<byte[]>("exportImage", rootEl, format, quality,
            width ?? 0, height ?? 0);
    }

    // Toolbar actions
    async Task ToggleCrop()
    {
        var newMode = currentMode == "crop" ? "none" : "crop";
        await SetModeAsync(newMode);
    }

    async Task ToggleDraw()
    {
        var newMode = currentMode == "draw" ? "none" : "draw";
        await SetModeAsync(newMode);
    }

    async Task ToggleText()
    {
        var newMode = currentMode == "text" ? "none" : "text";
        await SetModeAsync(newMode);
    }

    async Task ToggleLine()
    {
        var newMode = currentMode == "line" ? "none" : "line";
        await SetModeAsync(newMode);
    }

    async Task ToggleArrow()
    {
        var newMode = currentMode == "arrow" ? "none" : "arrow";
        await SetModeAsync(newMode);
    }

    async Task OnFontFamilySelected(ChangeEventArgs e)
    {
        var value = e.Value?.ToString() ?? string.Empty;
        TextFontFamily = value;
        await TextFontFamilyChanged.InvokeAsync(value);
        if (module != null)
            await module.InvokeVoidAsync("updateTextSettings", rootEl, TextColor, TextFontSize, TextFontFamily);
    }

    async Task OnFontSizeSelected(ChangeEventArgs e)
    {
        if (double.TryParse(e.Value?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var size))
        {
            TextFontSize = size;
            await TextFontSizeChanged.InvokeAsync(size);
            if (module != null)
                await module.InvokeVoidAsync("updateTextSettings", rootEl, TextColor, TextFontSize, TextFontFamily);
        }
    }

    Task CancelCrop() => SetModeAsync("none").AsTask();

    // JS callbacks
    [JSInvokable]
    public async Task OnCanUndoChanged(bool value)
    {
        canUndo = value;
        await CanUndoChanged.InvokeAsync(value);
        StateHasChanged();
    }

    [JSInvokable]
    public async Task OnCanRedoChanged(bool value)
    {
        canRedo = value;
        await CanRedoChanged.InvokeAsync(value);
        StateHasChanged();
    }

    [JSInvokable]
    public async Task<string?> OnPromptText()
    {
        // In Blazor, we use JS prompt as a simple fallback.
        // Users can override ToolbarTemplate for custom text input UI.
        if (module == null) return null;
        return await JS.InvokeAsync<string?>("prompt", "Enter annotation text:");
    }

    public async ValueTask DisposeAsync()
    {
        if (module != null)
        {
            try
            {
                await module.InvokeVoidAsync("dispose", rootEl);
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException) { }
        }
        selfRef?.Dispose();
    }
}
