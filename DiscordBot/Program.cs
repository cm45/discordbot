using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            AppDomain.CurrentDomain.ProcessExit += (Object o, EventArgs args) => bot.ExitAsync().GetAwaiter().GetResult();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
