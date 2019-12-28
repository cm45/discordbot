using DiscordBot.EscapeFromTarkovAPI;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    [Group("eft")]
    public class EscapeFromTarkovCommands : BaseCommandModule
    {
        [Group("links")]
        public class Links : BaseCommandModule
        {
            [GroupCommand]
            public async Task All(CommandContext ctx)
            {
                var links = TarkovLinks.GetLinks();

                if (links.Length <= 0)
                {
                    await ctx.RespondAsync("No links found!");
                    return;
                }

                var embedBuilder = new DiscordEmbedBuilder()
                {
                    Title = "EFT Links",
                    Description = "Some useful eft links..."
                };


                foreach (var link in links)
                {
                    embedBuilder.Description += "\n" + link.ToClickableLink();
                }

                await ctx.RespondAsync(embed: embedBuilder);
            }

            [Command("add")]
            public async Task Add(CommandContext ctx, string title, string url)
            {
                TarkovLinks.AddLink(title, url);
                await ctx.RespondAsync("Added new link: " + title);
            }

            [Command("remove")]
            public async Task Remove(CommandContext ctx, string title)
            {
                TarkovLinks.RemoveLink(title);
                await ctx.RespondAsync("Removed link: " + title);
            }

        }

        [Group("maps")]
        public class Maps: BaseCommandModule
        {
            [GroupCommand]
            public async Task All(CommandContext ctx)
            {
                TarkovMaps.GetMaps();
                await ctx.RespondAsync("Querying maps...");
            }
        }
    }
}
