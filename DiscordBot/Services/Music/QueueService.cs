using Discord;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Services.Music
{
    public class QueueService
    {
        public MusicService MusicService { get; set; }

        private LavaPlayer Player => MusicService.Player;

        public async Task<Embed> GetQueueMessageEmbedAsync()
        {
            if (Player == null)
                return MusicService.NoPlayerEmbed;

            var currentTrack = await MusicService.GetCurrentTrack();

            var currentPosition = currentTrack != null ? new TimeSpan(currentTrack.Position.Hours, currentTrack.Position.Minutes, currentTrack.Position.Seconds) : new TimeSpan();

            var musicalNote = ":musical_note:";
            var embedBuilder = new EmbedBuilder
            {
                Title = "Track Queue",
                Description = currentTrack != null ? $"{musicalNote} Currently playing {currentTrack.Title} **[{currentPosition}/{currentTrack.Duration}]**!\n" : ""
            };

            var queue = Player.Queue;

            if (queue.Items != null)
            {
                int tracksPerPage = 10;
                int index = 0;
                foreach (var item in queue.Items)
                {
                    var track = item as LavaTrack;

                    embedBuilder.Description += $"\n**{++index})** {track?.Title} **[{track?.Duration}]**";
                    if (index >= tracksPerPage)
                    {
                        embedBuilder.Description += $"\nAnd {queue.Count - index} more...";
                        break;
                    }
                }
                return embedBuilder.Build();

                // TODO: Implement pagescroller through reactions
            }
            else
            {
                embedBuilder.Description += "queue.Items is null!";
                return embedBuilder.Build();
            }
        }
        public async Task<Embed> AddTrackToQueueAsync(LavaTrack track)
        {
            if (Player == null)
                return MusicService.NoPlayerEmbed;

            Player.Queue.Enqueue(track);

            return await CustomEmbedBuilder.BuildTrackEmbedAsync(track);
        }
        public Task<Embed> RemoveItemFromQueue(uint queueId)
        {
            if (Player == null)
                return Task.FromResult(MusicService.NoPlayerEmbed);

            if (queueId + 1 > Player.Queue.Count)
                return Task.FromResult(CustomEmbedBuilder.BuildErrorEmbed("Index/ID out of bounds"));

            var item = (LavaTrack)Player.Queue.RemoveAt((int)queueId);
            return Task.FromResult(CustomEmbedBuilder.BuildSuccessEmbed("", $"Removed '{item.Title}' from queue!"));
        }
        public Task<Embed> Shuffle()
        {
            if (Player == null)
                return Task.FromResult(MusicService.NoPlayerEmbed);

            Player.Queue.Shuffle();
            return Task.FromResult(CustomEmbedBuilder.BuildSuccessEmbed("", "Shuffled queue!"));
        }

        public Task ClearQueue()
        {
            if (Player == null)
                throw MusicService.PlayerNotFoundException;

            Player.Queue.Clear();
            return Task.CompletedTask;
        }
    }
}
