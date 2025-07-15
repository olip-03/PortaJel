namespace Portajel.Connections.Interfaces
{
    public class ConnectorFeeds: Dictionary<string, IMediaFeed>
    {
        public virtual void SetFeedState(string feedId, bool enabled = true)
        {
            if (TryGetValue(feedId, out var feed))
            {
                feed.IsEnabled = enabled;
            }
        }
        public virtual void Refresh()
        {
            
        }
    }
}