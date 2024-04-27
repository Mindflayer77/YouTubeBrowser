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
    /// <summary>
    /// Interface for interacting with the YouTube API.
    /// </summary>
    public interface IYoutubeApi
    {

        /// <summary>
        /// Fetches a list of videos from YouTube based on the specified search query and the number of videos to retrieve.
        /// </summary>
        /// <param name="search">The search query to use when fetching videos.</param>
        /// <param name="video_n">The number of videos to retrieve.</param>
        /// <returns>The result contains a list of Video objects.</returns>

        public Task<List<Video>> GetVideos(string search, int video_n);
    }
}
