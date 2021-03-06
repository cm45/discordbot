﻿using Discord;
using Discord.Commands;
using DiscordBot.Core;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiscordBot.Services.ReminderService;

namespace DiscordBot.Modules
{
    [Name("Reminder"), Summary("Contains functionality to create reminders.")]
    public class ReminderModule : ModuleBase<SocketCommandContext>
    {
        public ReminderService ReminderService { get; set; }

        [Command("remindme"), Summary("The bot sends you a reminder at a specific date/time.")]
        public async Task RemindMeAsync(DateTime time, [Remainder] string message)
        {
            if (time == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Timeparameter is invalid!"));
                return;
            }

            var reminder = new Reminder(message, false, Context.User as IGuildUser, DateTime.Now, time);

            await ReminderService.AddReminder(reminder);

            await ReplyAsync("Added reminder!");
        }

        [Command("remindmein"), Summary("The bot reminds you in a specific time span.")]
        public async Task RemindMeInAsync(TimeSpan time, [Remainder] string message)
        {
            if (time == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Timeparameter is invalid!"));
                return;
            }

            var reminder = new Reminder(message, false, Context.User as IGuildUser, DateTime.Now, DateTime.Now + time);
            await ReminderService.AddReminder(reminder);
            await ReplyAsync("Added reminder!");
        }

        [Command("reminder"), Summary("Gets a list of all your reminders and all public reminders.")]
        public async Task GetReminderAsync()
        {
            var reminders = ReminderService.Reminders.Where(r => r.Creator == Context.User || r.IsPublic == true);

            var remindersString = "";
            foreach (var reminder in reminders)
                remindersString += reminder;

            await ReplyAsync(remindersString.Trim('\n'));
        }
    }
}
