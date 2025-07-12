using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Sdk.Generated.Playlists;
using Portajel.Connections.Enum;
using Portajel.Connections.Structs;
using MediaType = Portajel.Connections.Enum.MediaType;

namespace Portajel.Desktop.Structures.Services;

public class LibraryViewCache
{
    private List<BaseData> _albums = [];
    private List<BaseData> _artists = [];
    private List<BaseData> _songs = [];
    private List<BaseData> _genres = [];
    private List<BaseData> _playlists = [];

    public bool Initialized(MediaType mediaType)
    {
        switch (mediaType)
        {
            case MediaType.Album:
                return _albums.Any();
            case MediaType.Artist:
                return _artists.Any();
            case MediaType.Song:
                return _songs.Any();
            case MediaType.Playlist:
                return _playlists.Any();
            case MediaType.Genre:
                return _genres.Any();
        }
        return false;
    }
    
    public void StoreMediaType(MediaType mediaType, IEnumerable<BaseData> baseData)
    {
        switch (mediaType)
        {
            case MediaType.Album:
                _albums = baseData.ToList();
                break;
            case MediaType.Artist:
                _artists = baseData.ToList();
                break;
            case MediaType.Song:
                _songs = baseData.ToList();
                break;
            case MediaType.Playlist:
                _playlists = baseData.ToList();
                break;
            case MediaType.Genre:
                _genres = baseData.ToList();
                break;

        }
    }
    
    public IEnumerable<BaseData> GetMediaType(MediaType mediaType)
    {
        switch (mediaType)
        {
            case MediaType.Album:
                return _albums;
            case MediaType.Artist:
                return _artists;
            case MediaType.Song:
                return _songs;
            case MediaType.Playlist:
                return _playlists;
            case MediaType.Genre:
                return _genres;
        }
        return null;
    }
}