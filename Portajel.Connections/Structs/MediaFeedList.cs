using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;

namespace Portajel.Connections.Structs;

/// <summary>
/// List containing all feeds the server supports
/// </summary>
public class MediaFeedList: List<IMediaFeed>
{
    public MediaFeedList()
    {
        
    }

    public IEnumerable<IMediaFeed> GetEnabled()
    {
        return [];
    }
}