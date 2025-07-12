using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using Portajel.Connections.Database;
using Portajel.Connections.Structs;
using ReactiveUI;

namespace Portajel.Desktop.Structures.ViewModel.Components;

public class HorizontalMusicViewModel: ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment { get; }
    public IScreen HostScreen { get; }
    public string Title { get; set; } = "";
    public ObservableCollection<BaseData> MusicData { get; set; } = new();
    private BaseData _selectedBase;
    public BaseData Selected
    {
        get => _selectedBase;
        set
        {
            if (value is AlbumData)
            {
                Trace.WriteLine("Selected album");
            }
            this.RaiseAndSetIfChanged(ref _selectedBase, value);
        } 
    }
    public ReactiveCommand<Unit, IRoutableViewModel> GoAlbum { get; }
}