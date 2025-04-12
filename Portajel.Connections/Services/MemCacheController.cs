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
        public Dictionary<Guid, BaseMusicItem> Albums = [];
        public Dictionary<Guid, BaseMusicItem> Artists = [];
        public Dictionary<Guid, BaseMusicItem> Songs = [];
        public Dictionary<Guid, BaseMusicItem> Playlists = [];
        public Dictionary<Guid, BaseMusicItem> Genres = [];
    }
}
