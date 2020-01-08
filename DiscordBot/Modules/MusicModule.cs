using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services.Music;
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


        [Command, Alias("info", "current", "track", "song", "information", "playing")]
        public async Task Info()
        {
            var track = await MusicService.GetCurrentTrack();
            var description = track != null ? $"Currently playing: {track.Title}" : "Idle...";

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Musicbot - Info")
                .WithDescription(description);

            if (MusicService.Player != null)
            {
                var player = MusicService.Player;

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

        #region Connection
        [Command("join"), Alias("summon", "connect")]
        public async Task Join(IVoiceChannel voiceChannel = null)
        {
            await MusicService.JoinAsync(voiceChannel ?? VoiceChannel, Context.Channel as ITextChannel);
            await ReplyAsync($"Joined {voiceChannel} channel!");
        }

        [Command("leave"), Alias("disconnect")]
        public async Task Leave()
        {
            var voiceChannelString = MusicService.Player.VoiceChannel.ToString();
            await MusicService.LeaveAsync(VoiceChannel);
            await ReplyAsync($"Left {voiceChannelString}");
        }
        #endregion

        #region Playback
        [Command("play")]
        public async Task Play([Remainder] string query)
        {
            if (MusicService.Player == null)
                await MusicService.JoinAsync(VoiceChannel, Context.Channel as ITextChannel);

            var (queueEmbed, nowPlayingEmbed) = await MusicService.PlayAsync(query, Context.Guild);
            if (queueEmbed != null) await ReplyAsync(embed: queueEmbed);
            if (nowPlayingEmbed != null) await ReplyAsync(embed: nowPlayingEmbed);
        }
        [Command("forceplay")]
        public async Task ForcePlay([Remainder] string query)
        {
            if (MusicService.Player == null)
                await MusicService.JoinAsync(VoiceChannel, Context.Channel as ITextChannel);

            var (queueEmbed, nowPlayingEmbed) = await MusicService.PlayAsync(query, Context.Guild, true);
            if (queueEmbed != null) await ReplyAsync(embed: queueEmbed);
            if (nowPlayingEmbed != null) await ReplyAsync(embed: nowPlayingEmbed);
        }

        [Command("resume"), Alias("r")] public async Task Resume() => await ReplyAsync(embed: await MusicService.ResumeAsync(Context.Guild));
        [Command("pause")] public async Task Pause() => await ReplyAsync(embed: await MusicService.PauseAsync(Context.Guild));
        [Command("stop"), Alias("s")] public async Task Stop() => await ReplyAsync(embed: await MusicService.StopAsync(Context.Guild));


        #endregion

        #region Volume control
        [Command("volume"), Alias("v", "vol")] public async Task Volume(ushort value) => await ReplyAsync(embed: await MusicService.SetVolumeAsync(value));
        [Command("volume"), Alias("v", "vol")] public async Task Volume() => await ReplyAsync(embed: await MusicService.GetVolumeAsEmbedMessage());
        #endregion

        #region Queue
        [Command("queue"), Alias("q", "show", "list", "all")] public async Task ShowQueue() => await ReplyAsync(embed: await MusicService.GetQueueMessageEmbedAsync());
        [Command("skip"), Alias("next", "n")] public async Task Skip() => await ReplyAsync(embed: await MusicService.SkipAsync());
        [Command("shuffle"), Alias("randomize", "rng")] public async Task ShuffleQueue() => await ReplyAsync(embed: await MusicService.Shuffle());
        [Command("remove"), Alias("delete", "r", "d", "-")] public async Task Remove(uint queueId) => await ReplyAsync(embed: await MusicService.RemoveItemFromQueue(queueId));
        #endregion
    }
}
