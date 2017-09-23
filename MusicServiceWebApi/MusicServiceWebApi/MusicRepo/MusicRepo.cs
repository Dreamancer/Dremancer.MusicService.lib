using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicServiceWebApi.Models;
using MusicServiceWebApi.ApiWrappers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using MusicServiceWebApi.DB;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace MusicServiceWebApi.MusicRepo
{
    public class MusicRepo : IMusicRepo
    {
        private readonly IDictionary<string, IMusicApiWrapper> _apis;
        private readonly MusicDbContext _dbContext = new MusicDbContext();
        private static readonly ILog _log = LogManager.GetLogger(typeof(MusicRepo));

        public MusicRepo(IConfiguration configuration)
        {
            _apis = new Dictionary<string, IMusicApiWrapper>
            {
                ["Spotify"] = new SpotifyWrapper(configuration),
                ["LastFM"] = new LastFmWrapper(configuration),
                ["MB"] = new MusicBrainzWrapper(configuration)
            };
        }

        public void ClearTempContext(string tableName)
        {
            int result1 = _dbContext.Database.ExecuteSqlCommand($"delete from {tableName}");

        }

        public Album GetAlbum(string id)
        {
            throw new NotImplementedException();
        }

        public Artist GetArtist(string spotifyId)
        {
            var apiId = _dbContext.ApiIds.Where(a => a.ApiName == "Spotify" && a.ApiId == spotifyId).FirstOrDefault();
            DbArtist dbArtist = _dbContext.Artists.Where(a => a.Id == apiId.ArtistId).Include(a=> a.ApiIds).FirstOrDefault();
            Artist resultArtist = new Artist();

            foreach (var dbApiId in dbArtist.ApiIds)
            {
                Artist apiArtist = GetArtist(dbApiId);
                resultArtist.Update(apiArtist);
            }

            return resultArtist;
        }

        public Artist GetArtist(Guid mbId)
        {
            var apiId = _dbContext.ApiIds.Where(a => a.ApiName == "MB" && a.ApiId == mbId.ToString()).FirstOrDefault();
            DbArtist dbArtist = _dbContext.Artists.Where(a => a.Id == apiId.ArtistId).Include(a => a.ApiIds).FirstOrDefault();

            Artist resultArtist = new Artist();

            foreach (var dbApiId in dbArtist.ApiIds)
            {
                Artist apiArtist = GetArtist(dbApiId);
                resultArtist.Update(apiArtist);
            }

            return resultArtist;
        }

        private Artist GetArtist(DbPublicApiRelation apiId)
        {
            string name = apiId.ApiName;
            Artist resultArtist = new Artist();
            ApiResponse response = _apis[name].GetArtistInfo(apiId.ApiId).Result;
            switch (name)
            {
                case "Spotify":
                    resultArtist = new Artist
                    {
                        Name = response.Response["name"].Value<string>(),
                        Genres = response.Response["genres"].Values<string>().ToList(),
                        ApiIds = new Dictionary<string, string>()
                    };
                    break;
                case "MB":
                    resultArtist = new Artist
                    {
                        Name = response.Response["name"].Value<string>(),
                        Genres = response.Response["tags"].Values<string>("name").ToList(),
                        Info = response.Response["disambiguation"].Value<string>(),
                        Country = response.Response["country"].Value<string>(),
                        ApiIds = new Dictionary<string, string>()
                    };
                    ApiResponse lastFMresponse = _apis["LastFM"].GetArtistInfo(apiId.ApiId).Result;
                    Artist lastFMArtist = new Artist
                    {
                        Name = lastFMresponse.Response["artist"].Value<string>("name"),
                        Genres = lastFMresponse.Response["artist"]["tags"]["tag"].Values<string>("name").ToList(),
                        Info = lastFMresponse.Response["artist"]["bio"]["content"].Value<string>(),
                        ApiIds = new Dictionary<string, string>()
                    };
                    //foreach (JToken tag in lastFMresponse.Response["artist"]["tags"].Children())
                    //{
                    //    lastFMArtist.Genres.Add(tag["name"].Value<string>());
                    //}
                    resultArtist.Update(lastFMArtist);
                    break;
            }
            resultArtist.ApiIds.Add(apiId.ApiName, apiId.ApiId);
            return resultArtist;
        }

        public IEnumerable<Album> GetArtistAlbums(string id)
        {
            throw new NotImplementedException();
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
            foreach (string apiName in _apis.Keys)
            {
                try
                {
                    ApiResponse response = _apis[apiName].SearchArtists(name).Result;
                    if (response != null && response.ResponseType != ResponseType.Error)
                    {
                        switch (apiName)
                        {
                            case "Spotify":
                                var spotifyArtists = response.Response["artists"]["items"].Where(token => int.Parse(token["popularity"].Value<string>()) > 0);
                                foreach (JToken spotifyArtist in spotifyArtists)
                                {
                                    Artist newArtist = new Artist
                                    {
                                        Name = spotifyArtist["name"].Value<string>(),
                                        Genres = spotifyArtist["genres"].Values<string>().ToList(),
                                        ApiIds = new Dictionary<string, string>()
                                    };
                                    newArtist.ApiIds.Add(apiName, spotifyArtist["id"].Value<string>());
                                    artists.Add(newArtist);
                                }
                                break;
                            case "LastFM":
                                var lastFMArtists = response.Response["results"]["artistmatches"]["artist"].Where(token => int.Parse(token["listeners"].Value<string>()) > 50);
                                foreach (JToken lastFMArtist in lastFMArtists)
                                {
                                    string jsonName = lastFMArtist["name"].Value<string>();
                                    string id = lastFMArtist["mbid"].Value<string>();
                                    var updateArtists = artists.Where(a => a.Name == jsonName
                                        && !a.ApiIds.ContainsKey("MB"));
                                    if (updateArtists.Count() > 0 && !string.IsNullOrEmpty(id))
                                    {
                                        updateArtists.ToList().ForEach(
                                            a =>
                                            {
                                                a.ApiIds.Add("MB", id);
                                            });
                                    }
                                    else
                                    {
                                        Artist newArtist = new Artist
                                        {
                                            Name = jsonName,
                                            Genres = new List<string>(),
                                            ApiIds = new Dictionary<string, string>()
                                        };
                                        newArtist.ApiIds.Add("MB", id);
                                        artists.Add(newArtist);
                                    }

                                }
                                break;
                            case "MB":
                                var musicBrainzArtists = response.Response["artists"].AsEnumerable();
                                foreach (JToken musicBrainzArtist in musicBrainzArtists)
                                {
                                    string jsonName = musicBrainzArtist["name"].Value<string>();
                                    string id = musicBrainzArtist["id"].Value<string>();
                                    List<string> genres = musicBrainzArtist["tags"] != null ? musicBrainzArtist["tags"].Values<string>("name").ToList() : new List<string>();

                                    var updateArtists = artists.Where(a => a.Name == jsonName
                                        && a.ApiIds.ContainsKey("MB") && a.ApiIds["MB"] == id);

                                    if (updateArtists.Count() > 0)
                                    {
                                        updateArtists.ToList().ForEach(
                                            a =>
                                            {
                                                a.Genres = a.Genres.Union(genres).ToList();
                                            });
                                    }
                                    else
                                    {
                                        Artist newArtist = new Artist
                                        {
                                            Name = jsonName,
                                            Genres = genres,
                                            ApiIds = new Dictionary<string, string>()
                                        };
                                        newArtist.ApiIds.Add("MB", id);
                                        artists.Add(newArtist);
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            List<Artist> dbUpdate = new List<Artist>(artists);
            Task.Run(() => UpdateDbArtists(dbUpdate));
            return artists;
        }

        private async void UpdateDbArtists(List<Artist> artists)
        {
            try
            {
                foreach (Artist artist in artists)
                {
                    if (artist.ApiIds != null && artist.ApiIds.Count > 0)
                    {
                        var artistsQuery = _dbContext.Artists.Where(a => a.Name == artist.Name).Include(a => a.ApiIds);//for each artist we want to update get all artists from the db with the same name
                        if (artistsQuery.Count() > 0)
                        {
                            foreach (var dbArtist in artistsQuery)
                            {
                                foreach (KeyValuePair<string, string> apiId in artist.ApiIds)//for each of the artists from db if they are missing an api key and it can be updated, do so
                                {
                                    if (dbArtist.ApiIds.Where(a => a.ApiName == apiId.Key && a.ApiId == apiId.Value).Count() == 0)
                                    {
                                        DbPublicApiRelation dbApiId = new DbPublicApiRelation
                                        {
                                            ApiId = apiId.Value,
                                            ApiName = apiId.Key,
                                            ArtistId = dbArtist.Id,
                                            Artist = dbArtist
                                        };
                                        _dbContext.ApiIds.Add(dbApiId);
                                    }
                                }
                            }
                        }
                        else//if there is no artist in the db with such a name, add one
                        {
                            DbArtist dbArtist = new DbArtist
                            {
                                Name = artist.Name
                            };
                            _dbContext.Add(dbArtist);
                            foreach (KeyValuePair<string, string> apiId in artist.ApiIds)
                            {
                                DbPublicApiRelation dbApiId = new DbPublicApiRelation
                                {
                                    ApiId = apiId.Value,
                                    ApiName = apiId.Key,
                                    ArtistId = dbArtist.Id,
                                    Artist = dbArtist
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
