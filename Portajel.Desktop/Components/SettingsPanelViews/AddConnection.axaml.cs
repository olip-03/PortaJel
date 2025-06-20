using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel.Settings;

namespace Portajel.Desktop.Components.SettingsPanelViews;

public partial class AddConnection : ReactiveUserControl<AddConnectionViewModel>
{
    public AddConnection()
    {
        InitializeComponent();
    }
}