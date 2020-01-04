using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot
{
    public class Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("onJoin")]
        public string OnJoin { get; private set; }

        [JsonProperty("lava")]
        public LavaConfig LavaConfig { get; private set; }


        private static Config configCache;
        public static Config ConfigCache
        {
            get
            {
                if (configCache == null)
                    configCache = GetConfigAsync().GetAwaiter().GetResult();

                return configCache;
            }
            set => configCache = value;
        }
        public async static Task<Config> GetConfigAsync()
        {
            // TODO: Add exceptionhandling!
            var path = "config.json";
            var jsonString = await File.ReadAllTextAsync(path);
            var config = JsonConvert.DeserializeObject<Config>(jsonString);
            ConfigCache = config;
            return config;
        }
    }
}
