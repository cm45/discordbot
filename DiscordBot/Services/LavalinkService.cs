using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class LavalinkService
    {
        public class LavalinkConnectionConfig
        {
            public string Hostname { get; set; } = "localhost";
            public int Port { get; set; } = 2333;
            public string Password { get; set; } = "youshallnotpass";
        }

        public LavalinkNodeConnection LavalinkNode { get; private set; }

        private LavalinkConnectionConfig Configuration { get; }

        public LavalinkService(LavalinkConnectionConfig cfg, DiscordClient client)
        {
            Configuration = cfg;
            client.Ready += Client_Ready;
        }

        private async Task Client_Ready(ReadyEventArgs e)
        {
            if (LavalinkNode != null)
                return;

            var lava = e.Client.GetLavalink();
            LavalinkNode = await lava.ConnectAsync(new LavalinkConfiguration
            {
                Password = Configuration.Password,

                SocketEndpoint = new ConnectionEndpoint(Configuration.Hostname, Configuration.Port),
                RestEndpoint = new ConnectionEndpoint(Configuration.Hostname, Configuration.Port)
            }).ConfigureAwait(false);

            // Events
            LavalinkNode.LavalinkSocketErrored += LavalinkNode_LavalinkSocketErrored;
            LavalinkNode.StatisticsReceived += LavalinkNode_StatisticsReceived;

            LavalinkNode.TrackException += LavalinkNode_TrackException;
            LavalinkNode.TrackStuck += LavalinkNode_TrackStuck;
        }

        private Task LavalinkNode_StatisticsReceived(DSharpPlus.Lavalink.EventArgs.StatsReceivedEventArgs e)
        {
            Console.WriteLine("active players:" + e.Statistics.ActivePlayers);
            Console.WriteLine("total players:" + e.Statistics.TotalPlayers);
            Console.WriteLine("uptime:" + e.Statistics.Uptime);
            return Task.CompletedTask;
        }

        private Task LavalinkNode_TrackStuck(DSharpPlus.Lavalink.EventArgs.TrackStuckEventArgs e)
        {
            Console.WriteLine("[ERR] Track stuck!");
            return Task.CompletedTask;
        }

        private Task LavalinkNode_TrackException(DSharpPlus.Lavalink.EventArgs.TrackExceptionEventArgs e)
        {
            Console.WriteLine("[ERR] Track exception\n" + e.Error);
            return Task.CompletedTask;
        }

        private Task LavalinkNode_LavalinkSocketErrored(SocketErrorEventArgs e)
        {
            Console.WriteLine("[ERR] Lavalink socket error!");
            return Task.CompletedTask;
        }
    }
}
