using Portajel.Connections.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Connections.Structs.Sync
{
    public struct ServerRequestInfo
    {
        public CancellationToken? CancellationToken = null;
        public MediaType MediaType;
        public int StartFrom;
        public int BatchSize;
        public ServerRequestInfo() { }
    }
}
