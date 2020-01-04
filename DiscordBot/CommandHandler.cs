using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class CommandHandler
    {
        public DiscordSocketClient Client { get; private set; }
        public CommandService CommandService { get; private set; }
        public IServiceProvider Services { get; private set; }

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services)
        {
            Client = client;
            CommandService = commandService;
            Services = services;
        }

        public async Task InitializeAsync()
        {
            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            CommandService.Log += CommandService_Log;
            Client.MessageReceived += Client_MessageReceived;
        }

        private async Task Client_MessageReceived(SocketMessage msg)
        {
            Console.WriteLine(msg.Content);

            if (!(msg is SocketUserMessage userMessage))
                return;

            var argPos = 0;

            // Check prefix
            var prefixString = (await Config.GetConfigAsync()).Prefix;

            var hasPrefix = userMessage.HasMentionPrefix(Client.CurrentUser, ref argPos) || userMessage.HasStringPrefix(prefixString, ref argPos);
                        
            if (!hasPrefix || userMessage.Author.IsBot)
                return;

            var context = new SocketCommandContext(Client, userMessage);
            var result = await CommandService.ExecuteAsync(context, argPos, Services);
        }

        private Task CommandService_Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }
    }
}
