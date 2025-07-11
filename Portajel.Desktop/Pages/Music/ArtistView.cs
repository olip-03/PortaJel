using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel.Music;
using ReactiveUI;

namespace Portajel.Desktop.Pages.Music;

public partial class ArtistView: ReactiveUserControl<ArtistViewModel>
{
    public ArtistView()
    {
        this.WhenActivated(disposables =>
        {
            
        });
        AvaloniaXamlLoader.Load(this);
    }
}