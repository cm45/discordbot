using DiscordBot.EscapeFromTarkovAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
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
            // TODO: Save in Config: https://docs.asp.net/en/latest/fundamentals/configuration.html und https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings

            // Local
            //var serverString = @"Server=(localdb)\MSSQLLocalDB;";
            //var databaseNameString = @"Database=tarkov;";
            //var securityString = @"Integrated Security=true;";

            // Remote linux
            try
            {
                var connectionString = File.ReadAllText("connectionString");
                optionsBuilder.UseSqlServer(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public TarkovContext()
        {

        }

        public TarkovContext(DbContextOptions options) : base(options)
        {

        }
    }
}
