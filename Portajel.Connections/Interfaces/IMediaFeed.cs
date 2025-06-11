using Portajel.Connections.Structs;

namespace Portajel.Connections.Interfaces;

public interface IMediaFeed
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string ServerUrl { get; }
    public bool IsEnabled { get; set; }
    public BaseData[] GetFrom(int itemIndex, int amount);
    public int Total();
}