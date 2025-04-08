using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using AndroidX.Startup;
using Java.IO;
using Java.Nio.Channels;
using Jellyfin.Sdk.Generated.Models;
using Microsoft.Extensions.DependencyInjection;
using Portajel.Connections;
using Portajel.Connections.Data;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Database;
using Portajel.Droid.Services;
using Portajel.Structures.Functional;
using PortaJel.Droid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

namespace Portajel.Services
{
    public class DroidServerConnector : IServerConnector
    {
        private ServiceCollection _serviceConnection = null!;
        public DroidServerConnector(DroidServiceController serverConnectior)
        {
            _serviceConnection = serverConnectior.AppServiceConnection;
        }
        public List<IMediaServerConnector> Servers
        {
            get
            {
                if (_serviceConnection.Binder == null)
                    throw GetNullReferenceException();
                return _serviceConnection.Binder.Server.Servers;
            }
        }
        public Dictionary<string, ConnectorProperty> Properties
        {
            get
            {
                if (_serviceConnection.Binder == null)
                    throw GetNullReferenceException();

                return _serviceConnection.Binder.Server.Properties;
            }
        }
        public List<Action<IMediaServerConnector>> AddServerActions
        {
            get
            {
                if (_serviceConnection.Binder == null)
                    throw GetNullReferenceException();
                return _serviceConnection.Binder.Server.AddServerActions;
            }
            set
            {
                if (_serviceConnection.Binder == null)
                    throw GetNullReferenceException();
                _serviceConnection.Binder.Server.AddServerActions = value;
            }
        }
        private NullReferenceException GetNullReferenceException()
        {
            return new NullReferenceException("Service not initalized! Check back later.");
        }
        public async Task<AuthResponse> AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            return await _serviceConnection.Binder.Server.AuthenticateAsync();
        }
        public ServerConnectorSettings GetSettings()
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            return _serviceConnection.Binder.Server.GetSettings();
        }
        public async Task<BaseMusicItem[]> SearchAsync(string searchTerm = "", int? limit = null, int startIndex = 0, ItemSortBy setSortTypes = ItemSortBy.Name, SortOrder setSortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            return await _serviceConnection.Binder.Server.SearchAsync(
                searchTerm, 
                limit, 
                startIndex, 
                setSortTypes, 
                setSortOrder, 
                cancellationToken);
        }
        public async Task<bool> StartSyncAsync(CancellationToken cancellationToken = default)
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            return await _serviceConnection.Binder.Server.StartSyncAsync(cancellationToken);
        }
        public void AddServer(IMediaServerConnector server)
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            _serviceConnection.Binder.Server.AddServer(server);
        }
        public void RemoveServer(IMediaServerConnector server)
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            _serviceConnection.Binder.Server.RemoveServer(server);
        }
        public void RemoveServer(string address)
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            _serviceConnection.Binder.Server.RemoveServer(Servers.First(s => s.GetAddress() == address));

        }
        public IMediaServerConnector[] GetServers()
        {
            if (_serviceConnection.Binder == null)
                throw GetNullReferenceException();
            return _serviceConnection.Binder.Server.GetServers();
        }
    }
}
