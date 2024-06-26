﻿using Microsoft.EntityFrameworkCore;
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
        public string ConnectionString { get; }
        /// <summary>
        ///  Default constructor for database context
        /// </summary>
        public YoutubeBrowserContext()
        {
            ConnectionString = "Data Source=(localdb)\\MSSqlLocalDB;Initial Catalog=YoutubeBrowser;Integrated Security=True";  
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
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
