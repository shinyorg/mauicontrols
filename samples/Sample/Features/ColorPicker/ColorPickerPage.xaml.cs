namespace Sample.Features.ColorPicker;

public partial class ColorPickerPage : ContentPage
{
    public ColorPickerPage(ColorPickerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
