using System.Linq; // Make sure to include this namespace
using Portajel.Connections.Interfaces;

namespace Portajel.Connections.Structs
{
    public class MediaServerList : List<IMediaServerConnector>
    {
        public MediaServerList()
        {
        }
        public IMediaServerConnector? this[string name] => this.FirstOrDefault(s => s.GetAddress() == name);
        public void RemoveServer(IMediaServerConnector server)
        {
            this.Remove(server);
        }
        public void RemoveServer(string address)
        {
            var srv = this.FirstOrDefault(s => s.GetAddress() == address);
            if (srv != null)
            {
                this.Remove(srv);
            }
        }
    }
}