using Discord.Commands;
using DiscordBot.Core;
using DiscordBot.Services.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Modules.Music
{
    [Group("playlist"), Alias("pl"), Name("Playlist")]
    public class PlaylistModule : ModuleBase<SocketCommandContext>
    {
        public MusicService MusicService { get; set; }
        public PlaylistService PlaylistService { get; set; }

        #region Play

        [Command("play")]
        public async Task PlayPlaylistAsync(int id)
        {
            Console.WriteLine("Get playlist via ID");
            var playlist = await PlaylistService.GetPlaylist(id);

            if (playlist == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist not found!"));
                return;
            }

            if (playlist.Tracks.Count() <= 0)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist is empty!"));
                return;
            }

            await MusicService.PlayPlaylistAsync(playlist);
            await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Successfully added the playlist '{playlist.Name}' with {playlist.Tracks?.Count()} tracks into the queue!"));
        }

        [Command("play")]
        public async Task PlayPlaylistAsync(string name)
        {
            Console.WriteLine("Get playlist via name");
            var playlist = await PlaylistService.GetPlaylist("name");

            if (playlist == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist not found!"));
                return;
            }

            if (playlist.Tracks.Count() <= 0)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Playlist is empty!"));
                return;
            }

            await MusicService.PlayPlaylistAsync(playlist);
        }

        #endregion

        #region Query
        [Command, Alias("all")]
        public async Task ListAllPlaylistsAsync()
        {
            var playlists = await PlaylistService.GetPlaylists();

            if (playlists.Count() <= 0)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("No playlists found!"));
                return;
            }

            var message = "";
            foreach (var playlist in playlists)
                message += $"[{playlist.Id}] - {playlist.Name} ({playlist.Tracks.Count()} tracks)\n";

            await ReplyAsync(embed: CustomEmbedBuilder.BuildInfoEmbed($"Found {playlists.Count()} playlists!", message));
        }
        [Command]
        public async Task GetPlaylist(int id)
        {
            var playlist = await PlaylistService.GetPlaylist(id);

            var maxTrackviewCount = 10;

            var message = "Tracks:\n";
            for (int i = 0; i < Math.Min(playlist.Tracks.Count(), maxTrackviewCount); i++)
                message += $"**{i + 1})** {playlist.Tracks.ElementAt(i).Title}\n";

            if (playlist.Tracks.Count() > maxTrackviewCount)
                message += $"*And {playlist.Tracks.Count() - maxTrackviewCount} more...*";

            await ReplyAsync(embed: CustomEmbedBuilder.BuildInfoEmbed($"Found playlist '{playlist.Name}'!", message.Trim('\n')));
        }
        [Command]
        public async Task GetPlaylist(string name)
        {
            var playlist = await PlaylistService.GetPlaylist(name);

            var message = "Tracks:\n";
            for (int i = 0; i < Math.Min(playlist.Tracks.Count(), 10); i++)
                message += $"**{i + 1})** {playlist.Tracks.ElementAt(i).Title}\n";

            await ReplyAsync(embed: CustomEmbedBuilder.BuildInfoEmbed($"Found playlist '{playlist.Name}'!", message.Trim('\n')));
        }
        #endregion

        #region Remove
        [Command("destroy"), Alias("d")]
        public async Task DestroyPlaylistAsync(int id)
        {
            if (await PlaylistService.RemovePlaylist(id))
                await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Removed playlist with id {id}!"));
            else
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed($"Failed to remove playlist with id {id}!"));
        }

        [Command("destroy"), Alias("d")]
        public async Task DestroyPlaylistAsync(string name)
        {
            if (await PlaylistService.RemovePlaylist(name))
                await ReplyAsync(embed: CustomEmbedBuilder.BuildSuccessEmbed($"Removed playlist with name '{name}'!"));
            else
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed($"Failed to remove playlist with name '{name}'!"));
        }
        #endregion


        #region Add Track/s

        [Command("addtrack")]
        public async Task AddTracksToPlaylistAsync(string playlistName, [Remainder] string query)
        {
            var playlist = await PlaylistService.GetPlaylist(playlistName);
            var tracks = await MusicService.GetTracksAsync(query);

            await playlist.AddTracks(tracks);
        }

        [Command("addtrack")]
        public async Task AddTracksToPlaylistAsync(int playlistId, [Remainder] string query)
        {
            var playlist = await PlaylistService.GetPlaylist(playlistId);
            var tracks = await MusicService.GetTracksAsync(query);

            await playlist.AddTracks(tracks);
        }

        #endregion

        [Command("skip"), Alias("next", "n")] public async Task SkipAsync() => await ReplyAsync(embed: await MusicService.SkipAsync());

        [Command("create")]
        public async Task CreatePlaylistAsync([Remainder] string name)
        {
            var playlist = await PlaylistService.CreatePlaylist(name);
            await ReplyAsync($"Created playlist called '{playlist.Name}'!");
        }

        [Command("save")]
        public async Task SaveQueueAsPlaylistAsync([Remainder] string playlistName)
        {
            if (MusicService.Player == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.NoPlayerEmbed);
                return;
            }

            var tracks = new List<LavaTrack>();

            if (MusicService.Player.Track != null && tracks != null)
                tracks.Add(MusicService.Player.Track);

            tracks.AddRange(MusicService.Player.Queue.Items.Cast<LavaTrack>());

            if (tracks.Count <= 0 && MusicService.Player.Track == null)
            {
                await ReplyAsync(embed: CustomEmbedBuilder.BuildErrorEmbed("Current queue is empty!"));
                return;
            }

            var playlist = new Playlist(playlistName, tracks);

            await PlaylistService.SavePlaylist(playlist);
            await ReplyAsync($"Saved current queue as playlist ({playlistName} with {playlist.Tracks.Count()} tracks)!");
        }
    }
}
