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


namespace Api
{
    public class ApiClass
    {
        //function returns list of links from search
        public async Task<List<string>> GetYouTubeVideos(string search)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBouPLVVOcmx2kdxm_o-j3anRs-MGUH558",
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = search; // Replace with your search term.
            searchListRequest.MaxResults = 50;
            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();
            List<string> videos = new List<string>();
            
            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            var address = "https://www.youtube.com/embed/";
            foreach (var searchResult in searchListResponse.Items)
            {
                videos.Add(string.Format("{0}{1}", address, searchResult.Id.VideoId));
            
            }
            
            return videos;
        }


        //function returns list of links from search
        public async Task<List<string>> GetYouTubeVideosTitle(string search)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBouPLVVOcmx2kdxm_o-j3anRs-MGUH558",
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = search; // Replace with your search term.
            searchListRequest.MaxResults = 50;
            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();
           
            List<string> titles = new List<string>();
            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            var address = "https://www.youtube.com/embed/";
            foreach (var searchResult in searchListResponse.Items)
            {
                
                titles.Add(string.Format("{0}{1}", searchResult.Snippet.Title));
            }

            return titles;
        }

        //function returns list of channels from search
        public async Task<List<string>> GetYouTubeChannels()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBouPLVVOcmx2kdxm_o-j3anRs-MGUH558",
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = "Pizza"; // Replace with your search term.
            searchListRequest.MaxResults = 50;
            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();
            
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();
            // Add each result to the appropriate list, and then display the lists of channels
            foreach (var searchResult in searchListResponse.Items)
            {
                channels.Add(string.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
            }

            return channels;
        }

        public async Task<List<string>> GetYouTubePlaylist()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBouPLVVOcmx2kdxm_o-j3anRs-MGUH558",
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = "Pizza"; // Replace with your search term.
            searchListRequest.MaxResults = 50;
            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();
           
            List<string> playlists = new List<string>();
            // Add each result to the appropriate list, and then display the lists of playlists.

            foreach (var searchResult in searchListResponse.Items)
            {
                playlists.Add(string.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
            }

            return playlists;
        }
    }
}
