using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot
{
    public class DiscordBotClient
    {
        public DiscordSocketClient Client { get; private set; }
        public CommandService CommandService { get; private set; }
        public IServiceProvider Services { get; private set; }



        public DiscordBotClient(DiscordSocketClient client = null, CommandService commandService = null)
        {
            Client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = Discord.LogSeverity.Debug
            });

            CommandService = commandService ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = Discord.LogSeverity.Verbose
            });
        }

        public async Task InitializeAsync()
        {
            var config = await Config.GetConfigAsync();

            // Connect
            await Client.LoginAsync(TokenType.Bot, config.Token);
            await Client.StartAsync();

            // Event-handling
            Client.Log += Client_Log;
            
            Services = SetupServices();

            var commandHandler = new CommandHandler(Client, CommandService, Services);
            await commandHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices()
        {
            return new ServiceCollection()
                 .AddSingleton(Client)
                 .AddSingleton(CommandService)
                 .AddSingleton<LavaSocket>()
                 .BuildServiceProvider();
        }

        private Task Client_Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            // TODO: DC Client
            return Task.CompletedTask;
        }
    }
}
