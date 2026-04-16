namespace Sample.Features.Fab;

public partial class FabPage : ContentPage
{
    public FabPage(FabViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
