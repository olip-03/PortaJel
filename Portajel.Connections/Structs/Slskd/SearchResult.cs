using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Connections.Structs.Slskd
{
    public class SearchResult
    {
        public string endedAt { get; set; }
        public int fileCount { get; set; }
        public string id { get; set; }
        public bool isComplete { get; set; }
        public int lockedFileCount { get; set; }
        public int responseCount { get; set; }
        public List<object> responses { get; set; }
        public string searchText { get; set; }
        public string startedAt { get; set; }
        public string state { get; set; }
        public int token { get; set; }
    }
}
