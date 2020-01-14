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
    [Name("Reminder")]
    public class ReminderModule : ModuleBase<SocketCommandContext>
    {
        public ReminderService ReminderService { get; set; }

        [Command("remindme")]
        public async Task RemindMeAsync(DateTime time, [Remainder] string message)
        {
            if (time == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Timeparameter is invalid!"));
                return;
            }

            var reminder = new Reminder
            {
                Creator = Context.User as IGuildUser,
                Message = message,
                IsPublic = false,
                StartTime = DateTime.Now,
                EndTime = time
            };

            await ReminderService.AddReminder(reminder);

            await ReplyAsync("Added reminder!");
        }

        [Command("remindmein")]
        public async Task RemindMeInAsync(TimeSpan time, [Remainder] string message)
        {
            if (time == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Timeparameter is invalid!"));
                return;
            }

            var reminder = new Reminder
            {
                Creator = Context.User as IGuildUser,
                Message = message,
                IsPublic = false,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now + time
            };

            string s = $"Old count = {ReminderService.Reminders.Count}";
            await ReminderService.AddReminder(reminder);
            s += $"New count = {ReminderService.Reminders.Count}";

            await ReplyAsync(s + "Added reminder!");
        }

        [Command("reminder")]
        public async Task GetReminderAsync()
        {
            var reminders = ReminderService.Reminders.Where(r => r.Creator == Context.User || r.IsPublic == true);

            var remindersString = reminders.Count().ToString() + '\n' + ReminderService.Reminders.Count().ToString() + '\n';
            foreach (var reminder in reminders)
                remindersString += reminder;

            await ReplyAsync(remindersString.Trim('\n'));
        }
    }
}
