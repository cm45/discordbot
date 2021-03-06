﻿using Discord;
using Discord.Commands;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Name("Random"), Summary("Contains several random generating commands.")]
    public class RandomModule : ModuleBase<SocketCommandContext>
    {
        public Random Rng { get; set; }

        [Command("dice"), Alias("roll"), Summary("Rolls a dice."), Remarks("Rolls a dice between 1 and 6 (customizable).")]
        public async Task RollDiceAsync(int eyes = 6)
        {
            var number = Rng.Next(1, eyes + 1);
            var embedBuilder = new EmbedBuilder()
            {
                Title = $"Rolled '{number}'!"
            };
            await ReplyAsync(embed: embedBuilder.Build());
        }

        #region Random Number Generator
        [Command("rng"), Alias("random"), Summary("Generates a random number between a min and max number.")]
        public async Task RandomNumberAsync(int min, int max)
        {
            var number = Rng.Next(min, max + 1);
            await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Random Number Generator ({min} - {max})", $"Your generated random number between {min} & {max} is {number}!"));
        }

        [Command("rng"), Alias("random"), Summary("Generates a random number between 0 and a max number.")]
        public async Task RandomNumberAsync(int max)
        {
            var number = Rng.Next(max + 1);
            await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Random Number Generator (0 - {max})", $"Your generated random number between 0 & {max} is {number}!"));
        }
        #endregion

        [Command("randomuser"), Summary("Selects a random user of your current voicechannel or a specified voicechannel.")]
        public async Task SelectUserPersonAsync([Remainder] IVoiceChannel voiceChannel = null)
        {
            voiceChannel ??= (Context.User as IGuildUser).VoiceChannel;

            if (voiceChannel == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("No voicechannel found!"));
                return;
            }

            var users = await voiceChannel.GetUsersAsync().FlattenAsync();

            if (users.Count() <= 0)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Selected voicechannel is empty!"));
                return;
            }

            var index = Rng.Next(users.Count() - 1);

            await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Random Person Selector ({voiceChannel.Name})", $"{users.ElementAt(index).Mention} is the chosen one!"));
        }
    }
}
