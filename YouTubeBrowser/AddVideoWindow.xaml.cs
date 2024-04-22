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

    public partial class AddVideoWindow : Window
    {
        private readonly YoutubeBrowserDbContextFactory factory;
        private List<Button> buttons = [];
        private Video video;

        public AddVideoWindow(Video p_video)
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
                if (playlist.Videos.Where(p => p.YoutubeId == video.YoutubeId).Count() != 0)
                {
                    Messages.showMessageBox("The video is already in the playlist.", "Cannot add the video", MessageBoxButton.OK);
                    return;
                }
                playlist.Videos.Add(new Video()
                {
                    YoutubeId = video.YoutubeId,
                    Title = video.Title,
                    Thumbnail_url = video.Thumbnail_url
                });
                context.SaveChangesAsync();
                context.PlaylistsVideos.Add(new PlaylistVideo()
                {
                    PlaylistId = playlist.Id,
                    VideoId = video.Id
                });

                //var lastAddedVideoTitle = playlist.Videos.LastOrDefault()?.Title;
                Messages.showMessageBox("Video added to the playlist.", "Success", MessageBoxButton.OK);
            }
        }
        
        private void Create_Button(string name)
        {
            int column = 0;
            double textBoxWidth = playlist_textbox.Width;
            double textBoxHeight = playlist_textbox.Height;
            int rowCount = pnlMainGrid.RowDefinitions.Count;

            Button newButton = new Button();
            newButton.Content = name;
            newButton.Width = textBoxWidth;
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


        
        
        private void Create_Playlist_Click(object sender, RoutedEventArgs e)
        {
            string name_of_new_playlist = playlist_textbox.Text;
            playlist_textbox.Text = "";
            if(name_of_new_playlist == string.Empty)
            {
                Messages.showMessageBox("Please provide a name for your playlist.", "Cannot create a playlist", MessageBoxButton.OK);
                return;
            }
            using (var dbContext = factory.CreateDbContext([]))
            {
                if(dbContext.Playlists.Where(playlist => playlist.Name == name_of_new_playlist).Count() != 0)
                {
                    Messages.showMessageBox("There already exists a playlist with the given name.", "Cannot create a playlist", MessageBoxButton.OK);
                    return;
                }
                Create_Button(name_of_new_playlist);
                dbContext.Playlists.Add(new Playlist(){ Name = name_of_new_playlist, Description = "" , Videos = { } });
                dbContext.SaveChanges();
            }
        }
    }

    }
