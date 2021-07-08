using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Borgar_Bot
{
    public class YouTubeServiceClient
    {
        private static YouTubeServiceClient instance;

        public static YouTubeServiceClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new YouTubeServiceClient();
                }
                return instance;
            }
        }

        private async Task<YouTubeService> GetYouTubeService(string userEmail)
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets_Z.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[]
                    {
                YouTubeService.Scope.Youtube,
                YouTubeService.Scope.Youtubepartner,
                YouTubeService.Scope.YoutubeUpload,
                YouTubeService.Scope.YoutubepartnerChannelAudit,
                YouTubeService.Scope.YoutubeReadonly
                    },
                    userEmail,
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString()));
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            return youtubeService;
        }
    }
}
