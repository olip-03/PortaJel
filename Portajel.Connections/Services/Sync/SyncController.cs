using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Structs;
using Portajel.Connections.Structs.Sync;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLite.SQLite3;

namespace Portajel.Connections.Services.Sync
{

    /// <summary>
    /// Service Class that handles Media Server data sync operations. 
    /// </summary>
    public class SyncController
    {
        /// <summary>
        /// Called when the status of a server updates.
        /// </summary>
        public EventHandler? OnStatusChange;

        private const int BatchSize = 10;
        private const int MaxParallel = 500;

        private IDbConnector _database;
        private Dictionary<Guid, CancellationTokenSource> _requestTokens = new();

        private Queue<ServerRequestInfo> serverRequests = new();

        public SyncController(IDbConnector database) 
        {
            _database = database;
        }

        /// <summary>
        /// Starts the sync service for select servers. 
        /// </summary>
        /// <param name="servers">Declare which Servers should begin syncing.</param>
        public async Task Start(IEnumerable<IMediaServerConnector>? servers = null)
        {
            var tasks = servers?.Select(s => Task.Run(() => Start(s)));
            if(tasks?.Any() == true)
            {
                await Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// Starts the sync service for one select server. 
        /// </summary>
        /// <param name="server">Declare which Server should begin syncing.</param>
        public async Task Start(IMediaServerConnector server)
        {
            // Get all counts
            var totalCount = server.DataConnectors.Select(s => Task.Run(() => SyncControlHelper.GetServerTotalCount(s.Key, s.Value)));
            await Task.WhenAll(totalCount);
            var result = totalCount.Select(t => t.Result).Where(r => r.Total > 0).ToList();

            if (result.Count == 0) return;
            CreateRequests(result);

            // By this point all serverRequests should exist. We should
            // return early to allow the application to get on with whatever it was doing
            // and run the actual sync in the background someplace else
        }

        /// <summary>
        /// Stops the sync service in place for select server.
        /// </summary>
        /// <param name="server">Declare which server sync should be paused.</param>
        public void Stop(IMediaServerConnector server)
        {
            Stop([server]);
        }

        /// <summary>
        /// Stops the sync service in place for all, or select severs.
        /// </summary>
        /// <param name="servers">
        /// Optional paramater to declare which servers sync should be paused. 
        /// If blank all servers will be stopped
        /// </param>
        public void Stop(IEnumerable<IMediaServerConnector>? servers = null) 
        {
            
        }
          
        private void CreateRequests(List<(MediaType MediaType, int Total)> result)
        {
            // Calculate batch counts and track current position for each type
            var typeBatchCounts = result.Select(r => (r.Total + BatchSize - 1) / BatchSize).ToArray();
            var currentBatchIndex = new long[result.Count];

            bool hasWork = true;
            while (hasWork)
            {
                hasWork = false;

                for (int typeIndex = 0; typeIndex < result.Count; typeIndex++)
                {
                    if (currentBatchIndex[typeIndex] < typeBatchCounts[typeIndex])
                    {
                        long start = currentBatchIndex[typeIndex] * BatchSize;
                        int actualBatchSize = (int)Math.Min(BatchSize, result[typeIndex].Total - start);

                        serverRequests.Enqueue(new ServerRequestInfo
                        {
                            MediaType = result[typeIndex].MediaType,
                            StartFrom = (int)start,
                            BatchSize = actualBatchSize
                        });

                        currentBatchIndex[typeIndex]++;
                        hasWork = true;
                    }
                }
            }
        }
    }
}
