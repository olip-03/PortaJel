using Microsoft.Extensions.Logging;
using Portajel.Connections;
using Portajel.Connections.Interfaces;
using Portajel.Connections.Services.Database;
using Portajel.Pages.Settings;
using Portajel.Structures.Functional;
using Portajel.Structures.Interfaces;
using Portajel.Structures.ViewModels.Settings;
using Mapsui;
using CommunityToolkit.Maui;
using Portajel.Pages.Settings.Debug;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Portajel.Pages;
using Portajel.Components;
using Portajel.Structures.ViewModels.Pages.Library;
using Portajel.Pages.Library;
using FFImageLoading.Maui;

namespace Portajel
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
        {
            // Todo: UpdateDb minimum version requirements so this messaage goes away
            builder
                .UseMauiApp<App>()
                .UseFFImageLoading()
                .UseVirtualListView()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .RegisterViewModels()
                .RegisterViews()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                ;

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<HomePage>();
            mauiAppBuilder.Services.AddSingleton<SettingsPage>();
            mauiAppBuilder.Services.AddSingleton<SearchPage>();
            mauiAppBuilder.Services.AddSingleton<MusicListItem>();
            mauiAppBuilder.Services.AddSingleton<ConnectionsPage>();
            mauiAppBuilder.Services.AddSingleton<DebugDatabase>();
            mauiAppBuilder.Services.AddSingleton<AlbumListPage>();
            mauiAppBuilder.Services.AddSingleton<ArtistListPage>();
            mauiAppBuilder.Services.AddSingleton<GenreListPage>();
            mauiAppBuilder.Services.AddSingleton<PlaylistListPage>();
            mauiAppBuilder.Services.AddSingleton<SongListPage>();
            return mauiAppBuilder;
        }
    }
}
