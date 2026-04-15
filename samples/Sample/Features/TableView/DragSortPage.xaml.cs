using System.ComponentModel;
using System.Windows.Input;
using Shiny.Maui.Controls;

namespace Sample.Features.TableView;

public partial class DragSortPage : ContentPage
{
    public DragSortPage()
    {
        InitializeComponent();
    }
}

public class DragSortViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand ItemDroppedCommand { get; } = new Command<ItemDroppedEventArgs>(args =>
    {
        System.Diagnostics.Debug.WriteLine($"Moved item from index {args.FromIndex} to {args.ToIndex}");
    });
}
