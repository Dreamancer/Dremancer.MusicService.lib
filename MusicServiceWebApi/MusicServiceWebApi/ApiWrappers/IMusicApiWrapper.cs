using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.ApiWrappers
{
    interface IMusicApiWrapper
    {
    Task<ApiResponse> SearchArtists(string name);

    Task<ApiResponse> GetArtistInfo(string id);

    Task<ApiResponse> GetArtistAlbums(string id);

    Task<ApiResponse> GetAlbumInfo(string id);

    //ApiResponse SearchSongs(string name);

    Task<ApiResponse> GetSongInfo(string id);

    Task<ApiResponse> GetArtistEvents(string artistId);
    }
}
