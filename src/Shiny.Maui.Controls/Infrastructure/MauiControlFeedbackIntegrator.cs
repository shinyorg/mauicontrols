using Shiny.Blazor.Controls;

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
        => feedback?.OnRequested(typeof(Button), nameof(Button.Clicked));

    void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
        => feedback?.OnRequested(typeof(Entry), nameof(Entry.TextChanged), e.NewTextValue);

    void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
        => feedback?.OnRequested(typeof(Slider), nameof(Slider.ValueChanged), e.NewValue.ToString("F2"));

    void OnSwitchToggled(object? sender, ToggledEventArgs e)
        => feedback?.OnRequested(typeof(Switch), nameof(Switch.Toggled), e.Value.ToString());

    void OnCheckBoxCheckedChanged(object? sender, CheckedChangedEventArgs e)
        => feedback?.OnRequested(typeof(CheckBox), nameof(CheckBox.CheckedChanged), e.Value.ToString());

    void OnDatePickerDateSelected(object? sender, DateChangedEventArgs e)
        => feedback?.OnRequested(typeof(DatePicker), nameof(DatePicker.DateSelected), e.NewDate.ToString());

    void OnTimePickerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
            feedback?.OnRequested(typeof(TimePicker), "TimeChanged");
    }

    void OnPickerSelectedIndexChanged(object? sender, EventArgs e)
        => feedback?.OnRequested(typeof(Picker), nameof(Picker.SelectedIndexChanged));

    void OnSearchBarSearchPressed(object? sender, EventArgs e)
        => feedback?.OnRequested(typeof(SearchBar), nameof(SearchBar.SearchButtonPressed));

    void OnStepperValueChanged(object? sender, ValueChangedEventArgs e)
        => feedback?.OnRequested(typeof(Stepper), nameof(Stepper.ValueChanged), e.NewValue.ToString("F2"));

    void OnEditorTextChanged(object? sender, TextChangedEventArgs e)
        => feedback?.OnRequested(typeof(Editor), nameof(Editor.TextChanged), e.NewTextValue);

    void OnRadioButtonCheckedChanged(object? sender, CheckedChangedEventArgs e)
        => feedback?.OnRequested(typeof(RadioButton), nameof(RadioButton.CheckedChanged), e.Value.ToString());

    // void OnCollectionViewScrolled(object? sender, ItemsViewScrolledEventArgs e)
    //     => feedback?.OnRequested(typeof(CollectionView), nameof(CollectionView.Scrolled));
}
