using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class UtilityCommands : BaseCommandModule
    {
        [Command("clear")]
        public async Task Clear(CommandContext ctx, int amount = 100)
        {
            var messages = await ctx.Channel.GetMessagesAsync(amount);
            var messageCount = messages.Count;
            await ctx.Channel.DeleteMessagesAsync(messages);
            var responseMessage = await ctx.RespondAsync($"Deleted {messageCount}/{amount} messages!");
            await Task.Delay(3000); // TODO: Store in config db!
            await responseMessage.DeleteAsync();
        }
    }
}
