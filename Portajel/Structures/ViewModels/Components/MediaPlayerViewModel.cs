using Portajel.Connections.Database;

namespace Portajel.Structures.ViewModels.Components;

public class MediaPlayerViewModel
{
    public List<SongData> Queue { get; set; } = new();
}