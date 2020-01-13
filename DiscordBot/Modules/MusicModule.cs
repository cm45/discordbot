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
    [Group("music"), Alias("m"), Name("Music")]
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        public MusicService MusicService { get; set; }

        private IVoiceChannel VoiceChannel => (Context.User as IGuildUser).VoiceChannel;

        [Command, Alias("info", "current", "track", "song", "information", "playing"), Summary("Gets info about the current Track, Volume & Queue as well as a command help list?!")]
        public async Task Info()
        {
            var track = await MusicService.GetCurrentTrack();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Musicbot - Info");

            embedBuilder.AddField("Status", MusicService.Player.PlayerState.ToString());

            if (MusicService.Player.PlayerState == PlayerState.Playing)
                embedBuilder.AddField("Currently Playing", track.Title);

            embedBuilder.AddField("Current Voicechannel", MusicService.Player.VoiceChannel.Name ?? "None");
            embedBuilder.AddField("Volume", MusicService.Player.Volume.ToString() ?? Config.ConfigCache.Volume.ToString());

            await ReplyAsync(embed: embedBuilder.Build());
        }

        #region Connection
        [Command("join"), Alias("summon", "connect")]
        public async Task Join(IVoiceChannel voiceChannel = null)
        {
            await MusicService.JoinAsync(voiceChannel ?? VoiceChannel, Context.Channel as ITextChannel, true);
            await ReplyAsync($"Joined {voiceChannel} channel!");
        }

        [Command("leave"), Alias("disconnect")]
        public async Task Leave()
        {
            var currentChannelString = MusicService.Player.VoiceChannel.ToString();
            await MusicService.LeaveAsync(VoiceChannel);
            await ReplyAsync($"Left {currentChannelString}!");
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

        [Command("forceplay"), Alias("playnow")]
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
