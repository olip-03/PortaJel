using System;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel;

public class LibraryViewModel: ReactiveObject, IRoutableViewModel
{
    // Reference to IScreen that owns the routable view model.
    public IScreen HostScreen { get; }

    // Unique identifier for the routable view model.
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public LibraryViewModel(IScreen screen) => HostScreen = screen;
}