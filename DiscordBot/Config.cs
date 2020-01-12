using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot
{
    public class Config
    {
        private const string Path = "config.json";

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("onJoin")]
        public string OnJoin { get; set; }

        [JsonProperty("lava")]
        public LavaConfig LavaConfig { get; set; }

        [JsonProperty("volume")]
        public ushort Volume { get; set; }


        private static Config configCache;
        public static Config ConfigCache
        {
            get
            {
                if (configCache == null)
                    configCache = LoadConfigAsync().GetAwaiter().GetResult();

                return configCache;
            }
            set => configCache = value;
        }
        public static async Task<Config> LoadConfigAsync()
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(Path);
                var config = JsonConvert.DeserializeObject<Config>(jsonString);
                ConfigCache = config;
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't load config!\n" + ex.Message);
                throw ex;
            }
        }

        public static async Task SaveConfigAsync(Config config)
        {
            var jsonString = JsonConvert.SerializeObject(config);
            await File.WriteAllTextAsync(Path, jsonString);
            ConfigCache = config;
        }
    }
}
