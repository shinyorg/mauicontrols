namespace Shiny.Maui.Controls.Infrastructure;

public class MauiControlFeedbackIntegrator : IMauiInitializeService
{
    IFeedbackService? feedback;

    public void Initialize(IServiceProvider services)
    {
        feedback = services.GetService<IFeedbackService>();
        if (feedback == null)
            return;

        var app = services.GetRequiredService<IApplication>();
        if (app is Application application)
            Attach(application);
    }

    void Attach(Application application)
    {
        // Hook existing visual tree
        foreach (var descendant in application.GetVisualTreeDescendants().Skip(1))
        {
            if (descendant is VisualElement element)
                BindElement(element);
        }

        // Hook future additions/removals
        application.DescendantAdded += OnDescendantAdded;
        application.DescendantRemoved += OnDescendantRemoved;
    }

    void OnDescendantAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is VisualElement element)
            BindElement(element);
    }

    void OnDescendantRemoved(object? sender, ElementEventArgs e)
    {
        if (e.Element is VisualElement element)
            UnbindElement(element);
    }

    void BindElement(VisualElement element)
    {
        switch (element)
        {
            case Button button:
                button.Clicked += OnButtonClicked;
                break;

            case Entry entry:
                entry.TextChanged += OnEntryTextChanged;
                break;

            case Slider slider:
                slider.ValueChanged += OnSliderValueChanged;
                break;

            case Switch sw:
                sw.Toggled += OnSwitchToggled;
                break;

            case CheckBox cb:
                cb.CheckedChanged += OnCheckBoxCheckedChanged;
                break;

            case DatePicker dp:
                dp.DateSelected += OnDatePickerDateSelected;
                break;

            case TimePicker tp:
                tp.PropertyChanged += OnTimePickerPropertyChanged;
                break;

            case Picker picker:
                picker.SelectedIndexChanged += OnPickerSelectedIndexChanged;
                break;

            case SearchBar searchBar:
                searchBar.SearchButtonPressed += OnSearchBarSearchPressed;
                break;

            case Stepper stepper:
                stepper.ValueChanged += OnStepperValueChanged;
                break;

            case Editor editor:
                editor.TextChanged += OnEditorTextChanged;
                break;

            case RadioButton radio:
                radio.CheckedChanged += OnRadioButtonCheckedChanged;
                break;

            // case CollectionView collectionView:
            //     collectionView.Scrolled += OnCollectionViewScrolled;
            //     break;
        }
    }

    void UnbindElement(VisualElement element)
    {
        switch (element)
        {
            case Button button:
                button.Clicked -= OnButtonClicked;
                break;

            case Entry entry:
                entry.TextChanged -= OnEntryTextChanged;
                break;

            case Slider slider:
                slider.ValueChanged -= OnSliderValueChanged;
                break;

            case Switch sw:
                sw.Toggled -= OnSwitchToggled;
                break;

            case CheckBox cb:
                cb.CheckedChanged -= OnCheckBoxCheckedChanged;
                break;

            case DatePicker dp:
                dp.DateSelected -= OnDatePickerDateSelected;
                break;

            case TimePicker tp:
                tp.PropertyChanged -= OnTimePickerPropertyChanged;
                break;

            case Picker picker:
                picker.SelectedIndexChanged -= OnPickerSelectedIndexChanged;
                break;

            case SearchBar searchBar:
                searchBar.SearchButtonPressed -= OnSearchBarSearchPressed;
                break;

            case Stepper stepper:
                stepper.ValueChanged -= OnStepperValueChanged;
                break;

            case Editor editor:
                editor.TextChanged -= OnEditorTextChanged;
                break;

            case RadioButton radio:
                radio.CheckedChanged -= OnRadioButtonCheckedChanged;
                break;

            // case CollectionView collectionView:
            //     collectionView.Scrolled -= OnCollectionViewScrolled;
            //     break;
        }
    }

    // --- Event handlers (instance methods for proper unsubscription) ---

    void OnButtonClicked(object? sender, EventArgs e)
        => feedback?.OnRequested(sender!, nameof(Button.Clicked), e);

    void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
        => feedback?.OnRequested(sender!, nameof(Entry.TextChanged), e);

    void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
        => feedback?.OnRequested(sender!, nameof(Slider.ValueChanged), e);

    void OnSwitchToggled(object? sender, ToggledEventArgs e)
        => feedback?.OnRequested(sender!, nameof(Switch.Toggled), e);

    void OnCheckBoxCheckedChanged(object? sender, CheckedChangedEventArgs e)
        => feedback?.OnRequested(sender!, nameof(CheckBox.CheckedChanged), e);

    void OnDatePickerDateSelected(object? sender, DateChangedEventArgs e)
        => feedback?.OnRequested(sender!, nameof(DatePicker.DateSelected), e);

    void OnTimePickerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
            feedback?.OnRequested(sender!, "TimeChanged", e);
    }

    void OnPickerSelectedIndexChanged(object? sender, EventArgs e)
        => feedback?.OnRequested(sender!, nameof(Picker.SelectedIndexChanged), e);

    void OnSearchBarSearchPressed(object? sender, EventArgs e)
        => feedback?.OnRequested(sender!, nameof(SearchBar.SearchButtonPressed), e);

    void OnStepperValueChanged(object? sender, ValueChangedEventArgs e)
        => feedback?.OnRequested(sender!, nameof(Stepper.ValueChanged), e);

    void OnEditorTextChanged(object? sender, TextChangedEventArgs e)
        => feedback?.OnRequested(sender!, nameof(Editor.TextChanged), e);

    void OnRadioButtonCheckedChanged(object? sender, CheckedChangedEventArgs e)
        => feedback?.OnRequested(sender!, nameof(RadioButton.CheckedChanged), e);

    // void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
    //     => feedback?.OnRequested(typeof(CollectionView), nameof(CollectionView.Scrolled));
}
