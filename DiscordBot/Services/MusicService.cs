using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

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
            // TODO: Add lavasocketclient events

            return Task.CompletedTask;
        }

        public LavaTrack GetCurrentTrack(IGuild guild)
        {
            if (LavaNode.TryGetPlayer(guild, out LavaPlayer player))
                return player.Track;

            return null;
        }
        public async Task JoinAsync(IVoiceChannel voiceChannel, ITextChannel textChannel) => await LavaNode.JoinAsync(voiceChannel, textChannel);
        public async Task LeaveAsync(IVoiceChannel voiceChannel) => await LavaNode.LeaveAsync(voiceChannel);

        private Task Client_Ready()
        {
            return Task.CompletedTask;
        }
    }
}
