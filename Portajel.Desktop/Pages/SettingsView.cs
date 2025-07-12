using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel;
using ReactiveUI;

namespace Portajel.Desktop.Pages;

public partial class SettingsView: ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        this.WhenActivated(disposables =>
        {
            
        });
        AvaloniaXamlLoader.Load(this);
    }
}