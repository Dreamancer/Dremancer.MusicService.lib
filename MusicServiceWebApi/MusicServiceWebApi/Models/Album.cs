using MusicWebApi.MusicRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.Models
{
    public class Album
    {
        public string Name { get; set; }
        public List<string> AltRelaseNames { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public List<Song> Songs { get; set; }
        public string Type { get; set; }

        public Dictionary<string, List<string>> ApiIds { get; set; }

        public void Update(Album albumUpdate)
        {
            UpdateName(albumUpdate.Name);
            ReleaseDate = ReleaseDate ?? albumUpdate.ReleaseDate;
            Songs = Songs ?? albumUpdate.Songs;
            foreach (KeyValuePair<string, List<string>> apiId in albumUpdate.ApiIds)
            {
                if (ApiIds.ContainsKey(apiId.Key)
                    && ApiIds[apiId.Key].Intersect(apiId.Value).Count() == 0)
                {
                    ApiIds[apiId.Key].AddRange(apiId.Value);
                }
                else
                {
                    ApiIds.Add(apiId.Key, apiId.Value);
                }
            }
            Type = Type ?? albumUpdate.Type;
        }

        private void UpdateName(string newName)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                string nameBase = WebRepo.GetAlbumBaseName(Name.ToLower());
                string updateNameBase = WebRepo.GetAlbumBaseName(newName.ToLower());
                if (Name.ToLower() == nameBase && newName.ToLower() != updateNameBase)
                {
                    if (AltRelaseNames != null)
                    {
                        AltRelaseNames.Add(newName);
                    }
                    else
                    {
                        AltRelaseNames = new List<string>(new string[] { newName }.ToList());
                    }
                }
                else if(newName.ToLower() == updateNameBase && Name.ToLower() != nameBase)
                {
                    if (AltRelaseNames != null)
                    {
                        AltRelaseNames.Add(Name);
                    }
                    else
                    {
                        AltRelaseNames = new List<string>(new string[] { Name }.ToList());
                    }
                    Name = newName;
                }
            }
            else
            {
                Name = newName;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Album))
                return false;
            else
            {
                Album objAlbum = obj as Album;
                string thisName = WebRepo.GetAlbumBaseName(Name.ToLower());
                string equalsName = WebRepo.GetAlbumBaseName(objAlbum.Name.ToLower());
                bool result = thisName == equalsName
                    && this.Type.ToLower() == objAlbum.Type.ToLower();
                return result;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = -243844509;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WebRepo.GetAlbumBaseName(Name.ToLower()));
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type.ToLower());
            return hashCode;
        }
    }
}
