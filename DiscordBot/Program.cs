using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Implement sharding

            var bot = new Bot();
            AppDomain.CurrentDomain.ProcessExit += (object o, EventArgs args) => bot.ExitAsync().GetAwaiter().GetResult(); // TODO: FIX
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
