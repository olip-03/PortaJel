using Portajel.Connections.Interfaces;

namespace Portajel.Connections;

public class ServerFeedConnector: IFeedConnector
{
    public Dictionary<string, IMediaFeed> AvailableFeeds
    {
        get
        {
            return new Dictionary<string, IMediaFeed>();
        }
    }

    public void SetFeedState(string feedId, bool enabled = true)
    {
        throw new NotImplementedException();
    }
}