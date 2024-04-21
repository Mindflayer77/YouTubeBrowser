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
        private Video displayed_video = new();
        private readonly YoutubeBrowserDbContextFactory factory;

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
            search_text = textBox.Text;
            videos.Clear();
            List<Video> tmp_videos = [];
            try
            {
                tmp_videos = await apiService.GetVideos(search_text, 10);
            }
            catch(Exception ex)
            {
                Messages.showMessageBox(ex.ToString(), "Cannot execute request", MessageBoxButton.OK);
                return;
            }
            foreach(var video in tmp_videos)
            {
                videos[video.YoutubeId] = video;
            }
            DestroyImages();
            DisplayImages();
            Browser_Test.Address = GetEmbedAddress(videos.ElementAt(0).Value.YoutubeId);
            displayed_video = videos.ElementAt(0).Value;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window1(displayed_video);
            window.Show();
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
