using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Configuration
{
    public class Config
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string OnJoin { get; set; }

        [JsonProperty("Lava")]
        public LavaConfig LavaConfig { get; set; }

        public MusicBotConfiguration MusicBot { get; set; }
    }

    public class MusicBotConfiguration
    {
        public ushort Volume;
        public bool IsRepeating;
        public ActivePlaylistInfo ActivePlaylist;
    }

    public class ActivePlaylistInfo
    {
        public int PlaylistId;
        public int CurrentTrackIndex;
    }
}
