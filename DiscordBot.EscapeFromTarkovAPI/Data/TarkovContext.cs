using DiscordBot.EscapeFromTarkovAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DiscordBot.EscapeFromTarkovAPI.Data
{
    public class TarkovContext : DbContext
    {
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Caliber> Calibers { get; set; }
        public DbSet<Ammo> Ammos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Save om Config: https://docs.asp.net/en/latest/fundamentals/configuration.html und https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings
            var serverString = @"Server=(localdb)\MSSQLLocalDB;";
            var databaseNameString = @"Database=tarkov;";
            var securityString = @"Integrated Security=true;";

            var connectionString = serverString + databaseNameString + securityString;

            optionsBuilder.UseSqlServer(connectionString);
        }

        public TarkovContext()
        {

        }

        public TarkovContext(DbContextOptions options) : base(options)
        {

        }
    }
}
