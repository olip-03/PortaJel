using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Portajel.Desktop.Structures.ViewModel;

namespace Portajel.Desktop.Pages;

public partial class LibraryView : ReactiveUserControl<LibraryViewModel>
{
    public LibraryView()
    {
        InitializeComponent();
    }
}