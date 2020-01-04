using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new DiscordBotClient();
            AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) => bot.TerminateAsync().GetAwaiter().GetResult();
            await bot.InitializeAsync();
        }
    }
}
