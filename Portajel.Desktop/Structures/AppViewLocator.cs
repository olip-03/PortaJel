using System;
using Portajel.Desktop.Pages;
using Portajel.Desktop.Pages.Music;
using Portajel.Desktop.Structures.ViewModel;
using Portajel.Desktop.Structures.ViewModel.Music;
using ReactiveUI;

namespace Portajel.Desktop.Structures;

public class AppViewLocator : ReactiveUI.IViewLocator
{
    private HomeView? _homeView = null;
    private LibraryView? _libraryView = null;
    public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
    {
        HomeViewModel context => _homeView ??= new HomeView() { DataContext = context, ViewModel = context },
        LibraryViewModel context => new LibraryView() { DataContext = context, ViewModel = context },
        AlbumViewModel context => new AlbumView() { DataContext = context, ViewModel = context },
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}