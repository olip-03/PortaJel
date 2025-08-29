using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Embedding;
using Portajel.Components.FeedView;      // your HorizontalFeedView
using Portajel.Connections.Interfaces;   // IServerConnector, IDbConnector

namespace Portajel.Components
{
    public class FeedGridView : Grid
    {
        // grab MAUI DI container
        internal static IServiceProvider? ServiceProvider { get; set; }
            = IPlatformApplication.Current?.Services;

        readonly IServerConnector _server;
        readonly IDbConnector     _database;

        public FeedGridView()
            : this(
                ServiceProvider?.GetService<IServerConnector>()!,
                ServiceProvider?.GetService<IDbConnector>()!
            )
        {
        }

        public FeedGridView(IServerConnector server, IDbConnector database)
        {
            _server   = server;
            _database = database;

            // initial build
            _ = BuildUIAsync();

            // rebuild on theme change
            Application.Current.RequestedThemeChanged += (s, a) =>
                _ = BuildUIAsync();
        }

        /// <summary>
        /// Manually refresh/rebuild the list.
        /// </summary>
        public void RefreshFeeds() =>
            _ = BuildUIAsync();

        public async Task BuildUIAsync()
        {
            // wait until connector is ready
            while (_server.Feeds == null)
                await Task.Delay(250);

            // force a refresh
            _server.Feeds.Refresh();
            var feeds = _server.Feeds;

            // clear out old rows & children
            Children.Clear();
            RowDefinitions.Clear();

            // re-populate
            for (int i = 0; i < feeds.Count; i++)
            {
                var kv = feeds.ElementAt(i);
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                if (kv.Value.IsEnabled)
                {
                    var view = new HorizontalFeedView(kv.Value);
                    Grid.SetRow(view, i);
                    Children.Add(view);
                }
            }
        }
    }
}