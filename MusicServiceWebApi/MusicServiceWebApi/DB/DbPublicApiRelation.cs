using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.DB
{
    public class DbPublicApiRelation
    {
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public string ApiName { get; set; }
        public string ApiId { get; set; }

        public DbArtist Artist { get; set; }
    }
}
