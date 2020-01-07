using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.EscapeFromTarkovAPI.Models
{
    public class Ammo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FleshDamage { get; set; }
        public int Projectiles { get; set; } = 1;
        public int PenetrationPower { get; set; }
        public int AccuracyPercentage { get; set; } = 0;
        public int Recoil { get; set; } = 0;
        public int FragmentationChancePercentage { get; set; } = 0;

        public int ArmorEffectivenessAgainst1 { get; set; } = 0;
        public int ArmorEffectivenessAgainst2 { get; set; } = 0;
        public int ArmorEffectivenessAgainst3 { get; set; } = 0;
        public int ArmorEffectivenessAgainst4 { get; set; } = 0;
        public int ArmorEffectivenessAgainst5 { get; set; } = 0;
        public int ArmorEffectivenessAgainst6 { get; set; } = 0;
    }
}
