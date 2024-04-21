using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeBrowser.DbContexts
{
    public class YoutubeBrowserDbContextFactory : IDesignTimeDbContextFactory<YoutubeBrowserContext>
    {
        public YoutubeBrowserContext CreateDbContext(string[] args)
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite("Data Source=.\\YoutubeBrowser.db").Options;
            return new YoutubeBrowserContext(options);
        }
    }
}
