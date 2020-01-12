using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.EscapeFromTarkovAPI.Models
{
    public class Caliber
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        public ICollection<Ammo> Ammos { get; set; }
    }
}
