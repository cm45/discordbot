using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Services;
using DiscordBot.Services.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Victoria;
using Victoria.Enums;

namespace DiscordBot.Modules
{
    [Group("music"), Alias("m"), Name("Music")]
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        public MusicService MusicService { get; set; }

        private IVoiceChannel VoiceChannel => (Context.User as IGuildUser).VoiceChannel;

        [Command, Alias("info", "current", "track", "song", "information", "playing"), Summary("Gets info about the current Track, Volume & Queue as well as a command help list?!")]
        public async Task Info()
        {
            var track = await MusicService.GetCurrentTrack();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Musicbot - Info");

            embedBuilder.AddField("Status", MusicService.Player.PlayerState.ToString());

            if (MusicService.Player.PlayerState == PlayerState.Playing)
                embedBuilder.AddField("Currently Playing", track.Title);

            embedBuilder.AddField("Current Voicechannel", MusicService.Player.VoiceChannel.Name ?? "None");
            embedBuilder.AddField("Volume", MusicService.Player.Volume.ToString() ?? Config.ConfigCache.Volume.ToString());

            await ReplyAsync(embed: embedBuilder.Build());
        }

        #region Connection
        [Command("join"), Alias("summon", "connect")]
        public async Task Join(IVoiceChannel voiceChannel = null)
        {
            await MusicService.JoinAsync(voiceChannel ?? VoiceChannel, Context.Channel as ITextChannel, true);
            await ReplyAsync($"Joined {voiceChannel} channel!");
        }

        [Command("leave"), Alias("disconnect")]
        public async Task Leave()
        {
            var currentChannelString = MusicService.Player.VoiceChannel.ToString();
            await MusicService.LeaveAsync(VoiceChannel);
            await ReplyAsync($"Left {currentChannelString}!");
        }
        #endregion

        #region Playback
        [Command("play")]
        public async Task Play([Remainder] string query)
        {
            if (MusicService.Player == null)
                await MusicService.JoinAsync(VoiceChannel, Context.Channel as ITextChannel);

            try
            {
                var tracks = await MusicService.GetTracksAsync(query);

                if (tracks.Count() == 1)
                    await MusicService.PlayAsync(tracks.First());
                else
                    await MusicService.PlayAsync(tracks);
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed(ex.Message));
                return;
            }
        }

        [Command("resume"), Alias("r")] public async Task Resume() => await ReplyAsync(embed: await MusicService.ResumeAsync(Context.Guild));
        [Command("pause")] public async Task Pause() => await ReplyAsync(embed: await MusicService.PauseAsync(Context.Guild));
        [Command("stop"), Alias("s")] public async Task Stop() => await ReplyAsync(embed: await MusicService.StopAsync(Context.Guild));
        #endregion

        #region Volume control
        [Command("volume"), Alias("v", "vol")] public async Task Volume(ushort value) => await ReplyAsync(embed: await MusicService.SetVolumeAsync(value));
        [Command("volume"), Alias("v", "vol")] public async Task Volume() => await ReplyAsync(embed: await MusicService.GetVolumeAsEmbedMessage());
        #endregion

        #region Queue
        [Command("queue"), Alias("q", "show", "list", "all")] public async Task ShowQueueAsync() => await ReplyAsync(embed: await MusicService.GetQueueMessageEmbedAsync());
        [Command("skip"), Alias("next", "n")] public async Task SkipAsync() => await ReplyAsync(embed: await MusicService.SkipAsync());
        [Command("shuffle"), Alias("randomize", "rng")] public async Task ShuffleQueueAsync() => await ReplyAsync(embed: await MusicService.Shuffle());
        [Command("remove"), Alias("delete", "r", "d", "-")] public async Task RemoveTrackFromQueueAsync(uint queueId) => await ReplyAsync(embed: await MusicService.RemoveItemFromQueue(queueId));
        [Command("clear")]
        public async Task ClearQueueAsync()
        {
            try
            {
                await MusicService.ClearQueue();
                await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed("Cleared Queue"));
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed(ex.Message));
            }
        }
        #endregion

        #region Playlist

        [Group("playlist"), Alias("pl"), Name("Playlist")]
        public class PlaylistModule : ModuleBase<SocketCommandContext>
        {
            public PlaylistService PlaylistService { get; set; }
            public MusicService MusicService { get; set; }

            #region Play

            [Command("play")]
            public async Task PlayPlaylistAsync(int id)
            {
                Console.WriteLine("Get playlist via ID");
                var playlist = await PlaylistService.GetPlaylist(id);

                if (playlist == null)
                {
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist not found!"));
                    return;
                }

                if (playlist.Tracks.Count() <= 0)
                {
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist is empty!"));
                    return;
                }

                await MusicService.PlayPlaylistAsync(playlist);
                await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Successfully added the playlist '{playlist.Name}' with {playlist.Tracks?.Count()} tracks into the queue!"));
            }

            [Command("play")]
            public async Task PlayPlaylistAsync(string name)
            {
                Console.WriteLine("Get playlist via name");
                var playlist = await PlaylistService.GetPlaylist("name");

                if (playlist == null)
                {
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist not found!"));
                    return;
                }

                if (playlist.Tracks.Count() <= 0)
                {
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist is empty!"));
                    return;
                }

                await MusicService.PlayPlaylistAsync(playlist);
            }

            #endregion

            #region Query
            [Command, Alias("all")]
            public async Task ListAllPlaylistsAsync()
            {
                var playlists = await PlaylistService.GetPlaylists();

                if (playlists.Count() <= 0)
                {
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("No playlists found!"));
                    return;
                }

                var message = "";
                foreach (var playlist in playlists)
                    message += $"[{playlist.Id}] - {playlist.Name} ({playlist.Tracks.Count()} tracks)\n";

                await ReplyAsync(embed: CustomEmbedBuilder.BuildInfoEmbed($"Found {playlists.Count()} playlists!", message));
            }
            [Command]
            public async Task GetPlaylist(int id)
            {
                var playlist = await PlaylistService.GetPlaylist(id);

                var maxTrackviewCount = 10;

                var message = "Tracks:\n";
                for (int i = 0; i < Math.Min(playlist.Tracks.Count(), maxTrackviewCount); i++)
                    message += $"**{i+1})** {playlist.Tracks.ElementAt(i).Title}\n";

                if (playlist.Tracks.Count() > maxTrackviewCount)
                    message += $"*And {playlist.Tracks.Count() - maxTrackviewCount} more...*";

                await ReplyAsync(embed: CustomEmbedBuilder.BuildInfoEmbed($"Found playlist '{playlist.Name}'!", message.Trim('\n')));
            }
            [Command]
            public async Task GetPlaylist(string name)
            {
                var playlist = await PlaylistService.GetPlaylist(name);

                var message = "Tracks:\n";
                for (int i = 0; i < Math.Min(playlist.Tracks.Count(), 10); i++)
                    message += $"**{i + 1})** {playlist.Tracks.ElementAt(i).Title}\n";

                await ReplyAsync(embed: CustomEmbedBuilder.BuildInfoEmbed($"Found playlist '{playlist.Name}'!", message.Trim('\n')));
            } 
            #endregion

            #region Remove
            [Command("destroy"), Alias("d")]
            public async Task DestroyPlaylistAsync(int id)
            {
                if (await PlaylistService.RemovePlaylist(id))
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Removed playlist with id {id}!"));
                else
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed($"Failed to remove playlist with id {id}!"));
            }

            [Command("destroy"), Alias("d")]
            public async Task DestroyPlaylistAsync(string name)
            {
                if (await PlaylistService.RemovePlaylist(name))
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Removed playlist with name '{name}'!"));
                else
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed($"Failed to remove playlist with name '{name}'!"));
            }
            #endregion


            #region Add Track/s

            [Command("addtrack")]
            public async Task AddTracksToPlaylistAsync(string playlistName, [Remainder] string query)
            {
                var playlist = await PlaylistService.GetPlaylist(playlistName);
                var tracks = await MusicService.GetTracksAsync(query);

                await playlist.AddTracks(tracks);
            }

            [Command("addtrack")]
            public async Task AddTracksToPlaylistAsync(int playlistId, [Remainder] string query)
            {
                var playlist = await PlaylistService.GetPlaylist(playlistId);
                var tracks = await MusicService.GetTracksAsync(query);
                
                await playlist.AddTracks(tracks);
            }

            #endregion

            [Command("create")]
            public async Task CreatePlaylistAsync([Remainder] string name)
            {
                var playlist = await PlaylistService.CreatePlaylist(name);
                await ReplyAsync($"Created playlist called '{playlist.Name}'!");
            }

            [Command("save")]
            public async Task SaveQueueAsPlaylistAsync([Remainder] string playlistName)
            {
                if (MusicService.Player == null)
                {
                    await ReplyAsync(embed: CustomEmbedBuilder.NoPlayerEmbed);
                    return;
                }

                var tracks = MusicService.Player.Queue.Items.Cast<LavaTrack>().ToList();

                if (MusicService.Player.Track != null && tracks != null)
                    tracks.Add(MusicService.Player.Track);

                if (tracks.Count <= 0 && MusicService.Player.Track == null)
                {
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Current queue is empty!"));
                    return;
                }

                var playlist = new Playlist(playlistName, tracks);

                await PlaylistService.SavePlaylist(playlist);
                await ReplyAsync($"Saved current queue as playlist ({playlistName} with {playlist.Tracks.Count()} tracks)!");
            }
        }

        #endregion
    }
}
