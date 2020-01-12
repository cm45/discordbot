using DiscordBot.EscapeFromTarkovAPI.Data;
using DiscordBot.EscapeFromTarkovAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.EscapeFromTarkovAPI
{
    public class TarkovAPI
    {
        public TarkovContext Context { get; private set; }

        public TarkovAPI()
        {
            Context = new TarkovContext();
        }

        public IEnumerable<Weapon> GetWeapons() => Context.Weapons.ToArray();
        public IEnumerable<Caliber> GetCalibers() => Context.Calibers.ToArray();
        public IEnumerable<Ammo> GetAmmos() => Context.Ammos.ToArray();
    }
}
