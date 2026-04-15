using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.Pills;

public partial class PillViewModel : ObservableObject
{
    [ObservableProperty]
    string customPillText = "Custom";
}
