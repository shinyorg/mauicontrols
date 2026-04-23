namespace Sample.Features.AutoComplete;

public partial class AutoCompletePage : ContentPage
{
    public AutoCompletePage(AutoCompleteViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
