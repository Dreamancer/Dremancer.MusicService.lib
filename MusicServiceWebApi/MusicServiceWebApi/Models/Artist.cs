using MusicWebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.Models
{
    public class Artist
    {
        public string Name { get; set; }
        public List<string> Genres { get; set; }
        public string Country { get; set; }
        public string Info { get; set; }
        public Dictionary<string, string> ApiIds { get; set; }
        public List<Album> Albums { get; set; }

        public Artist()
        {
            // Name = "";
            // Country = "";
            // Info = "";
            Genres = new List<string>();
            ApiIds = new Dictionary<string, string>();
        }


        public override string ToString()
        {
            string apiString = "";
            foreach (string key in ApiIds.Keys)
            {
                apiString += $"'{key}' : '{ApiIds[key]}'\n";
            }

            string result = $"{{ Name: {Name} \nGenres: {Genres}\nApiIds: {apiString} }}";
            return result;
        }

        public void Update(Artist artistUpdate)
        {
            Name = Name ?? artistUpdate.Name;
            Genres = Genres.Union(artistUpdate.Genres, StringComparer.OrdinalIgnoreCase).ToList();
            Country = Country ?? artistUpdate.Country;
            foreach (KeyValuePair<string, string> apiId in artistUpdate.ApiIds)
            {
                if (!ApiIds.ContainsKey(apiId.Key))
                {
                    ApiIds.Add(apiId.Key, apiId.Value);
                }
            }
            Info = Info == null || Info.Length < artistUpdate.Info.Length ? artistUpdate.Info : Info;
            if (artistUpdate.Albums != null)
            {
                Albums = (Albums != null && artistUpdate.Albums != null) ? Albums.Union(artistUpdate.Albums).ToList() : artistUpdate.Albums;
            }
        }

        public bool SearchEquals(Artist eqArtist)
        {
            bool nameMatch = Name != null ? Name == eqArtist.Name : false;
           // bool genrePrefixesMatch = eqArtist.Genres != null ? Genres.Intersect(eqArtist.Genres, new SearchGenrePrefixComparer()).Count() > 0 : false;
            bool genreMatch = eqArtist.Genres != null ? Genres.Intersect(eqArtist.Genres, new SearchGenreComparer()).Count() > 1 : false;
            if (!genreMatch)
            {
                genreMatch = eqArtist.Genres != null ? Genres.Intersect(eqArtist.Genres, new SearchGenrePrefixComparer()).Count() > 0 : false;
            }
            return nameMatch && genreMatch;
        }

    }
}
