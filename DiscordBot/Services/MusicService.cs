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

            return Task.CompletedTask;
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
            await LavaNode.JoinAsync(voiceChannel, textChannel);

            if (!string.IsNullOrWhiteSpace(Config.ConfigCache.OnJoin))
                await PlayAsync(Config.ConfigCache.OnJoin, voiceChannel.Guild as SocketGuild, true);
        }
        public async Task LeaveAsync(IVoiceChannel voiceChannel) => await LavaNode.LeaveAsync(voiceChannel);

        private Task Client_Ready()
        {
            return Task.CompletedTask;
        }

        public async Task<string> PlayAsync(string query, SocketGuild guild, bool forcePlay = false)
        {
            var search = await LavaNode.SearchAsync(query);
            var track = search.Tracks.FirstOrDefault();

            LavaPlayer player = null;
            if (!LavaNode.TryGetPlayer(guild, out player))
                return "Player is null!";

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
    }
}
