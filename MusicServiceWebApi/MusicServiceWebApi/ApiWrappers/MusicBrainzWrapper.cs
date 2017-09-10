using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MusicServiceWebApi.ApiWrappers
{
    public class MusicBrainzWrapper : MusicApiWrapper
    {
        private static readonly string _apiName = "MusicBrainz";
        public override string ApiName { get { return _apiName; } }

        public MusicBrainzWrapper(IConfiguration configuration) : base(configuration)
        {

        }

        public override async Task<ApiResponse> GetAlbumInfo(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}release-group/{1}?inc=artists", _url, id));
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
            JObject responseJson = await GetResponse(string.Format("{0}release-group?artist={1}", _url, id));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Album,
                Response = responseJson
            };
            return res;
        }

        public override async Task<ApiResponse> GetArtistEvents(string artistId)
        {
            JObject responseJson = await GetResponse(string.Format("{0}event??artist={1}", _url, artistId));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Event,
                Response = responseJson
            };
            return res;
        }

        public override async Task<ApiResponse> GetArtistInfo(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}artist/{1}?inc=tags", _url, id));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Artist,
                Response = responseJson
            };
            return res;
        }

        public override Task<ApiResponse> GetSongInfo(string id)
        {
            throw new NotImplementedException();
        }

        public override async Task<ApiResponse> SearchArtists(string name)
        {
            JObject responseJson = await GetResponse(string.Format("{0}artist/?query={1}", _url, name));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Artist,
                Response = responseJson
            };
            return res;
        }

        protected override async Task<JObject> GetResponse(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DreamancersMusicService", "0.1"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.GetAsync(url);
                    if(response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return await GetResponse(url);
                    }
                    string responseString = await response.Content.ReadAsStringAsync();

                    return JObject.Parse(responseString);
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
