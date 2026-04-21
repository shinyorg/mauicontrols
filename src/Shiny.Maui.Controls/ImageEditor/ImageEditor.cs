using Shiny.Maui.Controls.ImageEditor.EditActions;

namespace Shiny.Maui.Controls.ImageEditor;

public partial class ImageEditor : ContentView
{
    readonly Grid rootGrid;
    readonly GraphicsView graphicsView;
    readonly ImageEditorDrawable drawable;
    readonly ImageEditorState state;
    View? toolbarView;

    public ImageEditor()
    {
        state = new ImageEditorState();
        drawable = new ImageEditorDrawable();

        state.StateChanged += OnStateChanged;

        graphicsView = new GraphicsView
        {
            Drawable = drawable,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        SetupGestures();
        SetupCommands();

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
        Grid.SetRowSpan(graphicsView, 2);

        BuildDefaultToolbar();

        Content = rootGrid;
    }

    #region Bindable Properties

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(byte[]),
        typeof(ImageEditor),
        null,
        propertyChanged: (b, _, _) => ((ImageEditor)b).OnSourceChanged());

    public byte[]? Source
    {
        get => (byte[]?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty CurrentToolModeProperty = BindableProperty.Create(
        nameof(CurrentToolMode),
        typeof(ImageEditorToolMode),
        typeof(ImageEditor),
        ImageEditorToolMode.None,
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
        nameof(DrawStrokeColor), typeof(Color), typeof(ImageEditor), Colors.Red,
        propertyChanged: (b, _, n) => ((ImageEditor)b).drawable.ActiveStrokeColor = (Color)n);

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

    #endregion

    void OnSourceChanged()
    {
        if (Source == null || Source.Length == 0)
        {
            drawable.Image = null;
        }
        else
        {
            using var stream = new MemoryStream(Source);
            drawable.Image = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
        }

        state.Reset();
        ResetViewTransform();
        Invalidate();
    }

    void OnToolModeChanged(ImageEditorToolMode mode)
    {
        // Finalize any in-progress operations
        FinalizeCurrentOperation();

        drawable.ToolMode = mode;

        if (mode == ImageEditorToolMode.Crop)
        {
            // Start crop with full image selected
            drawable.ActiveCropRect = new RectF(0, 0, 1, 1);
        }
        else
        {
            drawable.ActiveCropRect = null;
        }

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
        drawable.ViewScale = 1f;
        drawable.ViewOffsetX = 0f;
        drawable.ViewOffsetY = 0f;
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

        var toolbar = new HorizontalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(8, 4),
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.6f)
        };

        if (AllowCrop) toolbar.Children.Add(CreateToolButton("\u2702", "Crop", ImageEditorToolMode.Crop));
        if (AllowRotate) toolbar.Children.Add(CreateActionButton("\u21BB", "Rotate", () => Rotate(90)));
        if (AllowDraw) toolbar.Children.Add(CreateToolButton("\u270F", "Draw", ImageEditorToolMode.Draw));
        if (AllowTextAnnotation) toolbar.Children.Add(CreateToolButton("T", "Text", ImageEditorToolMode.Text));

        // Separator
        toolbar.Children.Add(new BoxView { WidthRequest = 1, HeightRequest = 30, Color = Colors.Grey, VerticalOptions = LayoutOptions.Center });

        toolbar.Children.Add(CreateActionButton("\u21B6", "Undo", Undo));
        toolbar.Children.Add(CreateActionButton("\u21B7", "Redo", Redo));
        toolbar.Children.Add(CreateActionButton("\u21BA", "Reset", Reset));

        // Add confirm/cancel buttons for crop mode
        if (CurrentToolMode == ImageEditorToolMode.Crop)
        {
            toolbar.Children.Add(new BoxView { WidthRequest = 1, HeightRequest = 30, Color = Colors.Grey, VerticalOptions = LayoutOptions.Center });
            toolbar.Children.Add(CreateActionButton("\u2713", "Apply", ApplyCrop));
            toolbar.Children.Add(CreateActionButton("\u2717", "Cancel", () => CurrentToolMode = ImageEditorToolMode.None));
        }

        toolbarView = toolbar;
        AddToolbarToGrid();
    }

    Button CreateToolButton(string icon, string tooltip, ImageEditorToolMode mode)
    {
        var btn = CreateBaseButton(icon, tooltip);
        btn.BackgroundColor = CurrentToolMode == mode
            ? Color.FromRgba(255, 255, 255, 0.3f)
            : Colors.Transparent;
        btn.Clicked += (_, _) =>
        {
            CurrentToolMode = CurrentToolMode == mode ? ImageEditorToolMode.None : mode;
        };
        return btn;
    }

    Button CreateActionButton(string icon, string tooltip, Action action)
    {
        var btn = CreateBaseButton(icon, tooltip);
        btn.Clicked += (_, _) => action();
        return btn;
    }

    static Button CreateBaseButton(string icon, string tooltip)
    {
        return new Button
        {
            Text = icon,
            FontSize = 18,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            WidthRequest = 44,
            HeightRequest = 44,
            CornerRadius = 22,
            Padding = 0
        };
    }

    void AddToolbarToGrid()
    {
        if (toolbarView == null)
            return;

        var row = ToolbarPosition == ToolbarPosition.Top ? 0 : 1;
        Grid.SetRow(toolbarView, row);
        toolbarView.VerticalOptions = ToolbarPosition == ToolbarPosition.Top
            ? LayoutOptions.Start
            : LayoutOptions.End;

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
