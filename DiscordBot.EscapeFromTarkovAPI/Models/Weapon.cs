﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBot.EscapeFromTarkovAPI.Models
{
    public class Weapon
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public Caliber Caliber { get; set; }
    }
}
