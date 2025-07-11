using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel;
using ReactiveUI;

namespace Portajel.Desktop.Pages;

public partial class SearchView: ReactiveUserControl<SearchViewModel>
{
    public SearchView()
    {
        this.WhenActivated(disposables =>
        {

        });
        AvaloniaXamlLoader.Load(this);
    }
}