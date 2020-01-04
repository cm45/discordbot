using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("eft")]
    public class EscapeFromTarkovModule: ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task Info()
        {
            await ReplyAsync("Info... WIP");
        }

        [Command("maps")]
        public async Task Maps()
        {
            await ReplyAsync("test");
        }
    }
}
