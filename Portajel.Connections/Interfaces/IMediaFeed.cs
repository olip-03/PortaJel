using Portajel.Connections.Enum;
using Portajel.Connections.Services;
using Portajel.Connections.Structs;

namespace Portajel.Connections.Interfaces;

public interface IMediaFeed
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ServerUrl { get; set; }
    public bool IsEnabled { get; set; }
    public FeedViewStyle ViewStyle { get; set; }
    public ConnectorProperties Properties { get; set; }
    public BaseData[] GetFrom(int itemIndex, int amount);
    public int Total();
}