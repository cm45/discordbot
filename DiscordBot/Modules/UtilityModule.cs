using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Name("Utility")]
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        #region Purge
        [Command("purge"), Alias("clear"), Summary("Clears x messages in this channel.")]
        public async Task PurgeAsync(uint amount = 100)
        {
            var messages = await Context.Channel.GetMessagesAsync((int)amount).FlattenAsync();
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages);
        }

        [Command("purge"), Alias("clear"), Summary("Clears x messages in a specific channel.")]
        public async Task PurgeAsync(ITextChannel channel, uint amount = 100)
        {
            var messages = await channel.GetMessagesAsync((int)amount).FlattenAsync();
            await channel.DeleteMessagesAsync(messages);
        }
        #endregion
    }
}
