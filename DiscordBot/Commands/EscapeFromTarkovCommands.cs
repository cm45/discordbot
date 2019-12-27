using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    [Group("eft")]
    public class EscapeFromTarkovCommands: BaseCommandModule
    {
        [Group("links")]
        public class Links: BaseCommandModule
        {
            [GroupCommand]
            public async Task All(CommandContext ctx)
            {
                await ctx.RespondAsync("Yay!");
            }
        }
    }
}
