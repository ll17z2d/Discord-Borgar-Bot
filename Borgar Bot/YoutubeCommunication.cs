using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Upload;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Borgar_Bot
{
    public class YoutubeCommunication
    {
        //YouTubeService youTubeService = new YouTubeService();
        public YouTubeService youtubeService { get; set; }

        public string playlistName { get; set; }

        public YoutubeCommunication(string keyPath, string accountEmail)
        {
            //var certificate = new X509Certificate2(keyPath,
            //    "notasecret", X509KeyStorageFlags.Exportable); //I think this uses a simple api key?

            //var credentials = new ServiceAccountCredential(
            //    new ServiceAccountCredential.Initializer(accountEmail)
            //    {
            //        Scopes = new[]
            //            {YouTubeService.Scope.YoutubeForceSsl
            //        }
            //    }.FromCertificate(certificate));

            ////YouTubeService.Scope.Youtube,
            ////            YouTubeService.Scope.YoutubeUpload,
            ////            YouTubeService.Scope.Youtubepartner,
            ////            YouTubeService.Scope.YoutubeReadonly

            //youtubeService = new YouTubeService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credentials,
            //    ApiKey = "APIKey"
            //});
        }

        public async Task Run()
        {
            UserCredential credential;
            using (var stream = new FileStream(ClientSecretPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows for full read/write access to the
                    // authenticated user's account.
                    new[] {
                        YouTubeService.Scope.YoutubeForceSsl
                    },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });


        }

        public void CreateYoutubePlaylist(List<string> trackList, string userPlaylistName)
        {
            // Create a new, private playlist in the authorized user's channel.
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = $"{userPlaylistName} {DateTime.Today.Date.ToShortDateString()}";
            newPlaylist.Snippet.Description = "A playlist created using the power of Borgar Bot :)";
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = "public";
            newPlaylist = youtubeService.Playlists.Insert(newPlaylist, "snippet,status").Execute();

            playlistName = newPlaylist.Id;

            AddToYoutubePlaylist(newPlaylist, trackList);
            
        }

        public void AddToYoutubePlaylist(Playlist newPlaylist, List<string> trackList)
        {
            foreach(var track in trackList)
            {
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = $"{track}";
                searchListRequest.MaxResults = 1;
                var searchListResponse = searchListRequest.Execute();

                Thread.Sleep(50);

                var newPlaylistItem = new PlaylistItem();
                newPlaylistItem.Snippet = new PlaylistItemSnippet();
                newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
                newPlaylistItem.Snippet.ResourceId = new ResourceId();
                newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                newPlaylistItem.Snippet.ResourceId.VideoId = searchListResponse.Items[0].Id.VideoId;
                newPlaylistItem = youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").Execute();

                Thread.Sleep(50);
            }
        }
    }
}
