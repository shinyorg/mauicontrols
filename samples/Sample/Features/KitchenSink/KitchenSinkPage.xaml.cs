namespace Sample.Features.KitchenSink;

public partial class KitchenSinkPage : ContentPage
{
    public KitchenSinkPage(KitchenSinkViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
