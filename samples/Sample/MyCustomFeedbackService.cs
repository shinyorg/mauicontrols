using Plugin.Maui.Audio;
using Shiny.Maui.Controls;
using Shiny.Maui.Controls.Chat;
using Shiny.Maui.Controls.Infrastructure;
using Shiny.Speech;

namespace Sample;

// we take in the haptic feedback service because we still like it as a base
public class MyCustomFeedbackService(
    AppSettings appSettings,
    ITextToSpeechService textToSpeech,
    IAudioManager audioManager
) : HapticFeedbackService
{
    public override async void OnRequested(object control, string eventName, object? args)
    {
        // let haptic do its thing first
        base.OnRequested(control, eventName, args);

        if (control is ChatView && appSettings.IsChatSpeakingEnabled && args is ChatMessage { IsFromMe: false } msg)
        {
            var say = $"Message from {msg.SenderId}. {msg.Text}";
            await textToSpeech.SpeakAsync(say);
        }
        else if (control is SecurityPin && appSettings.IsSecurityBeepEnabled)
        {
            var sound = eventName.Equals("completed", StringComparison.OrdinalIgnoreCase)
                ? "pin_success.wav"
                : "pin_click.wav";

            var raw = await FileSystem.OpenAppPackageFileAsync(sound);
            var player = audioManager.CreatePlayer(raw);
            player.Play();
        }
    }
}