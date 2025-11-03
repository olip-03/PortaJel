using Portajel.Connections.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Portajel.Connections.Services.Sync
{
    public class ServerRequestInfo
    {
        [JsonIgnore]
        public CancellationToken? CancellationToken { get; set; }

        public MediaType MediaType { get; set; }
        public string ServerAddress { get; set; } = "";
        public int StartFrom { get; set; }
        public int BatchSize { get; set; }

        public ServerRequestInfo() { }
    }

    public class SyncStatusInfo
    {
        /// <summary>
        /// The state of this task.
        /// </summary>
        public TaskStatus TaskStatus { get; set; } = TaskStatus.WaitingToRun;
        /// <summary>
        /// The overall percentage of this sync.
        /// </summary>
        public int StatusPercentage { get; set; } = 0;
        /// <summary>
        /// The amount of items for this type that exist on the server.
        /// </summary>
        public int ServerItemTotal { get; set; } = 0;
        /// <summary>
        /// The amount of items for this type that we're counting, or that we've got in memory.
        /// </summary>
        public int ServerItemCount { get; set; } = 0;
        /// <summary>
        /// The total number of this item found in the database.
        /// </summary>
        public int DbFoundTotal { get; set; } = 0;
        public SyncStatusInfo() { }
    }
}
