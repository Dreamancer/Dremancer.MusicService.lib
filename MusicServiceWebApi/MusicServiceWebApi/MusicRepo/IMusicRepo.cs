using MusicServiceWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.MusicRepo
{
    interface IMusicRepo
    {
        IEnumerable<Artist> SearchArtists(string name);

        Artist GetArtist(string spotifyId);

        Artist GetArtist(Guid mbId);

        IEnumerable<Album> GetArtistAlbums(string id);

        Album GetAlbum(string id);

        Song GetSong(string id);

        IEnumerable<Event> GetArtistEvents(string artistId);

        void ClearTempContext(string tableName);
    }
}
