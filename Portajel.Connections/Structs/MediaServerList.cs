using Portajel.Connections.Interfaces;

namespace Portajel.Connections.Structs;

public class MediaServerList: List<IMediaServerConnector>
{
    public MediaServerList()
    {
        
    }
    
    public IMediaServerConnector this[string name] => Find(s => s.GetAddress() == name);
    
    public void RemoveServer(IMediaServerConnector server)
    {
        this.Remove(server);
    }

    public void RemoveServer(string address)
    {
        var srv = this.First(s => s.GetAddress() == address);
        this.Remove(srv);
    }
}