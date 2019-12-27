using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public struct Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
