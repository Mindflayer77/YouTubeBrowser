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
using YoutubeBrowser.Utility;




namespace YoutubeBrowser
{
    /// <summary>
    /// The main window of the application.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// YouTube API service used for fetching videos
        /// </summary>
        public readonly IYoutubeApi apiService;
        /// <summary>
        /// Currently listed videos
        /// </summary>
        public Dictionary<string, Video> videos = [];
        /// <summary>
        /// Currently displayed video
        /// </summary>
        public Video? displayed_video = null;
        /// <summary>
        /// Database context factory instance for easy access to the database
        /// </summary>
        public readonly YoutubeBrowserDbContextFactory factory;
        /// <summary>
        /// Determines if the stored playlists are currently displayed
        /// </summary>
        public bool playlistsDisplayed = false;
        /// <summary>
        /// Stores the information about currently displayed playlist
        /// </summary>
        public Playlist? displayed_playlist = null;
        /// <summary>
        /// ScrollViewer used for displaying playlists.
        /// </summary>
        public ScrollViewer scrollViewer;

        /// <summary>
        /// Last searched query
        /// </summary>
        public string search_text = "";

        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            apiService = new YoutubeApiService();
            factory = new YoutubeBrowserDbContextFactory();
            videos = [];
        }

        /// <summary>
        /// Utility function to remove all playlists from database.
        /// </summary>
        /// <param name="sender">The object that raises the event.</param>
        /// <param name="e">The event arguments.</param>
        public void Clear_Playlists_Click(object sender, RoutedEventArgs e)
        {
            using (var context = factory.CreateDbContext([]))
            {
                foreach (var item in context.Playlists)
                {
                    context.Playlists.Remove(item);
                }
                context.SaveChanges();
                displayed_playlist = null;
            }
            if (playlistsDisplayed)
            {
                UpdatePlaylistView();
                DestroyImages();
            }
        }

        /// <summary>
        /// Search for videos based on the provided search query.
        /// Function is invoked after user clicks on the Search button.
        /// </summary>
        /// <param name="sender">The object that raises the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void Click_Search(object sender, RoutedEventArgs e)
        {
            search_text = textBox.Text;
            if (search_text == "")
            {
                Messages.showMessageBox("Please enter your search request", "Empty search", MessageBoxButton.OK);
                return;
            }
            Browser_Test.Visibility = Visibility.Visible;
           
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

        /// <summary>
        /// Retrieves the embed address for a YouTube video based on its ID.
        /// </summary>
        /// <param name="youtube_Id">The ID of the YouTube video.</param>
        /// <returns>The embed address for the YouTube video.</returns>
        public static string GetEmbedAddress(string youtube_Id)
        {
            return "https://www.youtube.com/embed/" + youtube_Id;
        }

        /// <summary>
        /// For adding the currently displayed video to a playlist. After clicking, the AddVideoWindow is opening.
        /// Method is invoked after the user clicks on the Add button.
        /// </summary>
        /// <param name="sender">The object that raises the event.</param>
        /// <param name="e">The event arguments.</param>
        public void Add_Click(object sender, RoutedEventArgs e)
        {   
            if (displayed_video == null)
            {
                Messages.showMessageBox("No videos are currently displayed", "Cannot add video", MessageBoxButton.OK);
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

        /// <summary>
        /// Updates the list of videos in the displayed playlist.
        /// The method is invoked when the user clicks on a button representing different Playlist.
        /// It is also invoked when there was a video added to or deleted from a playlist.
        /// </summary>
        public void Update_Playlist_Videos()
        {
            using (var context = factory.CreateDbContext([]))
            {
                if (displayed_playlist == null)
                {
                    return;
                }
                videos.Clear();
                DestroyImages();
                var playlist = context.Playlists.Where(p => p.Name == displayed_playlist.Name).Include(p => p.Videos).First();
                foreach (var video in playlist.Videos.ToList())
                {
                    videos[video.YoutubeId] = video;
                }

            }
            DisplayImages();
        }

        /// <summary>
        /// Updates the displayed playlists.
        /// </summary>
        public void UpdatePlaylistView()
        {
            RemovePlaylistScrollViewer();
            Create_Playlist_ScrollViewer();
        }

        /// <summary>
        /// A handler for an event which occurs when the user clicks on an image representing a video.
        /// This method retrieves the youtube id from the sender and displayes the desired video in the main frame.
        /// </summary>
        /// <param name="sender">The object that raises the event.</param>
        /// <param name="e">The event arguments.</param>
        public void Image_Click(object sender, RoutedEventArgs e)
        {
            string video_id = ((Image)sender).Tag.ToString();
            Browser_Test.Address = GetEmbedAddress(video_id);
            displayed_video = findVideo(video_id);
        }

        /// <summary>
        /// Displays images representing fetched videos.
        /// This method dynamically creates objects for every video and binds appropriate events to them.
        /// </summary>
        public void DisplayImages()
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

        /// <summary>
        /// Clears the panel displaying images.
        /// this method is invoked whenever there was and update to the displayed videos.
        /// </summary>
        public void DestroyImages()
        {
            videosPanel.Children.Clear();
        }

        /// <summary>
        /// Event handler for right-clicking on an image representing a video. Remove video from playlist after right click.
        /// </summary>
        /// <param name="sender">The object that raises the event.</param>
        /// <param name="e">The event arguments.</param>
        public void Image_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (displayed_playlist == null)
                return;
            string video_id = ((Image)sender).Tag.ToString();

            Remove_video_from_displayed_playlist(video_id);
            Update_Playlist_Videos();
        }

        /// <summary>
        /// Remove video from a displayed playlist.
        /// </summary>
        /// <param name="videoId">Video Id to remove</param>
        public void Remove_video_from_displayed_playlist(string videoId)
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
        
        /// <summary>
        /// Event handler for clicking on the "Your Playlists" button.
        /// When this method is invoked for the first time, a ScrollViewer object with all stored playlists is displayed.
        /// When invoked for the second time, it destroys the ScrollViever, hiding all playlists.
        /// </summary>
        /// <param name="sender">The object that raises the event.</param>
        /// <param name="e">The event arguments.</param>
        public void YourPlaylists_Click(object sender, RoutedEventArgs e)
        {
            if(playlistsDisplayed)
            {
                RemovePlaylistScrollViewer();
                playlistsDisplayed = false;
                return;
            }
            Create_Playlist_ScrollViewer();
        }

        /// <summary>
        /// Creates a ScrollViewer for displaying playlists.
        /// </summary>
        public async void Create_Playlist_ScrollViewer()
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

        /// <summary>
        /// Remove  ScrollViewer for displayed playlists
        /// </summary>
        public void RemovePlaylistScrollViewer()
        {
            Central_Grid.Children.Remove(scrollViewer);
            Central_Grid.ColumnDefinitions.RemoveRange(Central_Grid.ColumnDefinitions.Count - 2, 2);
        }

        /// <summary>
        /// Creates a button for a playlist.
        /// This method dynamically creates object for a playlist and binds appropriate events to it.
        /// </summary>
        /// <param name="name">The name of the playlist.</param>
        /// <param name="width">The width of the button.</param>
        /// <returns>The created button.</returns>
        public Button Create_Playlist_Button(string name, GridLength width)
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

        /// <summary>
        /// Event handler for clicking on a playlist button.
        /// When this method is invoked, all the videos from the playlist are listed in a column.
        /// </summary>
        /// <param name="sender">The object that raises the event.</param>
        /// <param name="e">The event arguments.</param>
        public void Playlist_Button_Click(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Remove Playlist and PlaylistVideo object after right clicking on a button representing given playlist.
        /// If the deleted playlist has just been displayed, all the associated videos are also deleted 
        /// and the column representing videos is cleared.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Playlist_Button_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            string playlist_name = ((Button)sender).Content.ToString();


            MessageBoxResult result1 = MessageBox.Show($"Do you want to delete the playlist '{playlist_name}'?", "Delete playlist", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result1 == MessageBoxResult.OK)
            {
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

                            if (displayed_playlist != null)
                            {
                                if (playlist.Name == displayed_playlist.Name)
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
        }

        /// <summary>
        /// Finds a video by its ID.
        /// </summary>
        /// <param name="video_id">The ID of the video to find.</param>
        /// <returns>The found video.</returns>
        public Video findVideo(string video_id)
        {
            return videos[video_id];
        }
    }
}


