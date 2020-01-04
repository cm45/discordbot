using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new DiscordBotClient();
            await bot.InitializeAsync();

            AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) => bot.DisconnectAsync().GetAwaiter().GetResult();
        }
    }
}
