using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.Models
{
    public class Artist
    {
        public string Name { get; set; }
        public List<string> Genres { get; set; }
        public string Country { get; set; }
        public string Info { get; set; }
        public Dictionary<string, string> ApiIds { get; set; }

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
            foreach(string key in ApiIds.Keys)
            {
                apiString += $"'{key}' : '{ApiIds[key]}'\n";
            }

            string result = $"{{ Name: {Name} \nGenres: {Genres}\nApiIds: {apiString} }}";
            return result;
        }

        public void Update(Artist artistUpdate)
        {
            Name = Name ?? artistUpdate.Name;
            foreach(string genre in artistUpdate.Genres)
            {
                if (!Genres.Contains(genre))
                {
                    Genres.Add(genre);
                }
            }
            Country = Country ?? artistUpdate.Country;
            foreach(KeyValuePair<string,string> apiId in  artistUpdate.ApiIds)
            {
                if (!ApiIds.ContainsKey(apiId.Key))
                {
                    ApiIds.Add(apiId.Key, apiId.Value);
                }
            }
            Info = Info == null || Info.Length < artistUpdate.Info.Length ? artistUpdate.Info : Info;
        }

    }
}
