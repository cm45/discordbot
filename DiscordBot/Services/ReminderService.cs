using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class ReminderService
    {
        public List<Reminder> Reminders { get; private set; } = new List<Reminder>();

        public Task AddReminder(Reminder reminder)
        {
            Reminders.Add(reminder);

            // TODO: async thread wait for x time to run out... (register)

            return Task.CompletedTask;
        }

        public struct Reminder
        {
            public string Message { get; set; }
            public bool IsPublic { get; set; }
            public IGuildUser Creator { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            public override string ToString() => $"\"{Message}\" by {Creator.Nickname} created at {StartTime} and ends at {EndTime}!\n";
        }
    }


}
