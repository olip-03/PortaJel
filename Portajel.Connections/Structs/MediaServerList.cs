using Portajel.Connections.Interfaces;

namespace Portajel.Connections.Structs;

public class MediaServerList: List<IMediaServerConnector>
{
    public MediaServerList()
    {
        
    }
    
    public void RemoveServer(IMediaServerConnector server)
    {
        
    }

    public void RemoveServer(string address)
    {
        
    }
}