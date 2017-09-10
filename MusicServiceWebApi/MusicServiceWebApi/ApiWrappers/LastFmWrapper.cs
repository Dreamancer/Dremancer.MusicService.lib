using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MusicServiceWebApi.ApiWrappers
{
    public class LastFmWrapper : MusicApiWrapper
    {
        private static readonly string _apiName = "LastFM";
        public override string ApiName { get { return _apiName; } }

        public LastFmWrapper(IConfiguration configuration) : base(configuration)
        {

        }

        public override async Task<ApiResponse> GetAlbumInfo(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}?method=album.getinfo&mbid={1}&api_key={2}&format=json", _url, id, _appId));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Album,
                Response = responseJson
            };
            return res;
        }

        public override async Task<ApiResponse> GetArtistAlbums(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}?method=artist.gettopalbums&mbid={1}&api_key={2}&format=json", _url, id, _appId));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Album,
                Response = responseJson
            };
            return res;
        }

        public override Task<ApiResponse> GetArtistEvents(string artistId)
        {
            throw new NotImplementedException();
        }

        public override async Task<ApiResponse> GetArtistInfo(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}?method=artist.getinfo&mbid={1}&api_key={2}&format=json", _url, id, _appId));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Artist,
                Response = responseJson
            };
            return res;
        }

        public override async Task<ApiResponse> GetSongInfo(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}?method=track.getinfo&mbid={1}&api_key={2}&format=json", _url, id, _appId));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Song,
                Response = responseJson
            };
            return res;
        }

        public override async Task<ApiResponse> SearchArtists(string name)
        {
            JObject responseJson = await GetResponse(string.Format("{0}?method=artist.search&artist={1}&api_key={2}&format=json", _url, name, _appId));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Artist,
                Response = responseJson
            };
            return res;
        }
    }
}
