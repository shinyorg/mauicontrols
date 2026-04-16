using Shiny.Maui.Controls;

namespace Sample.Features.SecurityPin;

public partial class SecurityPinPage : ContentPage
{
    public SecurityPinPage(SecurityPinViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    void OnHiddenPinCompleted(object? sender, SecurityPinCompletedEventArgs e)
        => ((SecurityPinViewModel)BindingContext).OnHiddenPinCompleted(e.Value);

    void OnVisiblePinCompleted(object? sender, SecurityPinCompletedEventArgs e)
        => ((SecurityPinViewModel)BindingContext).OnVisiblePinCompleted(e.Value);

    void OnAlphaPinCompleted(object? sender, SecurityPinCompletedEventArgs e)
        => ((SecurityPinViewModel)BindingContext).OnAlphaPinCompleted(e.Value);
}
