﻿using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        private readonly YoutubeBrowserDbContextFactory factory;
        private List<Button> buttons = [];
        private Video video;

        public Window1(Video p_video)
        {
            InitializeComponent();
            factory = new YoutubeBrowserDbContextFactory();
            Display_Playlists();
            video = p_video;
        }

        
        //Funckja do wstawiania filmów do bazy danych
        // Do poprawienia
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string playlist_name = ((Button)sender).Content.ToString();
            playlist_textbox.Text = playlist_name;
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

                // Save changes to the database
                var lastAddedVideoTitle = playlist.Videos.LastOrDefault()?.Title;
                Messages.showMessageBox("Video added to the playlist.", "Success", MessageBoxButton.OK);
                playlist_textbox.Text = playlist.Videos.ToList()[3].Title;
                //playlist_textbox.Text = lastAddedVideoTitle;
            }
        }
        public async void Add_playlist_Video(object sender, RoutedEventArgs e, Video displayed_video)
        {
            try
            {


                using (var context = factory.CreateDbContext([]))
                {
                    // Check if the video already exists in the database
                    var existingVideo = await context.Videos.FirstOrDefaultAsync(v => v.YoutubeId == displayed_video.YoutubeId);

                    if (existingVideo == null)
                    {
                        // If the video doesn't exist, add it to the database
                        context.Videos.Add(new Video()
                        {
                            YoutubeId = displayed_video.YoutubeId,
                            Title = displayed_video.Title,
                            Thumbnail_url = displayed_video.Thumbnail_url
                        });
                    }

                    // Get the playlist
                    var playlist = await context.Playlists.FirstOrDefaultAsync(p => p.Name == playlist_textbox.Text);

                    if (playlist == null)
                    {
                        Messages.showMessageBox("Playlist not found.", "Error", MessageBoxButton.OK);
                        return;
                    }

                    // Check if the video is already in the playlist
                    var existingPlaylistVideo = await context.PlaylistsVideos
                        .FirstOrDefaultAsync(pv => pv.PlaylistId == playlist.Id && pv.Video.YoutubeId == displayed_video.YoutubeId);

                    if (existingPlaylistVideo != null)
                    {
                        Messages.showMessageBox("The video is already in the playlist.", "Cannot add the video", MessageBoxButton.OK);
                        return;
                    }

                    // If the video is not in the playlist, add it
                    Video videoToAdd;
                    if (existingVideo != null)
                    {
                        videoToAdd = existingVideo;
                    }
                    else
                    {
                        videoToAdd = displayed_video;
                    }

                    context.PlaylistsVideos.Add(new PlaylistVideo()
                    {
                        PlaylistId = playlist.Id,
                        VideoId = videoToAdd.Id
                    });

                    // Save changes to the database
                    await context.SaveChangesAsync();
                    Messages.showMessageBox("Video added to the playlist.", "Success", MessageBoxButton.OK);
                    playlist_textbox.Text = playlist.Videos.ToList()[0].Title;
                }
            }
            catch (Exception ex)
            {
                Messages.showMessageBox($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK);
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
