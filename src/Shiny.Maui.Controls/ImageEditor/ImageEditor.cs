using Shiny.Maui.Controls.ColorPicker;
using Shiny.Maui.Controls.ImageEditor.EditActions;

namespace Shiny.Maui.Controls.ImageEditor;

public partial class ImageEditor : ContentView
{
    readonly Grid rootGrid;
    readonly GraphicsView graphicsView;
    readonly ImageEditorDrawable drawable;
    readonly ImageEditorState state;
    View? toolbarView;
    ColorPickerButton? drawColorButton;

    public ImageEditor()
    {
        state = new ImageEditorState();
        drawable = new ImageEditorDrawable { State = state };

        state.StateChanged += OnStateChanged;

        graphicsView = new GraphicsView
        {
            Drawable = drawable,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        SetupGestures();
        SetupCommands();
        EnableMoveGestures();

        rootGrid = new Grid
        {
            RowDefinitions =
            [
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            ],
            Children = { graphicsView }
        };

        Grid.SetRow(graphicsView, 0);

        BuildDefaultToolbar();

        Content = rootGrid;
    }

    #region Bindable Properties

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(ImageSource),
        typeof(ImageEditor),
        null,
        propertyChanged: (b, _, _) => _ = ((ImageEditor)b).OnSourceChangedAsync());

    public ImageSource? Source
    {
        get => (ImageSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty CurrentToolModeProperty = BindableProperty.Create(
        nameof(CurrentToolMode),
        typeof(ImageEditorToolMode),
        typeof(ImageEditor),
        ImageEditorToolMode.Move,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((ImageEditor)b).OnToolModeChanged((ImageEditorToolMode)n));

    public ImageEditorToolMode CurrentToolMode
    {
        get => (ImageEditorToolMode)GetValue(CurrentToolModeProperty);
        set => SetValue(CurrentToolModeProperty, value);
    }

    public static readonly BindableProperty AllowCropProperty = BindableProperty.Create(
        nameof(AllowCrop), typeof(bool), typeof(ImageEditor), true,
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public bool AllowCrop
    {
        get => (bool)GetValue(AllowCropProperty);
        set => SetValue(AllowCropProperty, value);
    }

    public static readonly BindableProperty AllowRotateProperty = BindableProperty.Create(
        nameof(AllowRotate), typeof(bool), typeof(ImageEditor), true,
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public bool AllowRotate
    {
        get => (bool)GetValue(AllowRotateProperty);
        set => SetValue(AllowRotateProperty, value);
    }

    public static readonly BindableProperty AllowDrawProperty = BindableProperty.Create(
        nameof(AllowDraw), typeof(bool), typeof(ImageEditor), true,
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public bool AllowDraw
    {
        get => (bool)GetValue(AllowDrawProperty);
        set => SetValue(AllowDrawProperty, value);
    }

    public static readonly BindableProperty AllowTextAnnotationProperty = BindableProperty.Create(
        nameof(AllowTextAnnotation), typeof(bool), typeof(ImageEditor), true,
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public bool AllowTextAnnotation
    {
        get => (bool)GetValue(AllowTextAnnotationProperty);
        set => SetValue(AllowTextAnnotationProperty, value);
    }

    public static readonly BindableProperty AllowZoomProperty = BindableProperty.Create(
        nameof(AllowZoom), typeof(bool), typeof(ImageEditor), true);

    public bool AllowZoom
    {
        get => (bool)GetValue(AllowZoomProperty);
        set => SetValue(AllowZoomProperty, value);
    }

    public static readonly BindableProperty CanUndoProperty = BindableProperty.Create(
        nameof(CanUndo), typeof(bool), typeof(ImageEditor), false, BindingMode.OneWayToSource);

    public bool CanUndo
    {
        get => (bool)GetValue(CanUndoProperty);
        private set => SetValue(CanUndoProperty, value);
    }

    public static readonly BindableProperty CanRedoProperty = BindableProperty.Create(
        nameof(CanRedo), typeof(bool), typeof(ImageEditor), false, BindingMode.OneWayToSource);

    public bool CanRedo
    {
        get => (bool)GetValue(CanRedoProperty);
        private set => SetValue(CanRedoProperty, value);
    }

    public static readonly BindableProperty DrawStrokeColorProperty = BindableProperty.Create(
        nameof(DrawStrokeColor), typeof(Color), typeof(ImageEditor), Colors.White,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) =>
        {
            var editor = (ImageEditor)b;
            editor.drawable.ActiveStrokeColor = (Color)n;
            if (editor.drawColorButton != null)
                editor.drawColorButton.SelectedColor = (Color)n;
        });

    public Color DrawStrokeColor
    {
        get => (Color)GetValue(DrawStrokeColorProperty);
        set => SetValue(DrawStrokeColorProperty, value);
    }

    public static readonly BindableProperty DrawStrokeWidthProperty = BindableProperty.Create(
        nameof(DrawStrokeWidth), typeof(double), typeof(ImageEditor), 3.0,
        propertyChanged: (b, _, n) => ((ImageEditor)b).drawable.ActiveStrokeWidth = (float)(double)n);

    public double DrawStrokeWidth
    {
        get => (double)GetValue(DrawStrokeWidthProperty);
        set => SetValue(DrawStrokeWidthProperty, value);
    }

    public static readonly BindableProperty TextFontSizeProperty = BindableProperty.Create(
        nameof(TextFontSize), typeof(double), typeof(ImageEditor), 16.0);

    public double TextFontSize
    {
        get => (double)GetValue(TextFontSizeProperty);
        set => SetValue(TextFontSizeProperty, value);
    }

    public static readonly BindableProperty AnnotationTextColorProperty = BindableProperty.Create(
        nameof(AnnotationTextColor), typeof(Color), typeof(ImageEditor), Colors.White);

    public Color AnnotationTextColor
    {
        get => (Color)GetValue(AnnotationTextColorProperty);
        set => SetValue(AnnotationTextColorProperty, value);
    }

    public static readonly BindableProperty ToolbarTemplateProperty = BindableProperty.Create(
        nameof(ToolbarTemplate), typeof(DataTemplate), typeof(ImageEditor), null,
        propertyChanged: (b, _, _) => ((ImageEditor)b).ApplyToolbarTemplate());

    public DataTemplate? ToolbarTemplate
    {
        get => (DataTemplate?)GetValue(ToolbarTemplateProperty);
        set => SetValue(ToolbarTemplateProperty, value);
    }

    public static readonly BindableProperty ToolbarPositionProperty = BindableProperty.Create(
        nameof(ToolbarPosition), typeof(ToolbarPosition), typeof(ImageEditor), ToolbarPosition.Bottom,
        propertyChanged: (b, _, _) => ((ImageEditor)b).UpdateToolbarPosition());

    public ToolbarPosition ToolbarPosition
    {
        get => (ToolbarPosition)GetValue(ToolbarPositionProperty);
        set => SetValue(ToolbarPositionProperty, value);
    }

    public static readonly BindableProperty UseHapticFeedbackProperty = BindableProperty.Create(
        nameof(UseHapticFeedback), typeof(bool), typeof(ImageEditor), true);

    public bool UseHapticFeedback
    {
        get => (bool)GetValue(UseHapticFeedbackProperty);
        set => SetValue(UseHapticFeedbackProperty, value);
    }

    public static readonly BindableProperty CropApplyTextProperty = BindableProperty.Create(
        nameof(CropApplyText), typeof(string), typeof(ImageEditor), "Apply Crop",
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public string CropApplyText
    {
        get => (string)GetValue(CropApplyTextProperty);
        set => SetValue(CropApplyTextProperty, value);
    }

    public static readonly BindableProperty CropCancelTextProperty = BindableProperty.Create(
        nameof(CropCancelText), typeof(string), typeof(ImageEditor), "Cancel",
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public string CropCancelText
    {
        get => (string)GetValue(CropCancelTextProperty);
        set => SetValue(CropCancelTextProperty, value);
    }

    public static readonly BindableProperty SaveCommandProperty = BindableProperty.Create(
        nameof(SaveCommand), typeof(System.Windows.Input.ICommand), typeof(ImageEditor), null,
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public System.Windows.Input.ICommand? SaveCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public static readonly BindableProperty SaveTextProperty = BindableProperty.Create(
        nameof(SaveText), typeof(string), typeof(ImageEditor), "Save",
        propertyChanged: (b, _, _) => ((ImageEditor)b).BuildDefaultToolbar());

    public string SaveText
    {
        get => (string)GetValue(SaveTextProperty);
        set => SetValue(SaveTextProperty, value);
    }

    #endregion

    async Task OnSourceChangedAsync()
    {
        if (Source == null)
        {
            drawable.Image = null;
        }
        else
        {
            var stream = await ResolveImageSourceStreamAsync(Source);
            if (stream != null)
            {
                await using (stream)
                    drawable.Image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
            }
            else
            {
                drawable.Image = null;
            }
        }

        state.Reset();
        ResetViewTransform();
        Invalidate();
    }

    static async Task<Stream?> ResolveImageSourceStreamAsync(ImageSource source)
    {
        switch (source)
        {
            case FileImageSource fileSource:
                try { return await FileSystem.OpenAppPackageFileAsync(fileSource.File); }
                catch
                {
                    // Try as a regular file path
                    if (File.Exists(fileSource.File))
                        return File.OpenRead(fileSource.File);
                    return null;
                }

            case StreamImageSource streamSource:
                return await streamSource.Stream(CancellationToken.None);

            case UriImageSource uriSource:
                using (var client = new HttpClient())
                    return new MemoryStream(await client.GetByteArrayAsync(uriSource.Uri));

            default:
                return null;
        }
    }

    void OnToolModeChanged(ImageEditorToolMode mode)
    {
        // Finalize any in-progress operations
        FinalizeCurrentOperation();

        drawable.ToolMode = mode;

        if (mode == ImageEditorToolMode.Crop)
        {
            drawable.ActiveCropRect = new RectF(0.1f, 0.1f, 0.8f, 0.8f);
            ResetViewTransform(); // Ensure crop overlay draws on unzoomed view
        }
        else
        {
            drawable.ActiveCropRect = null;
        }

        // Enable/disable move gestures
        if (mode == ImageEditorToolMode.Move)
            EnableMoveGestures();
        else
            DisableMoveGestures();

        BuildDefaultToolbar();
        Invalidate();
    }

    void OnStateChanged()
    {
        CanUndo = state.CanUndo;
        CanRedo = state.CanRedo;
        Invalidate();
    }

    void FinalizeCurrentOperation()
    {
        // Finalize in-progress text entry
        CommitActiveTextEntry();

        // Finalize in-progress draw stroke
        if (drawable.ActiveStrokePoints is { Count: >= 2 })
        {
            var imageRect = drawable.GetImageRect();
            if (imageRect is { Width: > 0, Height: > 0 })
            {
                var normalized = drawable.ActiveStrokePoints
                    .Select(p => new PointF(
                        (p.X - imageRect.X) / imageRect.Width,
                        (p.Y - imageRect.Y) / imageRect.Height))
                    .ToArray();

                state.Push(new DrawStrokeAction
                {
                    Points = normalized,
                    StrokeColor = DrawStrokeColor,
                    StrokeWidth = (float)DrawStrokeWidth
                });
            }
            drawable.ActiveStrokePoints = null;
        }
    }

    void ResetViewTransform()
    {
        // Reset native view transforms (used by Move mode)
        graphicsView.Scale = 1;
        graphicsView.TranslationX = 0;
        graphicsView.TranslationY = 0;
        currentScale = 1;
        xOffset = 0;
        yOffset = 0;
    }

    void Invalidate() => graphicsView.Invalidate();

    #region Toolbar

    void ApplyToolbarTemplate()
    {
        if (ToolbarTemplate != null)
        {
            RemoveToolbar();
            var content = ToolbarTemplate.CreateContent();
            if (content is View view)
            {
                toolbarView = view;
                AddToolbarToGrid();
            }
        }
        else
        {
            BuildDefaultToolbar();
        }
    }

    void BuildDefaultToolbar()
    {
        // Don't build if custom template is set
        if (ToolbarTemplate != null)
            return;

        RemoveToolbar();

        // When in crop mode, show a focused crop toolbar
        if (CurrentToolMode == ImageEditorToolMode.Crop)
        {
            toolbarView = BuildCropToolbar();
            AddToolbarToGrid();
            return;
        }

        var toolbar = new HorizontalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(8, 4),
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6f)
        };

        if (AllowZoom) toolbar.Children.Add(CreateToolButton("\u21c5", "Move", ImageEditorToolMode.Move));
        if (AllowCrop) toolbar.Children.Add(CreateToolButton("Crop", "Crop", ImageEditorToolMode.Crop));
        if (AllowRotate) toolbar.Children.Add(CreateActionButton("Rot", "Rotate", () => Rotate(90)));
        if (AllowDraw)
        {
            toolbar.Children.Add(CreateToolButton("Draw", "Draw", ImageEditorToolMode.Draw));
            toolbar.Children.Add(CreateDrawColorButton());
        }
        if (AllowTextAnnotation) toolbar.Children.Add(CreateToolButton("Txt", "Text", ImageEditorToolMode.Text));

        // Separator
        toolbar.Children.Add(new BoxView { WidthRequest = 1, HeightRequest = 30, Color = Colors.Grey, VerticalOptions = LayoutOptions.Center });

        toolbar.Children.Add(CreateActionButton("Undo", "Undo", Undo));
        toolbar.Children.Add(CreateActionButton("Redo", "Redo", Redo));
        toolbar.Children.Add(CreateActionButton("Reset", "Reset", Reset));

        if (SaveCommand != null)
        {
            toolbar.Children.Add(new BoxView { WidthRequest = 1, HeightRequest = 30, Color = Colors.Grey, VerticalOptions = LayoutOptions.Center });
            toolbar.Children.Add(CreateActionButton(SaveText, "Save", ExecuteSave));
        }

        toolbarView = toolbar;
        AddToolbarToGrid();
    }

    View BuildCropToolbar()
    {
        var toolbar = new HorizontalStackLayout
        {
            Spacing = 8,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(12, 6),
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6f)
        };

        var cancelBtn = new Button
        {
            Text = CropCancelText,
            FontSize = 14,
            TextColor = Colors.White,
            BackgroundColor = Color.FromRgba(255, 59, 48, 200), // red
            CornerRadius = 8,
            HeightRequest = 40,
            Padding = new Thickness(16, 0)
        };
        cancelBtn.Clicked += (_, _) => CurrentToolMode = ImageEditorToolMode.Move;

        var label = new Label
        {
            Text = "Drag edges to crop",
            TextColor = Colors.White,
            FontSize = 13,
            VerticalOptions = LayoutOptions.Center,
            Opacity = 0.8
        };

        var applyBtn = new Button
        {
            Text = CropApplyText,
            FontSize = 14,
            TextColor = Colors.White,
            BackgroundColor = Color.FromRgba(52, 199, 89, 200), // green
            CornerRadius = 8,
            HeightRequest = 40,
            Padding = new Thickness(16, 0)
        };
        applyBtn.Clicked += (_, _) => ApplyCrop();

        toolbar.Children.Add(cancelBtn);
        toolbar.Children.Add(label);
        toolbar.Children.Add(applyBtn);

        return toolbar;
    }

    Button CreateToolButton(string icon, string tooltip, ImageEditorToolMode mode)
    {
        var btn = CreateBaseButton(icon, tooltip);
        btn.BackgroundColor = CurrentToolMode == mode
            ? Color.FromRgba(255, 255, 255, 0.3f)
            : Colors.Transparent;
        btn.Clicked += (_, _) =>
        {
            CurrentToolMode = CurrentToolMode == mode ? ImageEditorToolMode.Move : mode;
        };
        return btn;
    }

    Button CreateActionButton(string icon, string tooltip, Action action)
    {
        var btn = CreateBaseButton(icon, tooltip);
        btn.Clicked += (_, _) => action();
        return btn;
    }

    ColorPickerButton CreateDrawColorButton()
    {
        drawColorButton = new ColorPickerButton
        {
            SelectedColor = DrawStrokeColor,
            CornerRadius = 22,
            HeightRequest = 36,
            WidthRequest = 36,
            VerticalOptions = LayoutOptions.Center
        };
        drawColorButton.ColorChanged += (_, color) => DrawStrokeColor = color;
        return drawColorButton;
    }

    static Button CreateBaseButton(string icon, string tooltip)
    {
        return new Button
        {
            Text = icon,
            FontSize = 12,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            MinimumWidthRequest = 44,
            HeightRequest = 44,
            CornerRadius = 22,
            Padding = new Thickness(6, 0)
        };
    }

    void AddToolbarToGrid()
    {
        if (toolbarView == null)
            return;

        if (ToolbarPosition == ToolbarPosition.Top)
        {
            // Swap row definitions so toolbar is on top
            rootGrid.RowDefinitions.Clear();
            rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            Grid.SetRow(graphicsView, 1);
            Grid.SetRow(toolbarView, 0);
        }
        else
        {
            rootGrid.RowDefinitions.Clear();
            rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetRow(graphicsView, 0);
            Grid.SetRow(toolbarView, 1);
        }

        rootGrid.Children.Add(toolbarView);
    }

    void RemoveToolbar()
    {
        if (toolbarView != null)
        {
            rootGrid.Children.Remove(toolbarView);
            toolbarView = null;
        }
    }

    void UpdateToolbarPosition()
    {
        if (toolbarView == null)
            return;

        RemoveToolbar();
        if (ToolbarTemplate != null)
            ApplyToolbarTemplate();
        else
            BuildDefaultToolbar();
    }

    #endregion
}

public enum ToolbarPosition
{
    Top,
    Bottom
}
