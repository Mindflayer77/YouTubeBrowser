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

using Api;

namespace YouTubeBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApiClass api;
        public string search_text = "";
        private List<string> displayedVideos;
        int currentVideoIndex;
        public MainWindow()
        {
            InitializeComponent();
            api = new ApiClass();

            //api.GetYouTubeVideos().GetAwaiter().GetResult();
            //Console.ReadLine();

        }
        private void pnlMainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
           // MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        private async void File_Click_1(object sender, RoutedEventArgs e)
        {
            //var videos = await api.GetYouTubeChannels();
            // textBox.Text = videos.Count.ToString();
            search_text = textBox.Text;
            //var videos = await api.GetYouTubeVideos(search_text);
            //textBoxSearch.Text = videos[0];
        }

        private void linkListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void Click_Search(object sender, RoutedEventArgs e)
        {
            search_text = textBox.Text;
            var videos = await api.GetYouTubeVideos(search_text);
            Browser_Test.Address = videos[0];
            displayedVideos = videos;
            currentVideoIndex = 0;
            //textBoxSearch.Text = videos[0];

        }
        private void DisplayCurrentVideo()
        {
            if (displayedVideos != null && displayedVideos.Count > 0 && currentVideoIndex < displayedVideos.Count)
            {
                Browser_Test.Address = displayedVideos[currentVideoIndex];
            }
        }
        private async void Click_Next(object sender, RoutedEventArgs e)
        {
            if (displayedVideos != null && displayedVideos.Count > 0)
            {
                currentVideoIndex++;
                if (currentVideoIndex >= displayedVideos.Count)
                {
                    currentVideoIndex = 0;
                }
                DisplayCurrentVideo();
            }
        }
        private async void Click_Previous(object sender, RoutedEventArgs e)
        {
            if (displayedVideos != null && displayedVideos.Count > 0)
            {
                currentVideoIndex--;
                if (currentVideoIndex <= 0)
                {
                    currentVideoIndex = 0;
                }
                DisplayCurrentVideo();
            }
        }
        private async void Click_add_to_playlist(object sender, RoutedEventArgs e)
        {
        }
    }
}
