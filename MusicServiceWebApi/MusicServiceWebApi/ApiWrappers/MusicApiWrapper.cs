using Microsoft.Extensions.Configuration;
using MusicWebApi.ApiWrappers.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MusicWebApi.ApiWrappers
{
    public abstract class MusicApiWrapper : IMusicApiWrapper
    {
        protected readonly string _url;
        protected readonly string _appId;
        protected readonly string _appSecret;
        public abstract ApiName ApiName { get; }

        public MusicApiWrapper(IConfiguration configuration)
        {
            _url = configuration[string.Format("Apis:{0}:Url", ApiName)];
            _appId = configuration[string.Format("Apis:{0}:ClientID", ApiName)];
            _appSecret = configuration[string.Format("Apis:{0}:ClientSecret", ApiName)];
        }

        public MusicApiWrapper()
        {

        }

        public abstract Task<ApiResponse> GetAlbumInfo(string id);
        public abstract Task<ApiResponse> GetArtistAlbums(string id);
        public abstract Task<ApiResponse> GetArtistEvents(string artistId);
        public abstract Task<ApiResponse> GetArtistInfo(string id);
        public abstract Task<ApiResponse> GetSongInfo(string id);
        public abstract Task<ApiResponse> SearchArtists(string name);

        protected virtual async Task<JObject> GetResponse(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(url);
                string responseString = await response.Content.ReadAsStringAsync();

                try
                {
                    return JObject.Parse(responseString);
                }
                catch (JsonReaderException ex)
                {
                    //log
                    return null;
                }
            }
        }
    }
}
