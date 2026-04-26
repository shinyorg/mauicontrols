namespace Shiny.Maui.Controls.SignaturePad;

public sealed class SignatureImageEventArgs : EventArgs
{
    public SignatureImageEventArgs(Stream imageStream)
    {
        ImageStream = imageStream;
    }

    public Stream ImageStream { get; }
}
