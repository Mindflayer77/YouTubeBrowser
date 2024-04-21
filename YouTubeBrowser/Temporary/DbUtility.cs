using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeBrowser.DbContexts;

namespace YoutubeBrowser.Temporary
{
    public class DbUtility
    {
        private static YoutubeBrowserDbContextFactory factory = new();

        public DbUtility() { }
        public static void ClearVideos()
        {
            using (var context = factory.CreateDbContext([]))
            {
                foreach (var item in context.Videos)
                {
                    context.Videos.Remove(item);
                }
                context.SaveChanges();
            }
        }

        public static void ClearPlaylists()
        {
            using (var context = factory.CreateDbContext([]))
            {
                foreach (var item in context.Playlists)
                {
                    context.Playlists.Remove(item);
                }
                context.SaveChanges();
            }
        }
    }
}
