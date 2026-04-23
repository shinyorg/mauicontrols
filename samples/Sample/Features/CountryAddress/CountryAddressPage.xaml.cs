namespace Sample.Features.CountryAddress;

public partial class CountryAddressPage : ContentPage
{
    public CountryAddressPage(CountryAddressViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
