using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.DevTools.Network;
using Microsoft.EntityFrameworkCore;
using YoutubeBrowser.ApiService;
using YoutubeBrowser.DbContexts;
using YoutubeBrowser.Models;
using YoutubeBrowser.Temporary;
using YoutubeBrowser.Utility;

namespace YoutubeBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IYoutubeApi apiService;
        private Dictionary<string, Video> videos = [];
        private Video? displayed_video = null;
        private readonly YoutubeBrowserDbContextFactory factory;
        private bool playlistsDisplayed = false;
        private Playlist? displayed_playlist = null;
        private ScrollViewer scrollViewer;

        public string search_text = "";

        public MainWindow()
        {
            InitializeComponent();
            apiService = new YoutubeApiService();
            factory = new YoutubeBrowserDbContextFactory();
            videos = [];
        }

        private void Clear_Videos_Click(object sender, RoutedEventArgs e)
        {
            DbUtility.ClearVideos();
        }

        private void Clear_Playlists_Click(object sender, RoutedEventArgs e)
        {
            DbUtility.ClearPlaylists();
        }

        private async void Click_Search(object sender, RoutedEventArgs e)
        {
            Browser_Test.Visibility = Visibility.Visible;
            search_text = textBox.Text;
            videos.Clear();
            List<Video> tmp_videos = [];
            try
            {
                tmp_videos = await apiService.GetVideos(search_text, 10);
            }
            catch (Exception)
            {
                Messages.showMessageBox("Cannot execute request", "Daily Youtube quota exceeded", MessageBoxButton.OK);
                return;
            }
            foreach (var video in tmp_videos)
            {
                videos[video.YoutubeId] = video;
            }
            DestroyImages();
            if (videos.Count == 0)
            {
                Messages.showMessageBox("Empty result", "No videos found", MessageBoxButton.OK);
                displayed_video = null;
                return;
            }
            DisplayImages();
            Browser_Test.Address = GetEmbedAddress(videos.ElementAt(0).Value.YoutubeId);
            displayed_video = videos.ElementAt(0).Value;
            displayed_playlist = null;
        }

        private static string GetEmbedAddress(string youtube_Id)
        {
            return "https://www.youtube.com/embed/" + youtube_Id;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (displayed_video == null)
            {
                return;
            }
            var window = new AddVideoWindow(displayed_video);
            window.ShowDialog();
            Update_Playlist_Videos();
            if (playlistsDisplayed)
            {
                UpdatePlaylistView();
            }
        }

        private void Click_Delete(object sender, RoutedEventArgs e)
        {
            if (displayed_video == null)
            {
                Messages.showMessageBox("No videos are currently displayed", "Cannot delete video", MessageBoxButton.OK);
                return;
            }
            var window = new DeleteVideoWindow(displayed_video);
            window.Show();
            Update_Playlist_Videos();
        }

        private void Update_Playlist_Videos()
        {
            videos.Clear();
            using (var context = factory.CreateDbContext([]))
            {
                if (displayed_playlist == null)
                {
                    return;
                }
                Messages.showMessageBox("No videos are currently displayed", "Cannot add video", MessageBoxButton.OK);

                DestroyImages();
                var playlist = context.Playlists.Where(p => p.Name == displayed_playlist.Name).Include(p => p.Videos).First();
                foreach (var video in playlist.Videos.ToList())
                {
                    videos[video.YoutubeId] = video;
                }

            }
            DisplayImages();
        }

        private void UpdatePlaylistView()
        {
            RemovePlaylistScrollViewer();
            Create_Playlist_ScrollViewer();
        }



        private void Image_Click(object sender, RoutedEventArgs e)
        {
            string video_id = ((Image)sender).Tag.ToString();
            Browser_Test.Address = GetEmbedAddress(video_id);
            displayed_video = findVideo(video_id);
        }

        private void DisplayImages()
        {
            foreach (KeyValuePair<string, Video> video in videos)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(video.Value.Thumbnail_url, UriKind.Absolute);
                bitmap.EndInit();
                var panel = new WrapPanel();
                var image = new Image();
                image.Tag = video.Key;
                image.Source = bitmap;
                image.MaxHeight = 50;
                image.MouseLeftButtonDown += new MouseButtonEventHandler(Image_Click);
                image.MouseRightButtonDown += new MouseButtonEventHandler(Image_RightClick);
                panel.Children.Add(image);
                videosPanel.Children.Add(panel);
            }
        }

        private void DestroyImages()
        {
            videosPanel.Children.Clear();
        }

        private void Image_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (displayed_playlist == null)
                return;
            string video_id = ((Image)sender).Tag.ToString();

            Remove_video_from_displayed_playlist(video_id);
            Update_Playlist_Videos();
        }

        private void Remove_video_from_displayed_playlist(string videoId)
        {
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the video ?", "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = factory.CreateDbContext([]))
                {
                    var playlist = context.Playlists.Where(p => p.Name == displayed_playlist.Name).Include(p => p.Videos).First();

                    try
                    {
                        var video_to_remove = playlist.Videos.Where(v => v.YoutubeId == videoId).First();
                        playlist.Videos.Remove(video_to_remove);
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        Messages.showMessageBox("Error", "Cannot delete the video", MessageBoxButton.OK);
                        return;
                    }
                }
            }
        }

        private void YourPlaylists_Click(object sender, RoutedEventArgs e)
        {
            if(playlistsDisplayed)
            {
                RemovePlaylistScrollViewer();
                playlistsDisplayed = false;
                return;
            }
            Create_Playlist_ScrollViewer();
        }

        private async void Create_Playlist_ScrollViewer()
        {
            ColumnDefinition separatorColumn = new ColumnDefinition();
            separatorColumn.Width = new GridLength(10);
            Central_Grid.ColumnDefinitions.Add(separatorColumn);
            ColumnDefinition playlistColumn = new ColumnDefinition();
            playlistColumn.Width = new GridLength(150);
            Central_Grid.ColumnDefinitions.Add(playlistColumn);

            ScrollViewer playlistScrollViewer = new ScrollViewer();
            playlistScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            playlistScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            playlistScrollViewer.HorizontalAlignment = HorizontalAlignment.Left;


            StackPanel playlistStackPanel = new StackPanel();
            playlistScrollViewer.Content = playlistStackPanel;

            using (var context = factory.CreateDbContext([]))
            {
                var playlists = await context.Playlists.ToListAsync();

                foreach (var playlist in playlists)
                {
                    var newButton = Create_Playlist_Button(playlist.Name, playlistColumn.Width);
                    newButton.Tag = playlist.Id;
                    newButton.Click += Playlist_Button_Click;
                    playlistStackPanel.Children.Add(newButton);
                }
            }

            Central_Grid.Children.Add(playlistScrollViewer);
            Grid.SetColumn(playlistScrollViewer, Central_Grid.ColumnDefinitions.Count - 1);
            Grid.SetRow(playlistScrollViewer, 2);
            playlistsDisplayed = true;
            scrollViewer = playlistScrollViewer;
        }

        private void RemovePlaylistScrollViewer()
        {
            Central_Grid.Children.Remove(scrollViewer);
            Central_Grid.ColumnDefinitions.RemoveRange(Central_Grid.ColumnDefinitions.Count - 2, 2);
        }

        private Button Create_Playlist_Button(string name, GridLength width)
        {
            Button newButton = new Button();
            newButton.Content = name;
            newButton.Width = width.Value;
            newButton.Height = 25;
            newButton.Click += new RoutedEventHandler(Playlist_Button_Click);

            //right click to delete
            newButton.PreviewMouseRightButtonDown += new MouseButtonEventHandler(Playlist_Button_PreviewMouseRightButtonDown);

            return newButton;
        }

        private void Playlist_Button_Click(object sender, RoutedEventArgs e)
        {

            videos.Clear();
            string playlist_name = ((Button)sender).Content.ToString();
            textBox.Text = "";
           
            using (var context = factory.CreateDbContext([]))
            {
                var playlist = context.Playlists.Where(p => p.Name == playlist_name).Include(p => p.Videos).First();
                displayed_playlist = playlist;
                foreach (var video in playlist.Videos)
                {
                    videos[video.YoutubeId] = video;
                }
            }
            DestroyImages();
            DisplayImages();
        }

        private void Playlist_Button_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            string playlist_name = ((Button)sender).Content.ToString();

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the playlist '{playlist_name}'?", "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                
                using (var context = factory.CreateDbContext([]))
                {
                    //playlist to delete
                    var playlist = context.Playlists.FirstOrDefault(p => p.Name == playlist_name);
                    
                    if (playlist != null)
                    {
                        //columns with PlaylistId for removing
                        var relatedRecordsfromPlaylist = context.PlaylistsVideos.Where(x => x.PlaylistId == playlist.Id);

                        //removing data from DB Playlist and PlaylistVideo
                        context.PlaylistsVideos.RemoveRange(relatedRecordsfromPlaylist);  //removing records                                           
                        context.Playlists.Remove(playlist);
                       
                        context.SaveChanges();

                        if(displayed_playlist != null)
                        {
                            if(playlist.Name == displayed_playlist.Name)
                            {
                                DestroyImages();
                                displayed_playlist = null;
                            }

                        }
                        UpdatePlaylistView();
                    }
                }
            }
        }

        private Video findVideo(string video_id)
        {
            return videos[video_id];
        }
    }
}


