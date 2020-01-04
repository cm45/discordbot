using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace DiscordBot.Services
{
    public class MusicService
    {
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
            await args.Player.TextChannel.SendMessageAsync($"Now playing: {((LavaTrack)track).Title}");
        }

        private Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            Console.WriteLine($"{user} moved from {oldVoiceState.VoiceChannel} to {newVoiceState.VoiceChannel}");
            return Task.CompletedTask;
        }

        public LavaTrack GetCurrentTrack(IGuild guild)
        {
            if (LavaNode.TryGetPlayer(guild, out LavaPlayer player))
                return player.Track;

            return null;
        }
        public async Task JoinAsync(IVoiceChannel voiceChannel, ITextChannel textChannel)
        {
            Player = await LavaNode.JoinAsync(voiceChannel, textChannel);
            await Player.UpdateVolumeAsync(Config.ConfigCache.Volume);

            if (!string.IsNullOrWhiteSpace(Config.ConfigCache.OnJoin))
                await PlayAsync(Config.ConfigCache.OnJoin, voiceChannel.Guild as SocketGuild, true);
        }
        public async Task LeaveAsync(IVoiceChannel voiceChannel) => await LavaNode.LeaveAsync(voiceChannel);

        public async Task UpdateVolumeAsync(IGuild guild, ushort value)
        {
            if (LavaNode.TryGetPlayer(guild, out var player))
            {
                await player.UpdateVolumeAsync(value);
                var config = Config.ConfigCache;
                config.Volume = value;
                await Config.SaveConfigAsync(config);
            }
        }

        public int GetVolume(IGuild guild)
        {
            if (LavaNode.TryGetPlayer(guild, out var player))
                return player.Volume;

            return -1;
        }

        private Task Client_Ready()
        {
            return Task.CompletedTask;
        }

        public Task<string> RemoveItemFromQueue(IGuild guild, uint queueId)
        {
            if (LavaNode.TryGetPlayer(guild, out var player))
            {
                if (queueId + 1 > player.Queue.Count)
                    return Task.FromResult("Index/ID out of bounds");

                var item = (LavaTrack)player.Queue.RemoveAt((int)queueId);
                return Task.FromResult($"Removed '{item.Title}' from queue!");
            }
            return Task.FromResult("No player detected!");
        }

        public void Shuffle(IGuild guild)
        {
            if (LavaNode.TryGetPlayer(guild, out var player))
                player.Queue.Shuffle();
        }

        public async Task<string> PlayAsync(string query, IGuild guild, bool forcePlay = false)
        {
            var search = await LavaNode.SearchAsync(query);

            LavaPlayer player = null;
            if (!LavaNode.TryGetPlayer(guild, out player))
                return "Player is null!";

            if (search.Tracks.Count > 1)
            {
                LavaTrack playTrack = null;
                foreach (var track in search.Tracks)
                {
                    if (player.PlayerState == PlayerState.Playing && forcePlay == false)
                    {
                        player.Queue.Enqueue(track);
                    }
                    else
                    {
                        await player.PlayAsync(track);
                        playTrack = track;
                    }
                }
                return $"Enqueued {search.Tracks.Count} tracks!";
            }
            else if (search.Tracks.Count == 1)
            {
                var track = search.Tracks.FirstOrDefault();

                if (player.PlayerState == PlayerState.Playing && forcePlay == false)
                {
                    player.Queue.Enqueue(track);
                    return $"Enqueued {track.Title}.";
                }
                else
                {
                    await player.PlayAsync(track);
                    return $"Playing {track.Title}.";
                }
            }
            else
            {
                return $"Couldn't find any results!";
            }
        }

        public DefaultQueue<Victoria.Interfaces.IQueueable> GetQueue(IGuild guild)
        {
            if (LavaNode.TryGetPlayer(guild, out var player))
                return player.Queue;

            return new DefaultQueue<Victoria.Interfaces.IQueueable>();
        }
    }
}
