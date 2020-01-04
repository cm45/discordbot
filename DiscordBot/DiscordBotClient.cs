using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
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

        // Music
        public LavaNode LavaNode { get; private set; }
        public LavaConfig LavaConfig { get; private set; }
        public MusicService MusicService { get; private set; }


        public DiscordBotClient(DiscordSocketClient client = null, CommandService commandService = null, LavaNode lavaNode = null)
        {
            Client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug
            });

            CommandService = commandService ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            // Music
            LavaConfig = Config.ConfigCache.LavaConfig;
            LavaNode = lavaNode ?? new LavaNode(Client, LavaConfig);
            MusicService = new MusicService(Client, LavaConfig, LavaNode);
            MusicService.InitializeAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            var config = await Config.GetConfigAsync();

            // Connect
            await Client.LoginAsync(TokenType.Bot, config.Token);
            await Client.StartAsync();

            // Event-handling
            Client.Log += Client_Log;
            Client.Ready += OnClientReadyAsync;

            Services = SetupServices();

            var commandHandler = new CommandHandler(Client, CommandService, Services);
            await commandHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        private async Task OnClientReadyAsync()
        {
            await LavaNode.ConnectAsync();
        }

        // TODO: Rename
        private IServiceProvider SetupServices()
        {
            return new ServiceCollection()
                 .AddSingleton(Client)
                 .AddSingleton(CommandService)
                 .AddSingleton(LavaConfig)
                 .AddSingleton(LavaNode)
                 .AddSingleton(MusicService)
                 .BuildServiceProvider();
        }

        private Task Client_Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }

        public async Task TerminateAsync() => await Client.StopAsync();
    }
}
