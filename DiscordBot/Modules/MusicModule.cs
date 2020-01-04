using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Victoria;
using Victoria.Enums;

namespace DiscordBot.Modules
{
    [Group("music")]
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        public MusicService MusicService { get; set; }

        private IVoiceChannel VoiceChannel => (Context.User as IGuildUser).VoiceChannel;
        private LavaNode LavaNode => MusicService.LavaNode;

        [Command, Alias("info", "current", "track", "song", "information", "playing")]
        public async Task Info()
        {
            var track = MusicService.GetCurrentTrack(Context.Guild);
            var description = track != null ? $"Currently playing: {track.Title}" : "Idle...";

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Musicbot - Info")
                .WithDescription(description);

            if (LavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                embedBuilder.AddField("Channel", player.VoiceChannel);
                embedBuilder.AddField("Volume", player.Volume);
                if (player.Track != null)
                {
                    embedBuilder.AddField("Time", $"WIP/{player.Track.Duration}");
                }
            }

            var embed = embedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        [Command("join"), Alias("summon", "connect")]
        public async Task Join()
        {
            await MusicService.JoinAsync(VoiceChannel, Context.Channel as ITextChannel);
            await ReplyAsync($"Joined {VoiceChannel} channel!");
        }

        [Command("leave"), Alias("disconnect")]
        public async Task Leave()
        {
            await MusicService.LeaveAsync(VoiceChannel);
            await ReplyAsync("Bye!");
        }

        [Command("play")]
        public async Task PlayAsync([Remainder] string query)
        {
            var result = await MusicService.PlayAsync(query, Context.Guild);
            await ReplyAsync(result);
        }
    }
}
