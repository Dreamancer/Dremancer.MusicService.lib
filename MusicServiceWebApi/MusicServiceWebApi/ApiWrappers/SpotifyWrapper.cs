using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MusicServiceWebApi.ApiWrappers
{
    public class SpotifyWrapper : MusicApiWrapper
    {
        private static readonly string _apiName = "Spotify";
        public override string ApiName { get { return _apiName; } }

        public SpotifyWrapper(IConfiguration configuration) : base(configuration)
        {
            
        }

        public override async Task<ApiResponse> GetAlbumInfo(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}albums/{1} ", _url, id));
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
            JObject responseJson = await GetResponse(string.Format("{0}artists/{1} ", _url, id));
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
            JObject responseJson = await GetResponse(string.Format("{0}tracks/{id} ", _url, id));
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
            JObject responseJson = await GetResponse(string.Format("{0}search?q={1}&type=artist", _url, name.Replace(' ', '+')));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Artist,
                Response = responseJson
            };
            return res;
        }

        public override async Task<ApiResponse> GetArtistAlbums(string id)
        {
            JObject responseJson = await GetResponse(string.Format("{0}artists/{1}/albums ", _url, id));
            ApiResponse res = new ApiResponse
            {
                ApiName = ApiName,
                ResponseType = responseJson == null ? ResponseType.Error : ResponseType.Album,
                Response = responseJson
            };
            return res;
        }
        #region private helpers
        private async Task<string> GetAuthToken()
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] authBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _appId, _appSecret));
                string authBase64 = Convert.ToBase64String(authBytes);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authBase64);

                Dictionary<string, string> data = new Dictionary<string, string> { ["grant_type"] = "client_credentials" };
                var content = new FormUrlEncodedContent(data);
                HttpResponseMessage response;
                while (true)
                {
                    response = await client.PostAsync("https://accounts.spotify.com/api/token", content);
                    if (response.StatusCode == HttpStatusCode.OK)
                        break;
                }
                var responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString)["access_token"].Value<string>();
            }
        }

        protected override async Task<JObject> GetResponse(string url)
        {
            string token = await GetAuthToken();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(url);
                string responseString = await response.Content.ReadAsStringAsync();

                return JObject.Parse(responseString);
            }
        }
        #endregion
    }
}
