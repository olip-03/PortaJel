using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CommunityToolkit.Mvvm.DependencyInjection;
using DynamicData;
using Portajel.Connections;
using Portajel.Connections.Database;
using Portajel.Connections.Interfaces;
using Portajel.Desktop.Structures.ViewModel;
using Portajel.Desktop.Structures.ViewModel.Components;
using Portajel.Desktop.Structures.ViewModel.Music;
using ReactiveUI;

namespace Portajel.Desktop.Pages.Music;

public partial class AlbumView: ReactiveUserControl<AlbumViewModel>
{
    private IDbConnector _database = Ioc.Default.GetService<IDbConnector>();
    private ServerConnector _server = Ioc.Default.GetService<ServerConnector>();
    
    public AlbumView()
    {
        this.WhenActivated(disposables =>
        {
            Load();
        });
        AvaloniaXamlLoader.Load(this);
    }

    public async void Load()
    {
        var songIds = ViewModel.Album.GetSongIds().Select(i => (Guid?)i).ToArray();
        var similarIds = ViewModel.Album.GetSongIds().Select(i => (Guid?)i).ToArray();
        var songs = _database
            .Connectors.Song
            .GetAll(includeIds: songIds)
            .Select(s => s.ToSong())
            .OrderBy(s => s.IndexNumber)
            .ThenBy(s => s.DiskNumber)
            .ToList();
        var similarSongs = _database
            .Connectors.Album
            .GetAll(includeIds: similarIds)
            .Select(s => s.ToAlbum())
            .ToList();
        ViewModel.Songs.AddRange(songs);
        
        ViewModel.HorizontalMusicViewModel.Songs.Clear();
        ViewModel.HorizontalMusicViewModel.Songs.AddRange(similarSongs);
        
        // do server fetch
        var srvLatest = _server.Servers[ViewModel.Album.ServerAddress].DataConnectors["Album"]
            .GetAsync(ViewModel.Album.ServerId, ViewModel.Album.ServerAddress);
        var similar = _server.Servers[ViewModel.Album.ServerAddress].DataConnectors["Album"]
            .GetSimilarAsync(ViewModel.Album.ServerId, 50, ViewModel.Album.ServerAddress);
        await Task.WhenAll(srvLatest, similar);
        if (srvLatest.Result is AlbumData albumLatest)
        {
            ViewModel.Album = albumLatest;
        }
        if (similar.Result.Any())
        {
            var albums = similar.Result.Select(s => s.ToAlbum());
            // ViewModel.Similar.AddRange(albums); 
            ViewModel.HorizontalMusicViewModel.Songs.Clear();
            ViewModel.HorizontalMusicViewModel.Songs.AddRange(albums);
        }
    }
}