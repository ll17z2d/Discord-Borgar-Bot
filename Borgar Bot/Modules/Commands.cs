using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using SpotifyAPI.Web;

namespace Borgar_Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("eat")]
        //[Alias("eat")]
        public async Task Ping([Remainder]string args = null)
        {
            var argsList = args.Split(' ');
            string message = "";
            foreach (var item in argsList)
            {
                if (item.Contains("monku") || item.Contains("munku") || item.Contains("monkey") || item.Contains("moko") 
                    || item.Contains("munke") || item.Contains("miku") || item.Contains("monki") || item.Contains("mokey"))
                {
                    message = "monkey eat da banana";
                    break;
                }

                else if (item.Contains("borgar") || item.Contains("borgo") || item.Contains("burgor") || item.Contains("bogo") 
                    || item.Contains("bargor") || item.Contains("birger") || item.Contains("borgo") || item.Contains("borg"))
                {
                    message = "mm borg";
                }

                else
                {
                    await ReplyAsync(GetErrorResponse());
                }
            }

            await ReplyAsync(message);
        }

        [Command("album")]
        public async Task SpotifyAlbums([Remainder] string args = null)
        {
            //args = args.ToLower();
            //args = args.Remove(0, 14);
            var argsList = args.Split(' ');
            argsList[0] = argsList[0].Remove(0, 14);

            var a = argsList.ToList();
            a.Remove(a[0]);
            var userPlaylistName = string.Join(" ", a);

            var spotify = GetSpotifyObject();
            FullAlbum songAlbum = await spotify.Albums.Get(argsList[0]);

            List<string> trackList = new List<string>();
            foreach (var item in songAlbum.Tracks.Items)
            {
                trackList.Add($"{item.Name} by {item.Artists[0].Name}");
                //trackList.Add(item.Name);
            }

            //await GetYoutubeLink(trackList);

            //string message = string.Join(Environment.NewLine, trackList.ToArray());
            //await ReplyAsync(message);

            try
            {
                var playlistName = await GetYoutubeLink(trackList, userPlaylistName);

                await ReplyAsync("Please wait for the album...");

                Thread.Sleep(10000);

                await ReplyAsync("https://www.youtube.com/playlist?list=" + $"{playlistName}");
            }
            catch
            {
                await ReplyAsync("OH NO! Youtube off the goop again with their limits so no more playlist making today :(");
            }
        }

        [Command("playlist")]
        public async Task SpotifyTracks([Remainder] string args = null)
        {
            //args = args.ToLower();
            //args = args.Remove(0, 17);
            var argsList = args.Split(' ');
            argsList[0] = argsList[0].Remove(0, 17);

            var a = argsList.ToList();
            a.Remove(a[0]);
            var userPlaylistName = string.Join(" ", a);

            //x.ToString();
            //new StringBuilder(x).ToString();
            
            //var userPlaylistName = argsList[1];

            var spotify = GetSpotifyObject();
            FullPlaylist songPlaylist = await spotify.Playlists.Get(argsList[0]);

            List<string> trackList = new List<string>();
            foreach (var item in songPlaylist.Tracks.Items)
            {
                FullTrack track = (FullTrack)item.Track;
                trackList.Add($"{track.Name} by {track.Artists[0].Name}");
            }

            await ReplyAsync("Please wait for the playlist...");

            var playlistName = await GetYoutubeLink(trackList, userPlaylistName);

            Thread.Sleep(10000);

            await ReplyAsync("https://www.youtube.com/playlist?list=" + $"{playlistName}");

            await ReplyAsync("OH NO! Youtube off the goop again with their limits so no more playlist making today :(");
            

        }

        [Command("sus")]
        public async Task test()
        {
            await ReplyAsync(">:(");
        }

        [Command("amogus")]
        public async Task test2()
        {
            await ReplyAsync(">>>>>>:(");
        }

        [Command("rage")]
        public async Task test3()
        {
            await ReplyAsync("Man's on a\n\n\n\n\n\n\n\n\n\nRAGE");
        }

        [Command("help")]
        public async Task GetHelp([Remainder] string args = null)
        {
            var message = $"Hi, I'm Borgar Bot, or BB for short. I've been made to convert Spotify playlists and albums to Youtube playlists, which can be used to get quickly playable links for use with Rythm Bot, or to store as an archive. These Youtube playlists are public playlists that go under Zak's account and there's also a daily limit to how much Google will let me use their services so try not to spam-use me! There's a short wait between calling me to make the playlist, and me sending you thie link so bear with me during those troubling times\n" +
                $"\n" +
                $"Here are some of the services I can do for you:\n" +
                $"-eat            : You can request for a monkey to eat a banana, or just eat a good old fashioned burger\n" +
                $"                   Example: -eat banana for moki\n" +
                $"                                     -eat borgo\n" +
                $"-playlist     : Paste the Spotify URL of a playlist, followed by the name you want to call the Youtube playlist\n" +
                $"                   Example: -playlist spotify:playlist:37i9dQZF1DX2vYju3i0lNX Champion Playlist\n" +
                $"-album       : Paste the Spotify URL of an album, followed by the name you want to call the Youtube playlist\n" +
                $"                   Example: -album spotify:album:20r762YmB5HeofjMCiPMLv My MBDTF Playlist\n" +
                $"\n" +
                $"If you have any questions, quandaries or quality features to recommend, then let Zak know :)";

            await ReplyAsync(message);
        }

        private async Task<string> GetYoutubeLink(List<string> trackList, string userPlaylistName)
        {
            //notasecret

            var keyPath = YOUTUBEKEYPATH;
            //var accountEmail = @"imgr8tness@gmail.com";
            var accountEmail = GMAILSERVICEACCOUNT;

            //new YoutubeCommunication(keyPath, accountEmail).GetData();
            //await new YouTubeServiceClient().GetYoutubeService

            var youtubeService = new YoutubeCommunication(keyPath, accountEmail);
            await youtubeService.Run();

            youtubeService.CreateYoutubePlaylist(trackList, userPlaylistName);

            //Thread.Sleep(6000);

            return youtubeService.playlistName;
        }

        private SpotifyClient GetTempSpotifyObject() //PERM TOKEN
            => new SpotifyClient(PERMSPOTIFYTOKEN);       
        


        private SpotifyClient GetSpotifyObject()
            => new SpotifyClient(TEMPSPOTIFYTOKEN);

        private string GetErrorResponse()
        {
            var random = new Random();
            var possibleResponse = new List<string>() { "Not sure what you mean there buddy", "Bro make your request clearer", "b-burger...?", 
                "No more monkey time, because of you" };

            return possibleResponse[random.Next(possibleResponse.Count)];
        }
    }
}
