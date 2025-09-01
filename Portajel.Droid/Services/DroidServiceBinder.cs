using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Jellyfin.Sdk.Generated.Models;
using Portajel.Connections.Data;
using Portajel.Connections.Enum;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services;
using Portajel.Connections.Services.Database;
using Portajel.Connections;
using Microsoft.Maui.Controls;
using PortaJel.Droid.Services;
using Portajel.Services;
using Portajel.Structures.Interfaces;

namespace Portajel.Droid.Services
{
    public class DroidServiceBinder : Binder, IDisposable
    {
        public IMediaController MediaController { get; private set; } = null!;
        public DatabaseConnector Database { get; private set; } = null!;
        public ServerConnector Server { get; private set; } = null!;
        public DroidService Service { get; private set; } = null!;
        public DroidServiceBinder(DroidService service)
        {
            Service = service;
            Server = Service.serverConnector;
            Database = Service.database;
            MediaController = service.MediaController;
        }
        public void Destroy()
        {

        }
    }
}
