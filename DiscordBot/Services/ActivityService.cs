using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Services
{
    public class ActivityService
    {
        public DiscordSocketClient DiscordSocketClient { get; set; }
        public IActivity Activity => DiscordSocketClient.Activity;

        public async Task ClearActivityAsync() => await DiscordSocketClient.SetActivityAsync(null);
        public async Task SetActivityAsync(IActivity activity) => await DiscordSocketClient.SetActivityAsync(activity);
        public async Task SetActivityAsync(string title, string url) => await DiscordSocketClient.SetActivityAsync(new StreamingGame(title, url));
        public async Task SetActivityAsync(LavaTrack track) => await DiscordSocketClient.SetActivityAsync(new StreamingGame(track.Title, track.Url));

        // TODO: Add builder functionality for different Activities outside of streaming game...
    }
}
