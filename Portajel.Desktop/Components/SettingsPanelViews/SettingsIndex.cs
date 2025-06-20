using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel;

namespace Portajel.Desktop.Components;

public partial class SettingsIndex : ReactiveUserControl<SettingsPanelViewModel>
{
    public SettingsIndex()
    {
        InitializeComponent();
        
        // Handle ListBox selection
        var ctrl = this.FindControl<ListBox>("SettingsList");
        if (ctrl != null)
        {
            ctrl.SelectionChanged += OnSelectionChanged;
        }
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0 && 
            e.AddedItems[0] is SettingsCategory category)
        {
            ViewModel?.NavigateToCategory(category.Name);
        }
    }
}