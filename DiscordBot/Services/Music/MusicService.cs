using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Interfaces;

namespace DiscordBot.Services.Music
{
    public class MusicService
    {
        public static Embed NoPlayerEmbed = CustomEmbedBuilder.BuildErrorEmbed("No player found!");
        public static Exception PlayerNotFoundException = new Exception("Player not found!");

        public DiscordSocketClient Client { get; private set; }
        public LavaConfig LavaConfig { get; private set; }
        public LavaNode LavaNode { get; private set; }
        public LavaPlayer Player { get; private set; }
        public PlaylistService PlaylistService { get; private set; }

        public MusicService(DiscordSocketClient client, LavaConfig lavaConfig, LavaNode lavaNode)
        {
            Client = client;
            LavaConfig = lavaConfig;
            LavaNode = lavaNode;
        }
        public Task InitializeAsync()
        {
            Client.Ready += Client_Ready;
            Client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;

            // TODO: Add lavasocketclient events
            LavaNode.OnTrackEnded += LavaNode_OnTrackEnded;
            LavaNode.OnPlayerUpdated += LavaNode_OnPlayerUpdated;

            return Task.CompletedTask;
        }

        #region Event-Methods
        private Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            Console.WriteLine($"{user} moved from {oldVoiceState.VoiceChannel} to {newVoiceState.VoiceChannel}");
            return Task.CompletedTask;
        }
        private Task Client_Ready()
        {
            return Task.CompletedTask;
        }
        private async Task LavaNode_OnTrackEnded(Victoria.EventArgs.TrackEndedEventArgs args)
        {
            Console.WriteLine("Ended track!");

            if (!args.Reason.ShouldPlayNext())
                return;

            if (!args.Player.Queue.TryDequeue(out var track))
            {
                await args.Player.TextChannel.SendMessageAsync("No more tracks to play.");
                return;
            }

            await args.Player.PlayAsync((LavaTrack)track);
        }
        private async Task LavaNode_OnPlayerUpdated(Victoria.EventArgs.PlayerUpdateEventArgs arg)
        {
            if (arg.Player.PlayerState == PlayerState.Playing)
                await Client.SetGameAsync(Player.Track.Title, Player.Track.Url);
            else
                await Client.SetActivityAsync(null);
        }
        #endregion

        #region Connection-handling commands
        public async Task JoinAsync(IVoiceChannel voiceChannel, ITextChannel textChannel, bool playOnJoinTrack = false)
        {
            Player = await LavaNode.JoinAsync(voiceChannel, textChannel);
            await Player.UpdateVolumeAsync(Config.ConfigCache.Volume);

            // Play On Join Song
            //if (!string.IsNullOrWhiteSpace(Config.ConfigCache.OnJoin) && playOnJoinTrack)
            //{
            //    await PlayAsync(Config.ConfigCache.OnJoin, true);
            //}
        }
        public async Task LeaveAsync(IVoiceChannel voiceChannel) => await LavaNode.LeaveAsync(voiceChannel);
        #endregion

        #region Volume commands
        public async Task<Embed> SetVolumeAsync(ushort value)
        {
            if (Player == null)
                return NoPlayerEmbed;

            int oldVolume = GetVolume();
            value = (ushort)Math.Clamp(value, 0U, 10U); // TODO: Replace magic numbers

            await Player.UpdateVolumeAsync(value);

            var config = Config.ConfigCache;
            config.Volume = value;
            await Config.SaveConfigAsync(config);

            return CustomEmbedBuilder.BuildSuccessEmbed("", $"Set volume from {oldVolume} to {value}");
        }
        public int GetVolume()
        {
            if (Player == null)
                return -1;

            return Player.Volume;
        }
        public Task<Embed> GetVolumeAsEmbedMessage()
        {
            var playerVolume = GetVolume();
            var volumeString = playerVolume == -1 ? Config.ConfigCache.Volume.ToString() : playerVolume.ToString();
            return Task.FromResult(CustomEmbedBuilder.BuildInfoEmbed("", $"Current volume is {volumeString}!"));
        }
        #endregion

        #region Playback commands
        public async Task<Embed> PlayPlaylistAsync(Playlist playlist)
        {
            var trackCount = await PlayAsync(playlist.Tracks, false);
            return CustomEmbedBuilder.BuildSuccessEmbed($"Added {trackCount} tracks from playlist '{playlist.Name}' to the queue!");
        }

        public async Task<IEnumerable<LavaTrack>> GetTracksAsync(string query)
        {
            var search = await LavaNode.SearchAsync(query);

            switch (search.LoadStatus)
            {
                case LoadStatus.NoMatches:
                    throw new Exception("No matches found!");
                case LoadStatus.LoadFailed:
                    throw new Exception("Load failed!");
                default:
                    break;
            }

            return search.Tracks;
        }

        /// <summary>
        /// Plays the first track of a collection while queueing the rest.
        /// </summary>
        /// <returns>Amount of tracks</returns>
        public async Task<int> PlayAsync(IEnumerable<LavaTrack> tracks, bool forcePlay = false)
        {
            if (Player == null)
                throw PlayerNotFoundException;

            if (tracks == null || tracks.Count() <= 0)
                throw new Exception("Tracks are either 'null' or empty!");

            await Player.PlayAsync(tracks.First());

            for (int i = 1; i < tracks.Count(); i++)
                Player.Queue.Enqueue(tracks.ElementAt(i));

            return tracks.Count();
        }


        public async Task PlayAsync(LavaTrack track, bool forcePlay = false)
        {
            if (Player == null)
                throw PlayerNotFoundException;

            if (track == null)
                throw new Exception("Tracks are either 'null' or empty!");

            await Player.PlayAsync(track);
        }

        public async Task<Embed> ResumeAsync(SocketGuild guild)
        {
            if (Player == null)
                return NoPlayerEmbed;

            if (Player.PlayerState == PlayerState.Stopped && Player.Queue.Count > 0)
            {
                LavaTrack track = (LavaTrack)Player.Queue.Items.FirstOrDefault();
                await Player.PlayAsync(track);
                Player.Queue.Remove(track);
                return CustomEmbedBuilder.BuildSuccessEmbed($"Resuming with {track.Title}");
            }
            else
            {
                await Player.ResumeAsync();
                return CustomEmbedBuilder.BuildSuccessEmbed($"Resumed playback of '{Player.Track.Title}'!");
            }
        }
        public async Task<Embed> PauseAsync(SocketGuild guild)
        {
            if (Player == null)
                return NoPlayerEmbed;

            await Player.PauseAsync();
            return CustomEmbedBuilder.BuildSuccessEmbed($"Paused playback of '{Player.Track.Title}'!");
        }
        public async Task<Embed> StopAsync(SocketGuild guild)
        {
            if (Player == null)
                return NoPlayerEmbed;

            await Player.StopAsync();
            await Client.SetActivityAsync(null);

            return CustomEmbedBuilder.BuildSuccessEmbed($"Stopped playback of '{Player.Track.Title}'!");
        }
        public async Task<Embed> SkipAsync()
        {
            if (Player == null)
                return NoPlayerEmbed;

            var track = Player.Track;
            await Player.SkipAsync();
            return CustomEmbedBuilder.BuildSuccessEmbed("", $"Skipped '**{track.Title}**'!");
        }
        public Task<LavaTrack> GetCurrentTrack()
        {
            if (Player != null)
                return Task.FromResult(Player.Track);

            return null;
        }
        #endregion

        #region Queue commands
        public async Task<Embed> GetQueueMessageEmbedAsync()
        {
            if (Player == null)
                return NoPlayerEmbed;

            var currentTrack = await GetCurrentTrack();

            var currentPosition = currentTrack != null ? new TimeSpan(currentTrack.Position.Hours, currentTrack.Position.Minutes, currentTrack.Position.Seconds) : new TimeSpan();

            var musicalNote = ":musical_note:";
            var embedBuilder = new EmbedBuilder
            {
                Title = "Track Queue",
                Description = currentTrack != null ? $"{musicalNote} Currently playing {currentTrack.Title} **[{currentPosition}/{currentTrack.Duration}]**!\n" : ""
            };

            var queue = Player.Queue;

            if (queue.Items != null)
            {
                int tracksPerPage = 10;
                int index = 0;
                foreach (var item in queue.Items)
                {
                    var track = item as LavaTrack;

                    embedBuilder.Description += $"\n**{++index})** {track?.Title} **[{track?.Duration}]**";
                    if (index >= tracksPerPage)
                    {
                        embedBuilder.Description += $"\nAnd {queue.Count - index} more...";
                        break;
                    }
                }
                return embedBuilder.Build();

                // TODO: Implement pagescroller through reactions
            }
            else
            {
                embedBuilder.Description += "queue.Items is null!";
                return embedBuilder.Build();
            }
        }
        public async Task<Embed> AddTrackToQueueAsync(LavaTrack track)
        {
            if (Player == null)
                return NoPlayerEmbed;

            Player.Queue.Enqueue(track);

            return await CustomEmbedBuilder.BuildTrackEmbedAsync(track);
        }
        public Task<Embed> RemoveItemFromQueue(uint queueId)
        {
            if (Player == null)
                return Task.FromResult(NoPlayerEmbed);

            if (queueId + 1 > Player.Queue.Count)
                return Task.FromResult(CustomEmbedBuilder.BuildErrorEmbed("Index/ID out of bounds"));

            var item = (LavaTrack)Player.Queue.RemoveAt((int)queueId);
            return Task.FromResult(CustomEmbedBuilder.BuildSuccessEmbed("", $"Removed '{item.Title}' from queue!"));
        }
        public Task<Embed> Shuffle()
        {
            if (Player == null)
                return Task.FromResult(NoPlayerEmbed);

            Player.Queue.Shuffle();
            return Task.FromResult(CustomEmbedBuilder.BuildSuccessEmbed("", "Shuffled queue!"));
        }

        public Task ClearQueue()
        {
            if (Player == null)
                throw PlayerNotFoundException;

            Player.Queue.Clear();
            return Task.CompletedTask;
        }
        #endregion
    }
}
