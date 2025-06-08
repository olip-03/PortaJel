using System;
using Portajel.Desktop.Pages;
using Portajel.Desktop.Structures.ViewModel;
using ReactiveUI;

namespace Portajel.Desktop.Structures;

public class AppViewLocator : ReactiveUI.IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string contract = null) => viewModel switch
    {
        HomeViewModel context => new HomeView() { DataContext = context },
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}