﻿using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using DiscordBot.Services;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Name("Core"), Summary("Contains core functionality and basic bot information.")]
    public class CoreModule : ModuleBase<SocketCommandContext>
    {
        public CommandService CommandService { get; set; }
        public CoreService CoreService { get; set; }

        protected override void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
        {
            CommandService.CommandExecuted += CommandService_CommandExecuted;

            base.OnModuleBuilding(commandService, builder);
        }

        private Task CommandService_CommandExecuted(Optional<CommandInfo> arg1, ICommandContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
                return Task.CompletedTask;

            CoreService.CommandCounter++;
            Console.WriteLine($"Command executed {arg1.Value?.Name}");
            return Task.CompletedTask;
        }

        // Get Version
        [Command("info"), Summary("Get info about the bot.")]
        public async Task GetInfo()
        {
            var embedBuilder = new EmbedBuilder()
            {
                Title = "Plexi - Info",
                Color = Color.Blue
            };

            embedBuilder.AddField("Version", GetVersionString());
            embedBuilder.AddField("Source", "https://github.com/cm45/DiscordBot/");
            embedBuilder.AddField("Trello-Board", "https://trello.com/b/EOplC2d6/");
            embedBuilder.AddField("Application-Uptime", GetUptimeString());
            embedBuilder.AddField("Running on", Environment.OSVersion);
            embedBuilder.AddField("Build-Date", GetBuildDate(Assembly.GetExecutingAssembly()).ToLocalTime());
            embedBuilder.AddField("Successful commands since startup", CoreService.CommandCounter);

            // Get active modules
            embedBuilder.AddField("Active Modules", GetActiveModulesString());

            await ReplyAsync(embed: embedBuilder.Build());
        }

        private string GetVersionString() => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        private string GetActiveModulesString()
        {
            var s = "";

            foreach (var module in CommandService.Modules)
            {
                if (module.IsSubmodule)
                    continue;

                s += module.Name;

                if (module.Submodules.Count > 0)
                {
                    s += " (";
                    foreach (var submodule in module.Submodules)
                        s += submodule.Name + ", ";

                    s = s.TrimEnd().Trim(',');
                    s += ")";
                }

                s += Environment.NewLine;
            }

            return s;
        }

        private string GetUptimeString()
        {
            var startTime = Process.GetCurrentProcess().StartTime;
            var timeSinceStartup = (DateTime.Now - startTime);
            var timeSinceStartupString = $"{Math.Floor(timeSinceStartup.TotalHours)} hours {timeSinceStartup.Minutes} minutes {timeSinceStartup.Seconds} seconds";

            return $"{startTime} ({timeSinceStartupString})";
        }

        private static DateTime GetBuildDate(Assembly assembly)
        {
            const string BuildVersionMetadataPrefix = "+build";

            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute?.InformationalVersion != null)
            {
                var value = attribute.InformationalVersion;
                var index = value.IndexOf(BuildVersionMetadataPrefix);
                if (index > 0)
                {
                    value = value.Substring(index + BuildVersionMetadataPrefix.Length);
                    if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    {
                        return result;
                    }
                }
            }

            return default;
        }
    }
}
