using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;
using System.IO;
using System.Diagnostics;
using DSharpPlus.Lavalink;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using DiscordBot.Services;

namespace DiscordBot.Commands
{
    [Group("music")]
    public class MusicCommands : BaseCommandModule
    {
        Queue<LavalinkTrack> tracks = new Queue<LavalinkTrack>();

        [Command("connect"), Aliases("join", "summon")]
        public async Task Connect(CommandContext ctx, DiscordChannel channel = null)
        {
            try
            {
                var lavalink = ctx.Services.GetService<LavalinkService>();

                channel ??= ctx.Member?.VoiceState?.Channel;

                if (channel == null)
                {
                    await ctx.RespondAsync("Not in a voicechannel or no voicechannel mentioned!");
                    return;
                }

                var guildConnection = await lavalink.LavalinkNode.ConnectAsync(channel);
                var result = await lavalink.LavalinkNode.GetTracksAsync("https://www.youtube.com/watch?v=iLIUmis_rR8");


                LavalinkTrack track = result.Tracks.First();

                guildConnection.Play(track);
                Console.WriteLine($"Playing {track.Title}...");


                //channel ??= ctx.Channel;
                //if (channel == null)
                //{
                //    await ctx.RespondAsync("Not in a channel...");
                //    return;
                //}
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(ex.Message);
                throw ex;
            }
        }

        [Command("disconnect"), Aliases("leave", "exit")]
        public async Task Disconnect(CommandContext ctx)
        {
            try
            {
                if (ctx.Message.Channel.IsPrivate)
                {
                    await ctx.RespondAsync("Direct Messages are not supported for this command!");
                    return;
                }

                var lavalinkConnectionNode = ctx.Services.GetService<LavalinkService>().LavalinkNode;
                var guildConnection = lavalinkConnectionNode.GetConnection(ctx.Guild);
                guildConnection.Disconnect();
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(ex.Message);
                throw ex;
            }
        }

        [Command("connection")]
        public async Task GetConnectionInfo(CommandContext ctx)
        {
            try
            {
                var lavalinkConnectionNode = ctx.Services.GetService<LavalinkService>().LavalinkNode;
                var guildConnection = lavalinkConnectionNode.GetConnection(ctx.Guild);

                var playbackString = guildConnection.CurrentState.PlaybackPosition + "/" + guildConnection.CurrentState.CurrentTrack.Length;
                Console.WriteLine(guildConnection.CurrentState.CurrentTrack.Title + " - " + playbackString);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(ex.Message);
                throw ex;
            }
        }

        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string file)
        {
            try
            {
                var lavalinkConnectionNode = ctx.Services.GetService<LavalinkService>().LavalinkNode;
                var guildConnection = lavalinkConnectionNode.GetConnection(ctx.Guild);

                var track = lavalinkConnectionNode.GetTracksAsync(new Uri("https://www.youtube.com/watch?v=iLIUmis_rR8")).Result.Tracks.First();
                guildConnection.Play(track);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await ctx.RespondAsync(ex.Message);
                throw ex;
            }
        }
    }
}
