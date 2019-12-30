using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;

namespace DiscordBot.Commands
{
    [Group("music")]
    public class MusicCommands: BaseCommandModule
    {
        [Command("join")]
        public async Task Join(CommandContext ctx, DiscordChannel channel = null)
        {
            if (ctx.Message.Channel.IsPrivate)
            {
                await ctx.RespondAsync("Direct Messages are not supported for this command!");
                return;
            }

            var voiceClient = ctx.Client.GetVoiceNext();
            var voiceConnection = voiceClient.GetConnection(ctx.Guild);

            if (voiceConnection != null)
            {
                await ctx.RespondAsync("Already connected to this guild!");
                return;
            }

            channel ??= ctx.Member?.VoiceState?.Channel;
            if (channel == null)
            {
                await ctx.RespondAsync($"You need to be in a voice channel or mention a channel in the join command!");
                return;
            }

            voiceConnection = await voiceClient.ConnectAsync(channel);
            await ctx.RespondAsync($"Joining \"{ctx.Member?.DisplayName}\" into channel \"{voiceConnection.Channel.Name}\"");
        }
    }
}
