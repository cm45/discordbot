using Discord;
using Discord.Commands;
using DiscordBot.Core;
using DiscordBot.EscapeFromTarkovAPI;
using DiscordBot.EscapeFromTarkovAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("eft")]
    public class EscapeFromTarkovModule : ModuleBase<SocketCommandContext>
    {
        public TarkovAPI TarkovAPI { get; set; }

        [Command]
        public async Task Query()
        {
            // Look for all stuff.. Weapons, Ammo, Maps, Calibers, Traders, Items, Quests, etc...
            await ReplyAsync("Full db query... WIP");
        }

        [Command("maps")]
        public async Task Maps()
        {
            await ReplyAsync("test");
        }

        [Group("weapons")]
        public class Weapons : ModuleBase<SocketCommandContext>
        {
            public TarkovAPI TarkovAPI { get; set; }

            [Command]
            public async Task AllWeapons()
            {
                var weapons = TarkovAPI.GetWeapons();

                var msg = "";
                foreach (var weapon in weapons)
                {
                    msg += weapon.Name + "\n";
                }

                await ReplyAsync(msg);
            }

            [Command]
            public async Task GetWeapon([Remainder] string query)
            {
                query = query.RemoveWhitespacesAndSeperators().ToLower();

                Console.WriteLine("Looking for a weapon with query: " + query);

                var weapons = TarkovAPI.Context.Weapons.AsEnumerable()
                    .Where(s => s.Name.RemoveWhitespacesAndSeperators().ToLower().Contains(query));

                var message = "";

                foreach (var weapon in weapons)
                {
                    message += weapon.Name + '\n';
                    
                    var embedBuilder = new EmbedBuilder
                    {
                        Author = new EmbedAuthorBuilder { Name = "Escape from Tarkov" },
                        Color = Color.DarkBlue,
                        Title = weapon.Name
                    };

                    if (weapon.Caliber == null)
                    {
                        await ReplyAsync("Caliber is null");
                        return;
                    }

                    embedBuilder.Description += $"Caliber: **{weapon.Caliber.Name}**";
                    embedBuilder.Description += "Ammos:\n";
                    
                    foreach (var ammo in weapon.Caliber.Ammos)
                    {
                        var projectileCountString = ammo.Projectiles > 1 ? ammo.Projectiles.ToString() + "x" : "";

                        embedBuilder.Description += $"**{ammo.Name}**";

                        embedBuilder.Description += $"**Flesh Damage:** {projectileCountString + ammo.FleshDamage}\n";
                        embedBuilder.Description += $"**PenetrationPower:** {ammo.PenetrationPower}\n";
                        embedBuilder.Description += $"**Recoil:** {ammo.Recoil}\n";
                        embedBuilder.Description += $"**Accuracy:** {ammo.AccuracyPercentage}%\n";
                        embedBuilder.Description += $"**Fragmentation Chance:** {ammo.FragmentationChancePercentage}%\n";
                    }

                    var embed = embedBuilder.Build();
                    await ReplyAsync(embed: embed);
                }

                if (message != string.Empty)
                    await ReplyAsync(message);
                else
                    await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed($"Couldn't find any weapon with query: " + query));
            }
        }
    }
}
