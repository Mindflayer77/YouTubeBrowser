using System.Configuration;
using System.Data;
using System.Windows;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.EntityFrameworkCore;
using YoutubeBrowser.DbContexts;
namespace YoutubeBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml. Represents the entry point for the application.
    /// </summary>
    public partial class App : Application
    {   
        /// <summary>
        /// Connection string representing the path to the database.
        /// </summary>
        public const string CONNECTION_STRING = "Data Source=.\\YoutubeBrowser.db";

        /// <summary>
        /// Initializes a new instance of the App class
        /// </summary>
        public App() : base() { }

        /// <summary>
        /// Invoked when the application is starting up.
        /// </summary>
        /// <param name="e">The StartupEventArgs that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(CONNECTION_STRING).Options;
            YoutubeBrowserContext context  = new YoutubeBrowserContext(options);
            context.Database.Migrate();
              
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }

}
