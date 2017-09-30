using MusicWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.MusicRepo
{
    interface IWebRepo
    {
        IEnumerable<Artist> SearchArtists(string name);

        Artist GetArtist(string spotifyId);

        Artist GetArtist(Guid mbId);

        IEnumerable<Album> GetArtistAlbums(int id);

        Album GetAlbum(string spotifyId);

        Album GetAlbum(Guid mbId);

        Song GetSong(string id);

        IEnumerable<Event> GetArtistEvents(string artistId);

        void ClearTempContext(string tableName);
    }
}
