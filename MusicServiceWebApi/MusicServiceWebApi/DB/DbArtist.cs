using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.DB
{
    public class DbArtist
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<DbPublicApiRelation> ApiIds { get; set; }

        public DbArtist()
        {
            ApiIds = new HashSet<DbPublicApiRelation>();
        }
    }
}
