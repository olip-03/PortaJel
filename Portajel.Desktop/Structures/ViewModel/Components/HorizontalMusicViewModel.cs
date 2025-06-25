using System.Collections.ObjectModel;
using Portajel.Connections.Database;
using Portajel.Connections.Structs;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel.Components;

public class HorizontalMusicViewModel: ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment { get; }
    public IScreen HostScreen { get; }
    public string Title { get; set; } = "";
    public ObservableCollection<BaseData> Songs { get; set; } = new();
}