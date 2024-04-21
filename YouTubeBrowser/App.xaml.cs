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
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string CONNECTION_STRING = "Data Source=.\\YoutubeBrowser.db";

        public App() : base() { }
 
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
