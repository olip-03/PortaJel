using Portajel.Connections.Database;
using System.Collections.ObjectModel;

namespace Portajel.Structures.ViewModels.Components;

public class MediaPlayerViewModel
{
    public ObservableCollection<SongData> Queue { get; set; } = new();
}