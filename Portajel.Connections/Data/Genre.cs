﻿using Portajel.Connections.Database;

namespace Portajel.Connections.Data
{
    public class Genre : BaseMusicItem
    {
        public GenreData GetBase => _genreData;
        // public new Guid Id => _genreData.Id;
        public override Guid Id => _genreData.Id;
        public string ServerAddress => _genreData.ServerAddress;
        public override string Name => _genreData.Name;
        // public override DateTimeOffset DateAdded => _genreData.DateAdded;
        public Guid[] AlbumIds => _genreData.GetAlbumIds();
        private readonly GenreData _genreData ;

        public Genre()
        {
            
        }
        public Genre(GenreData genreData)
        {
            _genreData = genreData;
        }
    }
}
