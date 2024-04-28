using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YoutubeBrowser.Models;


namespace YoutubeBrowser.ApiService
{
    /// <summary>
    /// Methods of interacting with the YouTube data API to download videos 
    /// </summary>
    public class YoutubeApiService : IYoutubeApi
    {
        /// <summary>
        /// Represents the YouTube service used for API requests.
        /// </summary>
        public readonly YouTubeService service;

        /// <summary>
        /// Initiates a new instance of the YoutubeApiService class with the default API key and application name.
        /// </summary>
        public YoutubeApiService()
        {
            service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBouPLVVOcmx2kdxm_o-j3anRs-MGUH558",
                ApplicationName = GetType().ToString()
            });
        }

        /// <summary>
        /// Parses a SearchResult object into a Video object.
        /// </summary>
        /// <param name="searchResult">Object to parse</param>
        /// <returns>Model with parsed data</returns>
        public static Models.Video ParseVideo(SearchResult searchResult)
        {
            return new Models.Video()
            {
                YoutubeId = searchResult.Id.VideoId,
                Title = searchResult.Snippet.Title,
                Thumbnail_url = searchResult.Snippet.Thumbnails.High.Url
            };
        }

        //
        /// <summary>
        ///Downloads a list of YouTube videos based on specific search criteria. 
        /// </summary>
        /// <param name="search">Search request to perform</param>
        /// <param name="video_n">The maximum number of videos to download.</param>
        /// <returns>List of videos</returns>
        public async Task<List<Models.Video>> GetVideos(string search, int video_n)
        {
            var searchListRequest = service.Search.List("snippet");
            searchListRequest.Type = "video";
            searchListRequest.VideoEmbeddable = SearchResource.ListRequest.VideoEmbeddableEnum.True__;
            searchListRequest.Q = search; // Replace with your search term.
            searchListRequest.MaxResults = video_n;
            searchListRequest.VideoLicense = SearchResource.ListRequest.VideoLicenseEnum.CreativeCommon;

            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<Models.Video> videos = [];

            foreach (var searchResult in searchListResponse.Items)
            {
                videos.Add(ParseVideo(searchResult));
            }
            return videos;
        }
    }
}
