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

        [Command]
        public async Task Info()
        {
            var track = MusicService.GetCurrentTrack(Context.Guild);
            var description = track != null ? $"Currently playing: {track.Title}" : "Idle...";

            var embed = new EmbedBuilder()
                .WithTitle("Musicbot - Info")
                .WithDescription(description)
                .Build();

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
            var search = await LavaNode.SearchAsync(query);
            var track = search.Tracks.First();

            LavaPlayer player = null;
            if (!LavaNode.TryGetPlayer(Context.Guild, out player))
                player = await LavaNode.JoinAsync(VoiceChannel);
            if (player is null) return;

            if (player.PlayerState == PlayerState.Playing)
            {
                player.Queue.Enqueue(track);
                await ReplyAsync($"Enqueued {track.Title}.");
            }
            else
            {
                await player.PlayAsync(track);
                await ReplyAsync($"Playing {track.Title}.");
            }
        }
    }
}
