﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.DB
{
    public class WebAlbum
    {
        public int Id { get; set; }
        public string Name { get; set; }
   //     public DateTime RelaseDate { get; set; }
     //   public string Type { get; set; }

        public ICollection<ApiRelations> ApiIds
        {
            get
            {
                using (WebDbContext context = new WebDbContext())
                {
                    return context.ApiIds.Where(a => a.EntityType == (int)WebEntityType.Album && a.EntityId == this.Id) as ICollection<ApiRelations>;
                }
            }
        }
    }
}
