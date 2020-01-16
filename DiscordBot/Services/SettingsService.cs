using DiscordBot.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class SettingsService
    {
        private const string ConfigPath = "Configurations/config.json";

        public Config Config { get; private set; }

        // TODO:
        public SettingsService()
        {
            Config = LoadConfigAsync().GetAwaiter().GetResult();
        }


        private async Task<Config> LoadConfigAsync()
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(ConfigPath);
                var config = JsonConvert.DeserializeObject<Config>(jsonString);
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't load config!\n" + ex.Message);
                throw ex;
            }
        }
        public async Task SaveConfigAsync()
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(Config, Formatting.Indented);
                await File.WriteAllTextAsync(ConfigPath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't save config!\n" + ex.Message);
                throw ex;
            }
        }



        #region MusicBot
        public async Task SetVolumeAsync(ushort value)
        {
            Config.MusicBot.Volume = value;
            await SaveConfigAsync();
        }

        public async Task SetRepeatAsync(bool value)
        {
            Config.MusicBot.IsRepeating = value;
            await SaveConfigAsync();
        }
        #endregion
    }
}
