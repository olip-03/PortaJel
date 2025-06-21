using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel;

namespace Portajel.Desktop.Components.SettingsPanelViews;

public partial class SettingsConnections : ReactiveUserControl<SettingsConnectionViewModel>
{
    public SettingsConnections()
    {
        InitializeComponent();
    }
}