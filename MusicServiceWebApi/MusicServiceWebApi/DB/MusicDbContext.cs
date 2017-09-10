using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.DB
{
    public class MusicDbContext : DbContext
    {
        public DbSet<DbArtist> Artists { get; set; }
        public DbSet<DbPublicApiRelation> ApiIds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=MusicServiceDb.db");
        }
    }
}
