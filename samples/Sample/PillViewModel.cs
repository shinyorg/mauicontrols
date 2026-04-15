using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample;

public partial class PillViewModel : ObservableObject
{
    [ObservableProperty]
    string customPillText = "Custom";
}