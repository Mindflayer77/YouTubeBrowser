using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using YoutubeBrowser.Models;

namespace YoutubeBrowser.DbContexts
{
    public class YoutubeBrowserContext : DbContext
    {
        public YoutubeBrowserContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            //builder.Entity<Playlist>().Property(p => p.Description).HasDefaultValue(null);
             
        }

        public DbSet<Video> Videos { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistVideo> PlaylistsVideos { get; set; }

    }
}
