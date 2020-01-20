using Discord.Commands;
using DiscordBot.Core;
using DiscordBot.Services.Music;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Music
{
    [Group("queue"), Alias("q"), Name("Queue"), Summary("Contains all queue managing commands.")]
    public class QueueModule : ModuleBase<SocketCommandContext>
    {
        public MusicService MusicService { get; set; }
        public QueueService QueueService { get; set; }

        [Command("queue"), Alias("q", "show", "list", "all"), Summary("Shows current queue.")] public async Task ShowQueueAsync() => await ReplyAsync(embed: await QueueService.GetQueueMessageEmbedAsync());
        [Command("shuffle"), Alias("randomize", "rng"), Summary("Shuffle current queue.")] public async Task ShuffleQueueAsync() => await ReplyAsync(embed: await QueueService.Shuffle());
        [Command("remove"), Alias("delete", "r", "d", "-"), Summary("Remove track from current queue based on index.")] public async Task RemoveTrackFromQueueAsync(uint queueId) => await ReplyAsync(embed: await QueueService.RemoveItemFromQueue(queueId));
        [Command("clear"), Alias("c"), Summary("Clears the current queue.")]
        public async Task ClearQueueAsync()
        {
            try
            {
                await QueueService.ClearQueue();
                await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed("Cleared Queue"));
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed(ex.Message));
            }
        }
    }
}
