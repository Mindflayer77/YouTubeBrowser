using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using YoutubeBrowser.Models;


namespace YoutubeBrowser.ApiService
{
    public interface IYoutubeApi
    {
        public Task<List<Video>> GetVideos(string search, int video_n);
    }
}
