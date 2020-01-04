using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        public async static Task<Config> GetConfigAsync()
        {
            // TODO: Add exceptionhandling!
            var path = "config.json";
            var jsonString = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<Config>(jsonString);
        }
    }
}
