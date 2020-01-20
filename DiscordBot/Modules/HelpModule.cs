using Discord;
using Discord.Commands;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class Help : ModuleBase
    {
        public CommandService CommandService { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        [Command("help")]
        [Summary("Lists this bot's commands.")]
        public async Task GetHelp([Remainder] string path = "")
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            // Top-Level help
            if (string.IsNullOrWhiteSpace(path))
            {
                embedBuilder.Title = "Help";
                embedBuilder.Description = "Listing all top-level commands and groups.";
                embedBuilder.Footer = new EmbedFooterBuilder().WithText("Specify a group/command to see more information. (help <module/command>)");

                // Create Top Level Modules Field
                var topLevelModules = CommandService.Modules.Where(m => !m.IsSubmodule);
                var modulesString = string.Join(", ", topLevelModules.Select(m => $"`{m.Name}`"));
                embedBuilder.AddField("Top Level Modules", modulesString);

                // Create Top Level Commands Field
                var topLevelCommands = CommandService.Commands.Where(c => !c.Module.IsSubmodule && string.IsNullOrEmpty(c.Module.Group));
                var commandsString = string.Join(", ", topLevelCommands.Select(c => $"`{c.Name}`"));
                embedBuilder.AddField("Top Level Commands", commandsString);
            }

            // Module/Command specific help
            else
            {
                ModuleInfo module = null;
                CommandInfo command = null;

                foreach (var arg in path.Split(' '))
                {
                    var moduleInfo = GetModule(arg, module);

                    if (moduleInfo == null)
                    {
                        if (module == null)
                            break;

                        command = GetCommand(arg, module);
                        module = null;
                        break;
                    }
                    else
                        module = moduleInfo;
                }

                command ??= CommandService.Commands.Where(c => c.Name.ToLower() == path.ToLower()).FirstOrDefault();

                if (command != null)
                    await ReplyAsync(embed: BuildCommandHelp(command));

                else if (module != null)
                    await ReplyAsync(embed: BuildModuleHelp(module));

                else
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed($"Module or Command '{path}' not found!"));

                return;
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }


        private Embed BuildModuleHelp(ModuleInfo module)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Help - " + module.Name)
                .WithDescription(module.Summary);

            if (module.Submodules.Count > 0)
                embedBuilder.AddField("Submodules", string.Join(", ", module.Submodules.Select(m => m.Name)));

            embedBuilder.AddField("Commands", string.Join('\n', module.Commands.Select(c => $"`{c.Name}`: {c.Summary}")));

            return embedBuilder.Build();
        }

        private Embed BuildCommandHelp(CommandInfo command)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Help - " + command.Name)
                .WithDescription(command.Summary);

            // Aliases
            if (command.Aliases.Count > 0)
                embedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(a => $"`{a}`")));

            // Usage(s) // TODO: Implement multi overload commands
            embedBuilder.AddField("Usage(s)", string.Join('\n', GetUsageStrings(command)));

            // Arguments
            if (command.Parameters.Count > 0)
                embedBuilder.AddField("Parameters", string.Join(", ", command.Parameters.Select(p => $"`{p.Name}`: {p.Summary}")));

            return embedBuilder.Build();
        }

        private IEnumerable<string> GetParameterStrings(CommandInfo command)
        {
            List<string> parameterStrings = new List<string>();
            foreach (var parameter in command.Parameters)
            {
                if (parameter.IsOptional)
                    parameterStrings.Add($"[{parameter.Name} = {parameter.DefaultValue}]");

                else if (parameter.IsMultiple)
                    parameterStrings.Add($"|{parameter.Name}|");

                else if (parameter.IsRemainder)
                    parameterStrings.Add($"...{parameter.Name}");

                else
                    parameterStrings.Add(parameter.Name);
            }

            return parameterStrings;
        }

        private IEnumerable<string> GetUsageStrings(CommandInfo command)
        {
            var output = new List<string>();

            var commandPath = $"`{GetCommandPrefixString(command)}`  ";
            var parameterList = string.Join(' ', GetParameterStrings(command).Select(p => $"`<{p}>`"));

            output.Add(commandPath + parameterList);

            return output;
        }

        private string GetCommandPrefixString(CommandInfo command)
        {
            #region Module Prefix
            ModuleInfo currentModule = command.Module;
            Stack<ModuleInfo> moduleStack = new Stack<ModuleInfo>();

            do
            {
                moduleStack.Push(currentModule);
                currentModule = currentModule.Parent;
            }
            while (currentModule != null);

            string output = string.Join(' ', moduleStack.Select(m => m.Name));
            #endregion

            output += $" {command.Name}";
            return output;
        }

        private ModuleInfo GetModule(string arg, ModuleInfo parent)
        {
            return CommandService.Modules
                .Where(m => m.Parent == parent &&
                    (m.Name.ToLower() == arg.ToLower() || m.Submodules.Select(m => m.Name.ToLower()).Contains(arg.ToLower())))
                .FirstOrDefault();
        }

        private CommandInfo GetCommand(string arg, ModuleInfo parent)
        {
            return CommandService.Commands
                .Where(c => c.Module == parent && c.Name.ToLower() == arg.ToLower())
                .FirstOrDefault();
        }
    }
}