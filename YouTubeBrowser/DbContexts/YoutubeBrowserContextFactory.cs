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
    /// <summary>
    /// Factory class which makes database context creation easier
    /// </summary>
    public class YoutubeBrowserDbContextFactory : IDesignTimeDbContextFactory<YoutubeBrowserContext>
    {   
        /// <summary>
        /// Method for creating database context.
        /// The context is created using Sqlite library.
        /// </summary>
        /// <param name="args">Optional arguments for database context</param>
        /// <returns>Database context of a type YoutubeBrowserContext</returns>
        public YoutubeBrowserContext CreateDbContext(string[] args)
        {
            return new YoutubeBrowserContext();
        }
    }
}
