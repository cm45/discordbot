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
        public async Task Join(IVoiceChannel voiceChannel = null)
        {
            await MusicService.JoinAsync(voiceChannel ?? VoiceChannel, Context.Channel as ITextChannel);
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

        [Command("volume"), Alias("v", "vol")]
        public async Task Volume(ushort value)
        {
            int oldVolume = MusicService.GetVolume(Context.Guild);
            await MusicService.UpdateVolumeAsync(Context.Guild, value);
            await ReplyAsync($"Set Volume from {oldVolume} to {value} (Check: {MusicService.GetVolume(Context.Guild)})");
        }

        [Command("skip"), Alias("next", "n")]
        public async Task Skip()
        {
            await MusicService.Player.SkipAsync();
        }

        [Command("shuffle"), Alias("randomize", "rng")]
        public async Task ShuffleQueue()
        {
            MusicService.Shuffle(Context.Guild);
            await ReplyAsync("Shuffled Queue!");
        }

        [Command("pause"), Alias("p")] public async Task Pause() => await ReplyAsync(await MusicService.PauseAsync(Context.Guild));
        [Command("resume"), Alias("r")] public async Task Resume() => await ReplyAsync(await MusicService.ResumeAsync(Context.Guild));
        [Command("stop"), Alias("s")] public async Task Stop() => await ReplyAsync(await MusicService.StopAsync(Context.Guild));



        [Group("queue"), Alias("q")]
        public class Queue : ModuleBase<SocketCommandContext>
        {
            public MusicService MusicService { get; set; }

            [Command("enqueue"), Alias("add", "a", "+")]
            public async Task Enqueue([Remainder] string query)
            {
                await ReplyAsync("WIP (use '!music play' instead)");
                var result = await MusicService.PlayAsync(query, Context.Guild);
                await ReplyAsync(result);
            }

            [Command("remove"), Alias("delete", "r", "d", "-")]
            public async Task Remove(uint queueId)
            {
                await ReplyAsync(await MusicService.RemoveItemFromQueue(Context.Guild, queueId));
            }

            [Command("shuffle"), Alias("randomize", "rng")]
            public async Task ShuffleQueue()
            {
                MusicService.Shuffle(Context.Guild);
                await ReplyAsync("Shuffled Queue!");
            }

            [Command, Alias("show", "list", "all")]
            public async Task ShowQueue()
            {
                var currentTrack = MusicService.GetCurrentTrack(Context.Guild);

                var embedBuilder = new EmbedBuilder
                {
                    Title = "Musicbot - Queue",
                    Description = $"Currently playing {currentTrack?.Title}\n"
                };

                var queue = MusicService.GetQueue(Context.Guild);

                if (queue.Items != null)
                {
                    int tracksPerPage = 10;
                    int index = 0;
                    foreach (var item in queue.Items)
                    {
                        var track = (LavaTrack)item;
                        embedBuilder.Description += $"\n#{++index} - {track.Title} - [{track.Duration}]";
                        if (index >= tracksPerPage)
                        {
                            embedBuilder.Description += $"\nAnd {queue.Count - index + 1} more...";
                            break;
                        }
                    }
                    await ReplyAsync(embed: embedBuilder.Build());
                }
                else
                {
                    embedBuilder.Description += "queue.Items is null!";
                    await ReplyAsync(embed: embedBuilder.Build());
                }
            }
        }
    }
}
