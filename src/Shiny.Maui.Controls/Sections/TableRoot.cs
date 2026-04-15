using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using TvTableView = Shiny.Maui.Controls.TableView;

namespace Shiny.Maui.Controls.Sections;

[ContentProperty(nameof(Sections))]
public class TableRoot : BindableObject
{
    readonly ObservableCollection<TableSection> sections = new();

    public TableRoot()
    {
        sections.CollectionChanged += OnSectionsCollectionChanged;
    }

    public ObservableCollection<TableSection> Sections => sections;

    public event EventHandler? RootChanged;

    void OnSectionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (TableSection section in e.OldItems)
            {
                section.SectionChanged -= OnSectionChanged;
                section.ParentTableView = null;
            }
        }

        if (e.NewItems != null)
        {
            foreach (TableSection section in e.NewItems)
            {
                section.SectionChanged += OnSectionChanged;
                section.ParentTableView = ParentTableView;
            }
        }

        RaiseRootChanged();
    }

    void OnSectionChanged(object? sender, EventArgs e)
    {
        RaiseRootChanged();
    }

    void RaiseRootChanged()
    {
        RootChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        foreach (var section in sections)
            SetInheritedBindingContext(section, BindingContext);
    }

    internal TvTableView? ParentTableView { get; set; }

    internal void SetParentTableView(TvTableView? tableView)
    {
        ParentTableView = tableView;
        foreach (var section in sections)
            section.ParentTableView = tableView;
    }
}