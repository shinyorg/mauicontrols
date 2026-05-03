namespace Sample.Features.Toast;

public partial class ToastPage : ContentPage
{
    public ToastPage(ToastViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
