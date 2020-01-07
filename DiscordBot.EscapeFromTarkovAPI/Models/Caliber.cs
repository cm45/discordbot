using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.EscapeFromTarkovAPI.Models
{
    public class Caliber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Ammo> Ammos { get; set; }
    }
}
