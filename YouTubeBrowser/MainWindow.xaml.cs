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
        private static string GetEmbedAddress(string youtube_Id)
        {
            return "https://www.youtube.com/embed/" + youtube_Id;
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            string video_id = ((Image)sender).Tag.ToString();
            Browser_Test.Address = GetEmbedAddress(video_id);
            displayed_video = findVideo(video_id);
        }

        private void DestroyImages()
        {
            videosPanel.Children.Clear();
        }


        private void DisplayImages()
        {
            foreach(KeyValuePair<string, Video> video in videos)
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
                panel.Children.Add(image);
                videosPanel.Children.Add(panel);
            }
        }

        private void Clear_Videos_Click(object sender, RoutedEventArgs e)
        {
            DbUtility.ClearVideos();
        }

        private void Clear_Playlists_Click(object sender, RoutedEventArgs e)
        {
            DbUtility.ClearPlaylists();
        }

        private Video findVideo(string video_id)
        {
            return videos[video_id];
        }

        private async void Click_Search(object sender, RoutedEventArgs e)
        {
            Browser_Test.Visibility = Visibility.Visible;
            //RemovePlaylistScrollViewer();
            search_text = textBox.Text;
            videos.Clear();
            List<Video> tmp_videos = [];
            try
            {
                tmp_videos = await apiService.GetVideos(search_text, 10);
            }
            catch(Exception ex)
            {
                Messages.showMessageBox("Cannot execute request", "Daily Youtube quota exceeded", MessageBoxButton.OK);
                return;
            }
            foreach(var video in tmp_videos)
            {
                videos[video.YoutubeId] = video;
            }
            DestroyImages();
            if(videos.Count == 0) 
            {
                Messages.showMessageBox("Empty result", "No videos found", MessageBoxButton.OK);
                displayed_video = null;
                return;
            }
            DisplayImages();
            Browser_Test.Address = GetEmbedAddress(videos.ElementAt(0).Value.YoutubeId);
            displayed_video = videos.ElementAt(0).Value;
        }

        private void Update_Playlist()
        {
            //DestroyImages();
            //using(var context = factory.CreateDbContext([]))
            //{
                
            //}

            // TODO: Implement a function which updates the diplayed videos from playlist if one video was deleted
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(displayed_video == null) 
            {
                Messages.showMessageBox( "No videos are currently displayed", "Cannot add video", MessageBoxButton.OK);
                return;
            }
            var window = new AddVideoWindow(displayed_video);
            window.Show();
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
        }

        private async void YourPlaylists_Click(object sender, RoutedEventArgs e)
        {
            if(playlistsDisplayed)
            {
                Central_Grid.Children.Remove(scrollViewer);
                Central_Grid.ColumnDefinitions.RemoveRange(Central_Grid.ColumnDefinitions.Count - 2, 2);
                playlistsDisplayed = false;
                return;
            }
            ColumnDefinition separatorColumn = new ColumnDefinition();
            separatorColumn.Width = new GridLength(10);
            Central_Grid.ColumnDefinitions.Add(separatorColumn);
            ColumnDefinition playlistColumn = new ColumnDefinition();
            playlistColumn.Width = new GridLength(150);
            Central_Grid.ColumnDefinitions.Add(playlistColumn);

            //new ScrollViewer for playlists
            ScrollViewer playlistScrollViewer = new ScrollViewer();
            playlistScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            playlistScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            playlistScrollViewer.HorizontalAlignment = HorizontalAlignment.Left;

            
            StackPanel playlistStackPanel = new StackPanel();
            playlistScrollViewer.Content = playlistStackPanel;

            //load from DB to button
            using (var context = factory.CreateDbContext([]))
            {
                var playlists = await context.Playlists.ToListAsync();

                foreach (var playlist in playlists)
                {
                    var newButton = Create_Playlist_Button(playlist.Name, playlistColumn.Width);
                    playlistStackPanel.Children.Add(newButton);
                }
            }
        

            // new scrollViewer to Grid
            Central_Grid.Children.Add(playlistScrollViewer);
            Grid.SetColumn(playlistScrollViewer, Central_Grid.ColumnDefinitions.Count - 1);
            Grid.SetRow(playlistScrollViewer, 2);
            playlistsDisplayed = true;
            scrollViewer = playlistScrollViewer;
        }

        //function to delete ScrollViewer with playlists
        private void RemovePlaylistScrollViewer()
        {
            
            foreach (UIElement child in pnlMainGrid.Children)
            {
                if (child is ScrollViewer)
                {
                    pnlMainGrid.Children.Remove(child);
                    break; 
                }
            }
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

        private Button Create_Playlist_Button(string name, GridLength width)
        {
            Button newButton = new Button();
            newButton.Content = name;
            newButton.Width = width.Value;
            newButton.Height = 25;
            newButton.Click += new RoutedEventHandler(Playlist_Button_Click);
            return newButton;
        }
        private void Show_Click(object sender, RoutedEventArgs e)
        {
            List<Video> dBVideos;
            using (YoutubeBrowserContext dbContext = factory.CreateDbContext([]))
            {
                dBVideos = dbContext.Videos.ToList<Video>();
            }
            if(dBVideos.Count() == 0) { textBox.Text = "No Videos to display"; }
            else { textBox.Text = dBVideos[0].Title + dBVideos.Count().ToString(); }
        }

    }
}
