using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls;

[ContentProperty(nameof(Items))]
public class FabMenu : ContentView
{
    const uint DefaultAnimationDuration = 200;
    const double ItemStaggerMs = 30;
    const double ItemTravelDistance = 16;

    readonly Grid rootGrid;
    readonly BoxView backdrop;
    readonly VerticalStackLayout stack;
    readonly VerticalStackLayout itemsLayout;
    readonly Fab mainFab;
    readonly TapGestureRecognizer backdropTap;

    bool isAnimating;

    public FabMenu()
    {
        backdrop = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0,
            IsVisible = false
        };
        backdropTap = new TapGestureRecognizer();
        backdropTap.Tapped += OnBackdropTapped;
        backdrop.GestureRecognizers.Add(backdropTap);

        itemsLayout = new VerticalStackLayout
        {
            Spacing = 12,
            HorizontalOptions = LayoutOptions.End
        };

        mainFab = new Fab();
        mainFab.Clicked += OnMainFabClicked;

        stack = new VerticalStackLayout
        {
            Spacing = 16,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End
        };
        stack.Add(itemsLayout);
        stack.Add(mainFab);

        rootGrid = new Grid();
        rootGrid.Children.Add(backdrop);
        rootGrid.Children.Add(stack);

        Content = rootGrid;
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;
        InputTransparent = false;

        // Assign Items last — the ItemsProperty change handler calls RebuildItemsLayout(),
        // which requires itemsLayout to already be constructed.
        Items = new ObservableCollection<FabMenuItem>();
    }


    // ------- Items / ItemsSource -------

    public static readonly BindableProperty ItemsProperty = BindableProperty.Create(
        nameof(Items),
        typeof(IList<FabMenuItem>),
        typeof(FabMenu),
        null,
        propertyChanged: (b, o, n) =>
        {
            var menu = (FabMenu)b;
            if (o is INotifyCollectionChanged oldNotify)
                oldNotify.CollectionChanged -= menu.OnItemsCollectionChanged;
            if (n is INotifyCollectionChanged newNotify)
                newNotify.CollectionChanged += menu.OnItemsCollectionChanged;
            menu.RebuildItemsLayout();
        });
    public IList<FabMenuItem> Items
    {
        get => (IList<FabMenuItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }


    // ------- IsOpen -------

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(FabMenu),
        false,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, n) => _ = ((FabMenu)b).AnimateToStateAsync((bool)n));
    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }


    // ------- Main Fab pass-throughs -------

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(ImageSource),
        typeof(FabMenu),
        null,
        propertyChanged: (b, _, n) => ((FabMenu)b).mainFab.Icon = n as ImageSource);
    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(FabMenu),
        null,
        propertyChanged: (b, _, n) => ((FabMenu)b).mainFab.Text = n as string);
    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty FabBackgroundColorProperty = BindableProperty.Create(
        nameof(FabBackgroundColor),
        typeof(Color),
        typeof(FabMenu),
        Color.FromArgb("#2196F3"),
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((FabMenu)b).mainFab.FabBackgroundColor = c;
        });
    public Color? FabBackgroundColor
    {
        get => (Color?)GetValue(FabBackgroundColorProperty);
        set => SetValue(FabBackgroundColorProperty, value);
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(FabMenu),
        null,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((FabMenu)b).mainFab.BorderColor = c;
        });
    public Color? BorderColor
    {
        get => (Color?)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(double),
        typeof(FabMenu),
        0.0,
        propertyChanged: (b, _, n) => ((FabMenu)b).mainFab.BorderThickness = (double)n);
    public double BorderThickness
    {
        get => (double)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(FabMenu),
        Colors.White,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((FabMenu)b).mainFab.TextColor = c;
        });
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }


    // ------- Behaviour -------

    public static readonly BindableProperty HasBackdropProperty = BindableProperty.Create(
        nameof(HasBackdrop),
        typeof(bool),
        typeof(FabMenu),
        true);
    public bool HasBackdrop
    {
        get => (bool)GetValue(HasBackdropProperty);
        set => SetValue(HasBackdropProperty, value);
    }

    public static readonly BindableProperty CloseOnBackdropTapProperty = BindableProperty.Create(
        nameof(CloseOnBackdropTap),
        typeof(bool),
        typeof(FabMenu),
        true);
    public bool CloseOnBackdropTap
    {
        get => (bool)GetValue(CloseOnBackdropTapProperty);
        set => SetValue(CloseOnBackdropTapProperty, value);
    }

    public static readonly BindableProperty CloseOnItemTapProperty = BindableProperty.Create(
        nameof(CloseOnItemTap),
        typeof(bool),
        typeof(FabMenu),
        true);
    public bool CloseOnItemTap
    {
        get => (bool)GetValue(CloseOnItemTapProperty);
        set => SetValue(CloseOnItemTapProperty, value);
    }

    public static readonly BindableProperty BackdropColorProperty = BindableProperty.Create(
        nameof(BackdropColor),
        typeof(Color),
        typeof(FabMenu),
        Colors.Black,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((FabMenu)b).backdrop.Color = c;
        });
    public Color? BackdropColor
    {
        get => (Color?)GetValue(BackdropColorProperty);
        set => SetValue(BackdropColorProperty, value);
    }

    public static readonly BindableProperty BackdropOpacityProperty = BindableProperty.Create(
        nameof(BackdropOpacity),
        typeof(double),
        typeof(FabMenu),
        0.4);
    public double BackdropOpacity
    {
        get => (double)GetValue(BackdropOpacityProperty);
        set => SetValue(BackdropOpacityProperty, value);
    }

    public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(
        nameof(AnimationDuration),
        typeof(uint),
        typeof(FabMenu),
        DefaultAnimationDuration);
    public uint AnimationDuration
    {
        get => (uint)GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }


    public static readonly BindableProperty UseFeedbackProperty = BindableProperty.Create(
        nameof(UseFeedback),
        typeof(bool),
        typeof(FabMenu),
        true);
    public bool UseFeedback
    {
        get => (bool)GetValue(UseFeedbackProperty);
        set => SetValue(UseFeedbackProperty, value);
    }


    public event EventHandler<FabMenuItem>? ItemTapped;


    public void Open() => IsOpen = true;
    public void Close() => IsOpen = false;
    public void Toggle() => IsOpen = !IsOpen;


    // ------- Internals -------

    void OnMainFabClicked(object? sender, EventArgs e)
    {
        if (UseFeedback)
            FeedbackHelper.Execute(this, "Toggled");
        Toggle();
    }

    void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        if (CloseOnBackdropTap)
            Close();
    }

    void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => RebuildItemsLayout();

    void RebuildItemsLayout()
    {
        // detach old handlers
        foreach (var child in itemsLayout.Children.OfType<FabMenuItem>())
            child.Clicked -= OnMenuItemClicked;

        itemsLayout.Clear();

        if (Items is null)
            return;

        foreach (var item in Items)
        {
            item.Clicked -= OnMenuItemClicked;
            item.Clicked += OnMenuItemClicked;

            // Prime initial state based on IsOpen
            item.Opacity = IsOpen ? 1 : 0;
            item.TranslationY = IsOpen ? 0 : ItemTravelDistance;
            item.IsVisible = IsOpen;

            itemsLayout.Add(item);
        }
    }

    void OnMenuItemClicked(object? sender, EventArgs e)
    {
        if (sender is FabMenuItem item)
        {
            ItemTapped?.Invoke(this, item);
            if (CloseOnItemTap)
                Close();
        }
    }


    async Task AnimateToStateAsync(bool open)
    {
        if (isAnimating)
            return;
        isAnimating = true;
        try
        {
            var duration = AnimationDuration;
            var items = itemsLayout.Children.OfType<FabMenuItem>().ToList();

            if (open)
            {
                // show backdrop
                if (HasBackdrop)
                {
                    backdrop.IsVisible = true;
                    backdrop.Opacity = 0;
                }

                // prime items
                foreach (var item in items)
                {
                    item.IsVisible = true;
                    item.Opacity = 0;
                    item.TranslationY = ItemTravelDistance;
                }

                var tasks = new List<Task>();
                if (HasBackdrop)
                    tasks.Add(backdrop.FadeToAsync(BackdropOpacity, duration, Easing.CubicOut));

                // Stagger from bottom to top (closest to the main Fab animates first)
                for (var i = items.Count - 1; i >= 0; i--)
                {
                    var item = items[i];
                    var delay = (items.Count - 1 - i) * ItemStaggerMs;
                    tasks.Add(AnimateItemAsync(item, 1, 0, duration, delay, Easing.CubicOut));
                }

                await Task.WhenAll(tasks);
            }
            else
            {
                var tasks = new List<Task>();
                if (HasBackdrop)
                    tasks.Add(backdrop.FadeToAsync(0, duration, Easing.CubicIn));

                // Animate top to bottom on close
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var delay = i * ItemStaggerMs;
                    tasks.Add(AnimateItemAsync(item, 0, ItemTravelDistance, duration, delay, Easing.CubicIn));
                }

                await Task.WhenAll(tasks);

                foreach (var item in items)
                    item.IsVisible = false;

                if (HasBackdrop)
                    backdrop.IsVisible = false;
            }
        }
        finally
        {
            isAnimating = false;
        }
    }

    static async Task AnimateItemAsync(View item, double targetOpacity, double targetTranslationY, uint duration, double delayMs, Easing easing)
    {
        if (delayMs > 0)
            await Task.Delay((int)delayMs);

        await Task.WhenAll(
            item.FadeToAsync(targetOpacity, duration, easing),
            item.TranslateToAsync(0, targetTranslationY, duration, easing)
        );
    }
}
