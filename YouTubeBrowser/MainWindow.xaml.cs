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

        public MainWindow()
        {
            InitializeComponent();
            api = new ApiClass();
            //api.GetYouTubeVideos().GetAwaiter().GetResult();
            //Console.ReadLine();

        }
        private void pnlMainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        private async void File_Click_1(object sender, RoutedEventArgs e)
        {
            var videos = await api.GetYouTubeVideos();
            textBox.Text = videos.Count.ToString();
        }

        private void linkListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
