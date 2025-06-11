namespace Portajel.Connections.Interfaces
{
    public interface IFeedConnector
    {
        Dictionary<string, IMediaFeed> AvailableFeeds { get; }
        void SetFeedState(string feedId, bool enabled = true);
    }
}