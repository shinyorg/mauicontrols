using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.TextToSpeech;

public partial class TextToSpeechViewModel : ObservableObject
{
    [ObservableProperty]
    string customText = "Hello from Shiny Controls!";

    [ObservableProperty]
    string statusMessage = "Tap a button to hear it speak";

    [ObservableProperty]
    float pitch = 1.0f;

    [ObservableProperty]
    float volume = 1.0f;
}
