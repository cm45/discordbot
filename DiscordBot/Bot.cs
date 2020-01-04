using DiscordBot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public sealed class Bot
    {
        private IServiceProvider Services { get; set; }

        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public VoiceNextExtension Voice { get; private set; }
        public LavalinkExtension Lavalink { get; private set; }
        public LavalinkNodeConnection LavalinkConnection { get; set; }

        public Bot()
        {
            // TODO: Move initial configurations into constructor
        }

        private async Task<Config> GetConfigAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            {
                using var sr = new StreamReader(fs, new UTF8Encoding(false));
                json = await sr.ReadToEndAsync().ConfigureAwait(false);
            }
            return JsonConvert.DeserializeObject<Config>(json);
        }

        public async Task RunAsync()
        {
            var config = await GetConfigAsync();

            #region Initialize Discord-Client
            var discordConfig = new DiscordConfiguration()
            {
                Token = config.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,

                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            Client = new DiscordClient(discordConfig);
            #endregion

            #region Attach Event Handlers // TODO:
            Client.Ready += Client_Ready;
            #endregion

            #region Create Service Provider
            Services = new ServiceCollection()
                .AddSingleton(new LavalinkService(new LavalinkService.LavalinkConnectionConfig(), Client))
                .AddSingleton(this)
                .BuildServiceProvider(true);
            #endregion

            #region Initialize CommandsNext Module
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { config.Prefix },
                DmHelp = true,
                Services = Services
            };
            Commands = Client.UseCommandsNext(commandsConfig);

            // TODO: Set help-formatter

            // Register Commands
            Commands.RegisterCommands<Commands.EscapeFromTarkovCommands>();
            Commands.RegisterCommands<Commands.UtilityCommands>();
            Commands.RegisterCommands<Commands.MusicCommands>();
            #endregion

            #region Initialize Lavalink Module
            Lavalink = Client.UseLavalink();
            #endregion

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task Client_Ready(DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            Console.WriteLine("Bot is ready!");
            return Task.CompletedTask;
        }

        public async Task ExitAsync()
        {
            await Client.DisconnectAsync();
        }
    }
}
