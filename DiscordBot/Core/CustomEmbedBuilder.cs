using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Core
{
    public static class CustomEmbedBuilder
    {
        public static Color ColorSuccess { get; } = Color.Green;
        public static Color ColorError { get; } = Color.Red;
        public static Color ColorInfo { get; } = Color.Blue;
        
        public static Embed NoPlayerEmbed = BuildErrorEmbed("No player found!");


        #region Error Embeds
        public static Embed BuildErrorEmbed(string message)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = "Error",
                Description = message,
                Color = ColorError
            };
            return embedBuilder.Build();
        }
        public static Embed BuildErrorEmbed(string title, string message)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = title,
                Description = message,
                Color = ColorError
            };
            return embedBuilder.Build();
        }
        #endregion



        #region Info Embeds
        public static Embed BuildInfoEmbed(string title, string message)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = title,
                Description = message,
                Color = ColorInfo
            };
            return embedBuilder.Build();
        }
        #endregion



        #region Success Embeds
        public static Embed BuildSuccessEmbed(string title, string message)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = title,
                Description = message,
                Color = ColorSuccess
            };
            return embedBuilder.Build();
        }
        public static Embed BuildSuccessEmbed(string message = null)
        {
            var embedBuilder = new EmbedBuilder
            {
                Description = message,
                Color = ColorSuccess
            };
            return embedBuilder.Build();
        }
        #endregion



        #region Track Embed
        public async static Task<Embed> BuildTrackEmbedAsync(LavaTrack track)
        {
            var embedBuilder = new EmbedBuilder
            {
                Title = track.Title,
                Author = new EmbedAuthorBuilder()
                {
                    Name = track.Author
                },
                Url = track.Url,
                ThumbnailUrl = await track.FetchArtworkAsync(),
                Color = ColorSuccess
            };
            return embedBuilder.Build();
        }
        #endregion
    }
}
