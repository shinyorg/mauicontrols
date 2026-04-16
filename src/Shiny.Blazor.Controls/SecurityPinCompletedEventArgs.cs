namespace Shiny.Blazor.Controls;

public class SecurityPinCompletedEventArgs(string value) : EventArgs
{
    public string Value { get; } = value;
}
