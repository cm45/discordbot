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
        public SettingsService SettingsService { get; set; }
        public QueueService QueueService { get; set; }

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
            if (!args.Reason.ShouldPlayNext())
                return;

            if (!args.Player.Queue.TryDequeue(out var track))
                return;

            if (SettingsService.Config.MusicBot.IsRepeating)
                await QueueService.AddTrackToQueueAsync((LavaTrack)track);

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
            await Player.UpdateVolumeAsync(SettingsService.Config.MusicBot.Volume);

            // Play On Join Song
            //if (!string.IsNullOrWhiteSpace(Config.ConfigCache.OnJoin) && playOnJoinTrack)
            //{
            //    await PlayAsync(Config.ConfigCache.OnJoin, true);
            //}
        }
        public async Task LeaveAsync(IVoiceChannel voiceChannel)
        {
            voiceChannel ??= Player.VoiceChannel;

            if (voiceChannel == null)
                throw PlayerNotFoundException;

            await LavaNode.LeaveAsync(voiceChannel);
        }
        #endregion

        #region Volume commands
        public async Task<Embed> SetVolumeAsync(ushort value)
        {
            if (Player == null)
                return NoPlayerEmbed;

            int oldVolume = GetVolume();
            value = (ushort)Math.Clamp(value, 0U, 10U); // TODO: Replace magic numbers

            await Player.UpdateVolumeAsync(value);

            await SettingsService.SetVolumeAsync(value);

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
            var volumeString = playerVolume == -1 ? SettingsService.Config.MusicBot.Volume.ToString() : playerVolume.ToString();
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

        public async Task<Embed> ResumeAsync()
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
        public async Task<Embed> PauseAsync()
        {
            if (Player == null)
                return NoPlayerEmbed;

            await Player.PauseAsync();
            return CustomEmbedBuilder.BuildSuccessEmbed($"Paused playback of '{Player.Track.Title}'!");
        }
        public async Task<Embed> StopAsync()
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
    }
}
