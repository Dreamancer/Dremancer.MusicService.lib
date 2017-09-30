using MusicWebApi.ApiWrappers;
using MusicWebApi.ApiWrappers.Enums;
using MusicWebApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.MusicRepo
{
    public partial class WebRepo
    {

        #region GetArtist
        private Artist GetSpotifyArtist(ApiResponse response, int apiNameValue, string artistId)
        {
            return new Artist
            {
                Name = response.Response["name"].Value<string>(),
                Genres = response.Response["genres"].Values<string>().ToList(),
                ApiIds = new Dictionary<string, string>()
            };
        }

        private Artist GetMusicBrainzArtist(ApiResponse response, int apiNameValue, string artistId)
        {
            Artist resultArtist = new Artist
            {
                Name = response.Response["name"].Value<string>(),
                Genres = response.Response["tags"].Values<string>("name").ToList(),
                Info = response.Response["disambiguation"].Value<string>(),
                Country = response.Response["country"].Value<string>(),
                ApiIds = new Dictionary<string, string>()
            };
            ApiResponse lastFMresponse = _apis[ApiName.LastFM].GetArtistInfo(artistId).Result;
            Artist lastFMArtist = new Artist
            {
                Name = lastFMresponse.Response["artist"].Value<string>("name"),
                Genres = lastFMresponse.Response["artist"]["tags"]["tag"].Values<string>("name").ToList(),
                Info = lastFMresponse.Response["artist"]["bio"]["content"].Value<string>(),
                ApiIds = new Dictionary<string, string>()
            };
            resultArtist.Update(lastFMArtist);
            return resultArtist;
        }
        #endregion

        #region SearchArtist
        private void SearchSpotifyArtists(ApiResponse response, List<Artist> artists)
        {
            var spotifyArtists = response.Response["artists"]["items"].Where(token => int.Parse(token["popularity"].Value<string>()) > 0);
            foreach (JToken spotifyArtist in spotifyArtists)
            {
                string jsonName = spotifyArtist["name"].Value<string>();
                string id = spotifyArtist["id"].Value<string>();

                Artist newArtist = new Artist
                {
                    Name = jsonName,
                    Genres = spotifyArtist["genres"].Values<string>().ToList(),
                    ApiIds = new Dictionary<string, string>()
                };
                newArtist.ApiIds.Add(_spotifyName, spotifyArtist["id"].Value<string>());

                var updateArtists = artists.Where(a => a.SearchEquals(newArtist)
                  );//&& !a.ApiIds.ContainsKey(_spotifyName));

                if (updateArtists.Count() > 0)
                {
                    updateArtists.ToList().ForEach(
                        a =>
                        {
                            a.Update(newArtist);
                        });
                }
                else
                {
                    artists.Add(newArtist);
                }
            }
        }

        private void SearchLastFmArtists(ApiResponse response, List<Artist> artists)
        {
            var lastFMArtists = response.Response["results"]["artistmatches"]["artist"].Where(token => int.Parse(token["listeners"].Value<string>()) > 50);
            foreach (JToken lastFMArtist in lastFMArtists)
            {
                string jsonName = lastFMArtist["name"].Value<string>();
                string id = lastFMArtist["mbid"].Value<string>();
                var updateArtists = artists.Where(a => a.Name == jsonName
                    && !a.ApiIds.ContainsKey(_musicBrainzName));
                if (updateArtists.Count() > 0 && !string.IsNullOrEmpty(id))
                {
                    updateArtists.ToList().ForEach(
                        a =>
                        {
                            a.ApiIds.Add(_musicBrainzName, id);
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
                    newArtist.ApiIds.Add(_musicBrainzName, id);
                    artists.Add(newArtist);
                }

            }
        }

        private void SearchMusicBrainzArtists(ApiResponse response, List<Artist> artists)
        {
            var musicBrainzArtists = response.Response["artists"].AsEnumerable();
            foreach (JToken musicBrainzArtist in musicBrainzArtists)
            {
                string jsonName = musicBrainzArtist["name"].Value<string>();
                string id = musicBrainzArtist["id"].Value<string>();
                List<string> genres = musicBrainzArtist["tags"] != null ? musicBrainzArtist["tags"].Values<string>("name").ToList() : new List<string>();

                Artist newArtist = new Artist
                {
                    Name = jsonName,
                    Genres = genres,
                    ApiIds = new Dictionary<string, string>()
                };
                newArtist.ApiIds.Add(_musicBrainzName, id);

                var updateArtists = artists.Where(a => a.SearchEquals(newArtist)
                    );//&& !a.ApiIds.ContainsKey(_musicBrainzName));

                if (updateArtists.Count() > 0)
                {
                    updateArtists.ToList().ForEach(
                        a =>
                        {
                            a.Update(newArtist);
                        });
                }
                else
                {
                    artists.Add(newArtist);
                }
            }
        }
        #endregion

        #region GetArtistAlbums
        private void GetSpotifyArtistAlbums(ApiResponse response, List<Album> albums)
        {
            var spotifyAlbums = response.Response["items"];
            foreach (JToken spotifyAlbum in spotifyAlbums)
            {
                string albumName = spotifyAlbum["name"].Value<string>();
                string albumType = spotifyAlbum["album_type"].Value<string>();
                Album newAlbum = new Album
                {
                    Name = albumName,
                    Type = albumType,
                    ApiIds = new Dictionary<string, List<string>>()
                };
                newAlbum.ApiIds.Add(_spotifyName, new string[]{ spotifyAlbum["id"].Value<string>() }.ToList());

                var albumsQuery = albums.Where(a => a.Name == albumName && a.Type.ToLower() == albumType.ToLower());
                if (albumsQuery.Count() == 0)
                {
                    albums.Add(newAlbum);
                }
                else
                {
                    foreach (Album album in albumsQuery)
                    {
                        album.Update(newAlbum);
                    }
                }
            }
        }

        private void GetMusicBrainzArtistAlbums(ApiResponse response, List<Album> albums)
        {
            var mbAlbums = response.Response["release-groups"];
            foreach (JToken mbAlbum in mbAlbums)
            {
                string albumName = mbAlbum["title"].Value<string>();
                string albumType = mbAlbum["primary-type"].Value<string>();
                string releaseDateString = mbAlbum["first-release-date"].Value<string>();
                DateTime releaseDate;
                Album newAlbum = new Album
                {
                    Name = albumName,
                    Type = albumType,
                    ReleaseDate = DateTime.TryParse(releaseDateString, out releaseDate) ? releaseDate as DateTime? : null,
                    ApiIds = new Dictionary<string, List<string>>()
                };
                newAlbum.ApiIds.Add(_musicBrainzName, new string[] { mbAlbum["id"].Value<string>() }.ToList());

                var albumsQuery = albums.Where(a => a.Name == albumName && a.Type.ToLower() == albumType.ToLower());
                if (albumsQuery.Count() == 0)
                {
                    albums.Add(newAlbum);
                }
                else
                {
                    foreach (Album album in albumsQuery)
                    {
                        album.Update(newAlbum);
                    }
                }
            }
        }
        #endregion

        public static string GetAlbumBaseName(string name)
        {
            if (!String.IsNullOrEmpty(name))
                return name.Split(" (")[0];
            else
                return name;
        }

    }
}
