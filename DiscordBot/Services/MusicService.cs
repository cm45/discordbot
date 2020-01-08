using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace DiscordBot.Services.Music
{
    public class MusicService
    {
        public static Embed NoPlayerEmbed = CustomEmbedBuilder.BuildErrorEmbed("No player found!");

        public DiscordSocketClient Client { get; private set; }
        public LavaConfig LavaConfig { get; private set; }
        public LavaNode LavaNode { get; private set; }
        public LavaPlayer Player { get; private set; }


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

        private async Task LavaNode_OnPlayerUpdated(Victoria.EventArgs.PlayerUpdateEventArgs arg)
        {
            if (Player == null || Player.Track == null)
                return;

            await Client.SetGameAsync(Player.Track.Title, Player.Track.Url);
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
            {
                await args.Player.TextChannel.SendMessageAsync("No more tracks to play.");
                return;
            }

            await args.Player.PlayAsync((LavaTrack)track);
        }
        #endregion

        #region Connection-handling commands
        public async Task JoinAsync(IVoiceChannel voiceChannel, ITextChannel textChannel)
        {
            Player = await LavaNode.JoinAsync(voiceChannel, textChannel);
            await Player.UpdateVolumeAsync(Config.ConfigCache.Volume);

            if (!string.IsNullOrWhiteSpace(Config.ConfigCache.OnJoin))
                await PlayAsync(Config.ConfigCache.OnJoin, voiceChannel.Guild as SocketGuild, true);
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
            var volume = GetVolume();
            return Task.FromResult(CustomEmbedBuilder.BuildInfoEmbed("", $"Current player volume is {volume}!"));
        }
        #endregion

        #region Playback commands
        public async Task<(Embed queueEmbed, Embed nowPlayingEmbed)> PlayAsync(string query, IGuild guild, bool forcePlay = false)
        {
            if (Player == null)
                return (NoPlayerEmbed, null);

            var search = await LavaNode.SearchAsync(query);

            switch (search.LoadStatus)
            {
                case LoadStatus.NoMatches:
                    return (CustomEmbedBuilder.BuildErrorEmbed("No matches!"), null);
                case LoadStatus.LoadFailed:
                    return (CustomEmbedBuilder.BuildErrorEmbed("Load failed!"), null);
                default:
                    break;
            }

            if (search.LoadStatus == LoadStatus.TrackLoaded || search.LoadStatus == LoadStatus.PlaylistLoaded)
            {
                var isPlaying = Player.PlayerState == PlayerState.Playing;

                LavaTrack nowPlaying = null;

                foreach (var track in search.Tracks)
                {
                    if (nowPlaying == null && (!isPlaying || forcePlay))
                    {
                        await Player.PlayAsync(track);
                        nowPlaying = track;
                    }
                    else Player.Queue.Enqueue(track);
                }

                var embedBuilder = new EmbedBuilder
                {
                    Description = $"Enqueued **{search.Tracks.Count}** tracks!"
                };

                return (embedBuilder.Build(), nowPlaying != null ? await CustomEmbedBuilder.BuildTrackEmbedAsync(nowPlaying) : null);
            }

            return (null, null);
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
            if (LavaNode.TryGetPlayer(guild, out var player))
            {
                await player.PauseAsync();
                return CustomEmbedBuilder.BuildSuccessEmbed($"Paused playback of '{player.Track.Title}'!");
            }
            return NoPlayerEmbed;
        }
        public async Task<Embed> StopAsync(SocketGuild guild)
        {
            if (LavaNode.TryGetPlayer(guild, out var player))
            {
                await player.StopAsync();
                return CustomEmbedBuilder.BuildSuccessEmbed($"Stopped playback of '{player.Track.Title}'!");
            }
            return NoPlayerEmbed;
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

            var currentPosition = new TimeSpan(currentTrack.Position.Hours, currentTrack.Position.Minutes, currentTrack.Position.Seconds);
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
                    var track = (LavaTrack)item;

                    embedBuilder.Description += $"\n**{++index})** {track.Title} **[{track.Duration}]**";
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
        #endregion
    }
}
