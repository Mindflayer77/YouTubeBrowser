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
    /// <summary>
    /// Class for managing database context
    /// </summary>
    public class YoutubeBrowserContext : DbContext
    {
        /// <summary>
        ///  Default constructor for database context
        /// </summary>
        /// <param name="options">Optional parameters for specifying databse properties</param>
        public YoutubeBrowserContext(DbContextOptions options) : base(options)
        {
            
        }

        /// <summary>
        /// Database set of Videos
        /// </summary>
        public DbSet<Video> Videos { get; set; }
        /// <summary>
        /// Database set of Playlists
        /// </summary>
        public DbSet<Playlist> Playlists { get; set; }
        /// <summary>
        /// Junction table between Videos and Playlists (many-to-many relationship)
        /// </summary>
        public DbSet<PlaylistVideo> PlaylistsVideos { get; set; }

    }
}
