using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.Models
{
    public class Artist
    {
        public string Name { get; set; }
        public string[] Genres { get; set; }
        public string Country { get; set; }
        public Dictionary<string, string> ApiIds { get; set; }

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

    }
}
