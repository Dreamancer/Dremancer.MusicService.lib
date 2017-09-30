using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicWebApi.Models;
using MusicWebApi.ApiWrappers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using MusicWebApi.DB;
using Microsoft.EntityFrameworkCore;
using log4net;
using MusicWebApi.ApiWrappers.Enums;
using MusicWebApi.Helpers;

namespace MusicWebApi.MusicRepo
{
    public partial class WebRepo : IWebRepo
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(WebRepo));

        private readonly IDictionary<ApiName, IMusicApiWrapper> _apis;
        private readonly WebDbContext _dbContext = new WebDbContext();

        private static readonly string _spotifyName = ApiName.Spotify.StringName(),
                                        _lastFmName = ApiName.LastFM.StringName(),
                                        _musicBrainzName = ApiName.MusicBrainz.StringName();

        private IDictionary<ApiName, Delegate> _getArtistActions;
        private IDictionary<ApiName, Action<ApiResponse, List<Artist>>> _searchArtistActions;
        private IDictionary<ApiName, Action<ApiResponse, List<Album>>> _getArtistAlbumsActions;

        public WebRepo(IConfiguration configuration)
        {
            _apis = new Dictionary<ApiName, IMusicApiWrapper>
            {
                [ApiName.Spotify] = new SpotifyWrapper(configuration),
                [ApiName.LastFM] = new LastFmWrapper(configuration),
                [ApiName.MusicBrainz] = new MusicBrainzWrapper(configuration)
            };

            _getArtistActions = new Dictionary<ApiName, Delegate>
            {
                [ApiName.Spotify] = new Func<ApiResponse, int, string, Artist>(GetSpotifyArtist),
                [ApiName.MusicBrainz] = new Func<ApiResponse, int, string, Artist>(GetMusicBrainzArtist)
            };

            _searchArtistActions = new Dictionary<ApiName, Action<ApiResponse, List<Artist>>>
            {
                [ApiName.Spotify] = new Action<ApiResponse, List<Artist>>(SearchSpotifyArtists),
                // [ApiName.LastFM] = new Action<ApiResponse, List<Artist>>(SearchLastFmArtists),
                [ApiName.MusicBrainz] = new Action<ApiResponse, List<Artist>>(SearchMusicBrainzArtists)
            };
            _getArtistAlbumsActions = new Dictionary<ApiName, Action<ApiResponse, List<Album>>>
            {
                [ApiName.Spotify] = new Action<ApiResponse, List<Album>>(GetSpotifyArtistAlbums),
                [ApiName.MusicBrainz] = new Action<ApiResponse, List<Album>>(GetMusicBrainzArtistAlbums)
            };

            //temp
            // ClearTempContext("Artists");
            // ClearTempContext("Albums");
            //  ClearTempContext("ApiIds");
        }

        public void ClearTempContext(string tableName)
        {
            int result1 = _dbContext.Database.ExecuteSqlCommand(string.Format("delete from {0}", tableName));
        }

        public Album GetAlbum(string spotifyId)
        {
            throw new NotImplementedException();
            //var apiId = _dbContext.ApiIds.Where(a => a.ApiName == (int)ApiName.Spotify && a.EntityApiId == spotifyId && a.EntityType == (int)WebEntityType.Album).FirstOrDefault();

        }

        public Album GetAlbum(Guid mbId)
        {
            throw new NotImplementedException();
        }

        private Album GetAlbum(ApiRelations apiId)
        {
            throw new NotImplementedException();
        }

        public Artist GetArtist(string spotifyId)
        {
            var apiId = _dbContext.ApiIds.Where(a => a.ApiName == (int)ApiName.Spotify && a.EntityApiId == spotifyId && a.EntityType == (int)WebEntityType.Artist).FirstOrDefault();
            var artistApiIds = _dbContext.ApiIds.Where(a => a.EntityId == apiId.EntityId);

            // WebArtist dbArtist = _dbContext.Artists.Where(a => a.Id == apiId.EntityId).Include(a => a.ApiIds).FirstOrDefault();
            Artist resultArtist = new Artist();

            var spotifyApiAlbumsResponse = _apis[ApiName.Spotify].GetArtistAlbums(spotifyId).Result;
            List<Album> spotifyAlbums = new List<Album>();

            GetSpotifyArtistAlbums(spotifyApiAlbumsResponse, spotifyAlbums);

            resultArtist.Albums = spotifyAlbums;

            foreach (var dbApiId in artistApiIds)
            {
                bool sameApiId = dbApiId == apiId;             

                List<Album> apiAlbums = new List<Album>();
                if (sameApiId)
                {
                    Artist apiArtist = GetArtist(dbApiId);
                    resultArtist.Update(apiArtist);
                }
                else
                {
                    var apiAlbumsResponse = dbApiId.ApiName == (int)ApiName.Spotify ? _apis[ApiName.Spotify].GetArtistAlbums(dbApiId.EntityApiId).Result
                    : _apis[ApiName.MusicBrainz].GetArtistAlbums(dbApiId.EntityApiId).Result;

                    if (dbApiId.ApiName == (int)ApiName.Spotify)
                    {
                        GetSpotifyArtistAlbums(apiAlbumsResponse, apiAlbums);
                    }
                    else
                    {
                        GetMusicBrainzArtistAlbums(apiAlbumsResponse, apiAlbums);
                    }
                    var match = resultArtist.Albums.Intersect(apiAlbums);
                    if (match.Count() >= 2)
                    {
                        Artist apiArtist = GetArtist(dbApiId);
                        apiArtist.Albums = apiAlbums;
                        resultArtist.Update(apiArtist);

                        foreach (Album al in resultArtist.Albums)
                        {
                            if (match.Contains(al))
                            {
                                Album updateAlbum = apiAlbums.Where(a => a.Equals(al)).FirstOrDefault();
                                al.Update(updateAlbum);
                            }
                        }
                    }
                }
            }

            return resultArtist;
        }

        public Artist GetArtist(Guid mbId)
        {
            var apiId = _dbContext.ApiIds.Where(a => a.ApiName == (int)ApiName.MusicBrainz && a.EntityApiId == mbId.ToString() && a.EntityType == (int)WebEntityType.Artist).FirstOrDefault();
            var artistApiIds = _dbContext.ApiIds.Where(a => a.EntityId == apiId.EntityId);

            //WebArtist dbArtist = _dbContext.Artists.Where(a => a.Id == apiId.EntityId).Include(a => a.ApiIds).FirstOrDefault();

            Artist resultArtist = new Artist();
            var mbApiAlbumsResponse = _apis[ApiName.MusicBrainz].GetArtistAlbums(mbId.ToString()).Result;
            List<Album> mbAlbums = new List<Album>();

            GetMusicBrainzArtistAlbums(mbApiAlbumsResponse, mbAlbums);
            resultArtist.Albums = mbAlbums;

            foreach (var dbApiId in artistApiIds.Where(a => a == apiId || a.ApiName != (int)ApiName.MusicBrainz))
            {
                bool sameApi = dbApiId == apiId;
              
                List<Album> apiAlbums = new List<Album>();

                if (sameApi)
                {
                    Artist apiArtist = GetArtist(dbApiId);
                    resultArtist.Update(apiArtist);
                }
                else
                {
                    var apiAlbumsResponse = _apis[ApiName.Spotify].GetArtistAlbums(dbApiId.EntityApiId).Result;

                    GetSpotifyArtistAlbums(apiAlbumsResponse, apiAlbums);
                    var match = resultArtist.Albums.Intersect(apiAlbums);
                    if (match.Count() >= 2)
                    {
                        Artist apiArtist = GetArtist(dbApiId);
                        apiArtist.Albums = apiAlbums;
                        resultArtist.Update(apiArtist);

                        foreach (Album al in resultArtist.Albums)
                        {
                            if (match.Contains(al))
                            {
                                Album updateAlbum = apiAlbums.Where(a => a.Equals(al)).FirstOrDefault();
                                al.Update(updateAlbum);
                            }
                        }
                    }
                }

            }
            return resultArtist;
        }

        private Artist GetArtist(ApiRelations apiId)
        {
            int name = apiId.ApiName;
            ApiResponse response = _apis[(ApiName)name].GetArtistInfo(apiId.EntityApiId).Result;
            Artist resultArtist = _getArtistActions[(ApiName)name].DynamicInvoke(response, name, apiId.EntityApiId) as Artist;

            resultArtist.ApiIds.Add(((ApiName)name).StringName(), apiId.EntityApiId);
            return resultArtist;
        }

        public IEnumerable<Album> GetArtistAlbums(int id)
        {
            IEnumerable<Album> albums = new List<Album>();
            WebArtist artist = _dbContext.Artists.Where(a => a.Id == id).Include(a => a.ApiIds).FirstOrDefault();
            foreach (var apiId in artist.ApiIds)
            {
                int name = apiId.ApiName;
                ApiResponse response = _apis[(ApiName)name].GetArtistAlbums(apiId.EntityApiId).Result;
                _getArtistAlbumsActions[(ApiName)name].DynamicInvoke(response, albums);
            }
            //async update albums in db
            return albums;
        }

        public IEnumerable<Event> GetArtistEvents(string artistId)
        {
            throw new NotImplementedException();
        }

        public Song GetSong(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Artist> SearchArtists(string name)
        {
            List<Artist> artists = new List<Artist>();
            foreach (ApiName apiName in _apis.Keys)
            {
                try
                {
                    ApiResponse response = _apis[apiName].SearchArtists(name).Result;
                    if (response != null && response.ResponseType != ResponseType.Error)
                    {
                        _searchArtistActions[apiName].DynamicInvoke(response, artists);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            List<Artist> dbUpdate = new List<Artist>(artists);
            Task.Run(() => UpdateDbArtists(dbUpdate));
            artists = artists.OrderBy(a => DreamancerHelper.LevenshteinDistance(a.Name.ToLower(), name.ToLower())).ThenByDescending(a => a.Genres.Count).ToList();
            return artists;
        }


        //todo previest na dictionary ked sa prida viac api
        private async void UpdateDbArtists(List<Artist> artists)
        {
            try
            {
                foreach (Artist artist in artists)
                {
                    if (artist.ApiIds != null && artist.ApiIds.Count > 0)
                    {
                        var artistsQuery = _dbContext.Artists.Where(a => a.Name == artist.Name);
                        //for each artist we want to update get all artists from the db with the same name
                        if (artistsQuery.Count() > 0)
                        {
                            foreach (var dbArtist in artistsQuery)
                            {
                                ICollection<ApiRelations> apiIds = _dbContext.ApiIds.Where(a => a.EntityId == dbArtist.Id && a.EntityType == (int)WebEntityType.Artist).ToList();
                                foreach (KeyValuePair<string, string> apiId in artist.ApiIds)//for each of the artists from db if they are missing an api key and it can be updated, do so
                                {
                                    if (apiIds.Where(a => (a.ApiName == ((int)EnumExtensions.ApiNameFromString(apiId.Key)) && a.EntityApiId == apiId.Value)).Count() == 0)
                                    {
                                        ApiRelations dbApiId = new ApiRelations
                                        {
                                            EntityApiId = apiId.Value,
                                            ApiName = (int)EnumExtensions.ApiNameFromString(apiId.Key),
                                            EntityId = dbArtist.Id,
                                            EntityType = (int)WebEntityType.Artist
                                        };
                                        _dbContext.ApiIds.Add(dbApiId);
                                    }
                                }
                            }
                        }
                        else//if there is no artist in the db with such a name, add one
                        {
                            WebArtist dbArtist = new WebArtist
                            {
                                Name = artist.Name
                            };
                            var newDbArtist = _dbContext.Add(dbArtist);
                            await _dbContext.SaveChangesAsync();
                            foreach (KeyValuePair<string, string> apiId in artist.ApiIds)
                            {
                                ApiRelations dbApiId = new ApiRelations
                                {
                                    EntityApiId = apiId.Value,
                                    ApiName = (int)EnumExtensions.ApiNameFromString(apiId.Key),
                                    EntityId = newDbArtist.Entity.Id,
                                    EntityType = (int)WebEntityType.Artist
                                };
                                _dbContext.ApiIds.Add(dbApiId);
                            }
                        }
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}
