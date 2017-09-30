using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.DB
{
    public class WebDbContext : DbContext
    {
        public DbSet<WebArtist> Artists { get; set; }
        public DbSet<WebAlbum> Albums { get; set; }
        public DbSet<ApiRelations> ApiIds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=WebMusicDb.db");
        }

        
    }
    public enum WebEntityType
    {
        Artist, Album, Song, Event
    }
}
