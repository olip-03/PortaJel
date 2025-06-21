using System;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Portajel.Desktop.Structures.ViewModel;

namespace Portajel.Desktop.Components;

public partial class SettingsPanel : ReactiveUserControl<SettingsPanelViewModel>
{
    private bool _isVisible = false;
    
    public SettingsPanel()
    {
        InitializeComponent();
    }
    
    public bool IsVisible => _isVisible;
}