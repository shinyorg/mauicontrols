namespace Sample.Features.ImageEditor;

public partial class ImageEditorPage : ContentPage
{
    public ImageEditorPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ImageEditorViewModel vm && vm.ImageData == null)
            vm.LoadSampleImageCommand.Execute(null);
    }
}
