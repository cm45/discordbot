using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Services;
using DiscordBot.Services.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Victoria;
using Victoria.Enums;

namespace DiscordBot.Modules.Music
{
    [Name("Music"), Summary("Contains all commands regarding the music bot.")]
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        public MusicService MusicService { get; set; }
        public SettingsService SettingsService { get; set; }

        private IVoiceChannel VoiceChannel => (Context.User as IGuildUser).VoiceChannel;

        [Command("music"), Summary("Gets information about the current player state.")]
        public async Task Info()
        {
            var track = await MusicService.GetCurrentTrack();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Musicbot - Info");

            embedBuilder.AddField("Status", MusicService.Player.PlayerState.ToString());

            if (MusicService.Player.PlayerState == PlayerState.Playing)
                embedBuilder.AddField("Currently Playing", track.Title);

            embedBuilder.AddField("Current Voicechannel", MusicService.Player.VoiceChannel.Name ?? "None");
            embedBuilder.AddField("Volume", MusicService.Player.Volume.ToString() ?? SettingsService.Config.MusicBot.Volume.ToString());

            await ReplyAsync(embed: embedBuilder.Build());
        }

        #region Connection
        [Command("join"), Alias("summon", "connect"), Summary("Instructs the bot to join your voicechannel or the specified voicechannel.")]
        public async Task Join(IVoiceChannel voiceChannel = null)
        {
            await MusicService.JoinAsync(voiceChannel ?? VoiceChannel, Context.Channel as ITextChannel, true);
            await ReplyAsync($"Joined {voiceChannel} channel!");
        }

        [Command("leave"), Alias("disconnect"), Summary("Instructs the bot to leave the current voicechannel.")]
        public async Task Leave()
        {
            try
            {
                var currentChannelString = MusicService.Player.VoiceChannel.ToString();
                await MusicService.StopAsync();
                await MusicService.LeaveAsync(VoiceChannel);
                await ReplyAsync($"Left {currentChannelString}!");
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed(ex.Message));
                throw ex;
            }
        }
        #endregion

        #region Playback
        [Command("play"), Alias("p"), Summary("Plays a specify track by URL or Search Query.")]
        public async Task Play([Remainder] string query)
        {
            if (MusicService.Player == null || MusicService.Player.VoiceChannel == null)
                await MusicService.JoinAsync(VoiceChannel, Context.Channel as ITextChannel);

            try
            {
                var tracks = await MusicService.GetTracksAsync(query);

                if (tracks.Count() == 1)
                    await MusicService.PlayAsync(tracks.First());
                else
                    await MusicService.PlayAsync(tracks);
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed(ex.Message));
                return;
            }
        }

        [Command("resume"), Alias("r"), Summary("Resumes the active track.")] public async Task Resume() => await ReplyAsync(embed: await MusicService.ResumeAsync());
        [Command("pause"), Summary("Pauses the player.")] public async Task Pause() => await ReplyAsync(embed: await MusicService.PauseAsync());
        [Command("stop"), Alias("s"), Summary("Stops the player as well as terminate the current track.")] public async Task Stop() => await ReplyAsync(embed: await MusicService.StopAsync());
        [Command("skip"), Alias("next", "n"), Summary("Skips the current track.")] public async Task SkipAsync() => await ReplyAsync(embed: await MusicService.SkipAsync());
        #endregion

        #region Volume control
        [Command("volume"), Alias("v", "vol"), Summary("Sets the players volume.")] public async Task Volume(ushort value) => await ReplyAsync(embed: await MusicService.SetVolumeAsync(value));
        [Command("volume"), Alias("v", "vol"), Summary("Gets the current volume.")] public async Task Volume() => await ReplyAsync(embed: await MusicService.GetVolumeAsEmbedMessage());
        #endregion
    }
}
