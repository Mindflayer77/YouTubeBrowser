using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoutubeBrowser.DbContexts;
using YoutubeBrowser.Models;
using YoutubeBrowser.Utility;

namespace YoutubeBrowser
{
    public partial class DeleteVideoWindow : Window
    {
        private readonly YoutubeBrowserDbContextFactory factory;
        private List<Button> buttons = [];
        private Video video;

        public DeleteVideoWindow(Video p_video)
        {
            InitializeComponent();
            factory = new YoutubeBrowserDbContextFactory();
            Display_Playlists();
            video = p_video;
        }

        //Funckja do wstawiania filmów do bazy danych
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string playlist_name = ((Button)sender).Content.ToString();
            using (var context = factory.CreateDbContext([]))
            {
                var playlist = context.Playlists.Where(p => p.Name == playlist_name).Include(p => p.Videos).First();

                try
                {
                    var video_to_remove = playlist.Videos.Where(v => v.YoutubeId == video.YoutubeId).First();
                    playlist.Videos.Remove(video_to_remove);
                    context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    Messages.showMessageBox("This video is not in the playlist.", "Cannot delete the video", MessageBoxButton.OK);
                    return;
                }
         
                Messages.showMessageBox("Video deleted from the playlist.", "Success", MessageBoxButton.OK);
            }
        }

        private void Create_Button(string name)
        {
            int column = 0;
  
            int rowCount = pnlMainGrid.RowDefinitions.Count;

            Button newButton = new Button();
            newButton.Content = name;
            newButton.Height = 25;
            newButton.Click += new RoutedEventHandler(Button_Click);
            pnlMainGrid.Children.Add(newButton);

            Grid.SetRow(newButton, rowCount);
            Grid.SetColumn(newButton, column);
            pnlMainGrid.RowDefinitions.Add(new RowDefinition());

            buttons.Add(newButton);
        }

        private void Display_Playlists()
        {
            List<Playlist> playlists;
            using (var dbContext = factory.CreateDbContext([]))
            {
                playlists = dbContext.Playlists.ToList();
            }

            foreach (var playlist in playlists)
            {
                Create_Button(playlist.Name);
            }
        }
    }
}
