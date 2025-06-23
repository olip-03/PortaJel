using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Sdk.Generated.Playlists;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;

namespace Portajel.Desktop.Structures.Services;

public class LibraryViewCache
{
    private List<BaseData> _albums = [];
    private List<BaseData> _artists = [];
    private List<BaseData> _songs = [];
    private List<BaseData> _genres = [];
    private List<BaseData> _playlists = [];

    public bool Initialized(MediaTypes mediaType)
    {
        switch (mediaType)
        {
            case MediaTypes.Album:
                return _albums.Any();
            case MediaTypes.Artist:
                return _artists.Any();
            case MediaTypes.Song:
                return _songs.Any();
            case MediaTypes.Playlist:
                return _playlists.Any();
            case MediaTypes.Genre:
                return _genres.Any();
        }
        return false;
    }
    
    public void StoreMediaType(MediaTypes mediaType, IEnumerable<BaseData> baseData)
    {
        switch (mediaType)
        {
            case MediaTypes.Album:
                _albums = baseData.ToList();
                break;
            case MediaTypes.Artist:
                _artists = baseData.ToList();
                break;
            case MediaTypes.Song:
                _songs = baseData.ToList();
                break;
            case MediaTypes.Playlist:
                _playlists = baseData.ToList();
                break;
            case MediaTypes.Genre:
                _genres = baseData.ToList();
                break;

        }
    }
    
    public IEnumerable<BaseData> GetMediaType(MediaTypes mediaType)
    {
        switch (mediaType)
        {
            case MediaTypes.Album:
                return _albums;
            case MediaTypes.Artist:
                return _artists;
            case MediaTypes.Song:
                return _songs;
            case MediaTypes.Playlist:
                return _playlists;
            case MediaTypes.Genre:
                return _genres;
        }
        return null;
    }
}