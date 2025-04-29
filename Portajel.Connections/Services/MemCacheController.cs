using Portajel.Connections.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Connections.Services
{
    public class MemCacheController
    {
        public Dictionary<Guid, BaseData> Albums = [];
        public Dictionary<Guid, BaseData> Artists = [];
        public Dictionary<Guid, BaseData> Songs = [];
        public Dictionary<Guid, BaseData> Playlists = [];
        public Dictionary<Guid, BaseData> Genres = [];
    }
}
