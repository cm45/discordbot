using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

        public class Reminder
        {
            public string Message { get; set; }
            public bool IsPublic { get; set; }
            public IGuildUser Creator { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            private Timer timer;

            public Reminder(string message, bool isPublic, IGuildUser creator, DateTime startTime, DateTime endTime)
            {
                Message = message;
                IsPublic = isPublic;
                Creator = creator;
                StartTime = startTime;
                EndTime = endTime;

                Console.WriteLine("Creating Reminder...");

                TimeSpan time = EndTime.TimeOfDay - DateTime.Now.TimeOfDay;
                timer = new Timer(x => Remind(), null, time, Timeout.InfiniteTimeSpan);
            }

            public Task Remind()
            {
                Console.WriteLine("YAY");
                Creator.SendMessageAsync($"**[Reminder]** *{Message}* from {StartTime}");

                return Task.CompletedTask;
            }

            public override string ToString() => $"*\"{Message}\"* by {Creator.Mention} created at **{StartTime}** and ends at **{EndTime}**!\n";
        }
    }


}
