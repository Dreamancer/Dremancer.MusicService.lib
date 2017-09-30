using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.DB
{
    public class ApiRelations
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int EntityType { get; set; }
        public int ApiName { get; set; }
        public string EntityApiId { get; set; }

        //public WebArtist Artist { get; set; }
    }
}
