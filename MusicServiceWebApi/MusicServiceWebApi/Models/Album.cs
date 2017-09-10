using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.Models
{
    public class Album
    {
    public string Name { get; private set; }
    public DateTime ReleaseDate { get; set; }
    public List<Song> Songs { get; private set; }
    public Dictionary<string, string> ApiIds { get; private set; }
  }
}
