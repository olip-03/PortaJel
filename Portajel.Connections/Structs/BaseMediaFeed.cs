    using Portajel.Connections.Enum;
    using Portajel.Connections.Interfaces;
    using Portajel.Connections.Services;

    namespace Portajel.Connections.Structs;

    public class BaseMediaFeed: IMediaFeed
    {
        public BaseMediaFeed()
        {
            
        }
        public BaseMediaFeed(IMediaFeed feed)
        {
            Id = feed.Id;
            Name = feed.Name;
            Description = feed.Description;
            ServerUrl = feed.ServerUrl;
            IsEnabled = feed.IsEnabled;
            Properties = feed.Properties;
        }
        public string Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServerUrl { get; set; }
        public bool IsEnabled { get; set; }
        public FeedViewStyle ViewStyle { get; set; }
        public ConnectorProperties Properties { get; set; }
        public BaseData[] GetFrom(int itemIndex, int amount)
        {
            throw new NotImplementedException();
        }
        public int Total()
        {
            throw new NotImplementedException();
        }
    }