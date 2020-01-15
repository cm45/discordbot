using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Services
{
    public class PlaylistService
    {
        public const string DatabasePath = @"Data/Playlists.db";

        #region Get Playlist/s
        public Task<IEnumerable<Playlist>> GetPlaylists()
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                return Task.FromResult(collection.FindAll());
            }
        }
        public Task<Playlist> GetPlaylist(string name)
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                return Task.FromResult(collection.FindOne(p => p.Name == name));
            }
        }
        public Task<Playlist> GetPlaylist(int id)
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                return Task.FromResult(collection.FindOne(p => p.Id == id));
            }
        }
        #endregion

        #region Remove Playlist
        public Task<bool> RemovePlaylist(Playlist playlist)
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                var deletedCount = collection.Delete(p => p.Id == playlist.Id);
                return deletedCount > 0 ? Task.FromResult(true) : Task.FromResult(false);
            }
        }
        public Task<bool> RemovePlaylist(string name)
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                var deletedCount = collection.Delete(p => p.Name == name);
                return deletedCount > 0 ? Task.FromResult(true) : Task.FromResult(false);
            }
        }
        public Task<bool> RemovePlaylist(int id)
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                var deletedCount = collection.Delete(p => p.Id == id);
                return deletedCount > 0 ? Task.FromResult(true) : Task.FromResult(false);
            }
        }
        #endregion

        public Task SavePlaylist(Playlist playlist)
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                collection.Upsert(playlist);
            }

            return Task.CompletedTask;
        }

        public Task<Playlist> CreatePlaylist(string name)
        {
            using var db = new LiteDatabase(DatabasePath);

            var collection = db.GetCollection<Playlist>("playlists");
            var playlist = new Playlist(name, new List<LavaTrack>());

            collection.Insert(playlist);

            return Task.FromResult(playlist);
        }
    }

    public class Playlist
    {
        private string DatabasePath => PlaylistService.DatabasePath;

        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<LavaTrack> Tracks { get; set; }

        public Playlist(string name, IEnumerable<LavaTrack> tracks)
        {
            Name = name;
            Tracks = tracks;
        }

        public Playlist() { } // LiteDB needs this!

        public Task AddTrack(LavaTrack track)
        {
            Tracks = Tracks.Append(track);
            Console.WriteLine($"Added {track.Title} to playlist '{Name}'!");
            
            SavePlaylist();

            return Task.CompletedTask;
        }

        public Task AddTracks(IEnumerable<LavaTrack> tracks)
        {
            Tracks = Tracks.Concat(tracks);
            Console.WriteLine($"Added {tracks.Count()} tracks to playlist '{Name}'!");

            SavePlaylist();

            return Task.CompletedTask;
        }

        public Task RemoveTrack(LavaTrack track)
        {
            Tracks = Tracks.Except(new List<LavaTrack> { track });
            SavePlaylist();

            return Task.CompletedTask;
        }

        private Task SavePlaylist()
        {
            using (var db = new LiteDatabase(DatabasePath))
            {
                var collection = db.GetCollection<Playlist>("playlists");
                collection.Update(this);
                Console.WriteLine($"Updated playlist '{Name}'!");
            }

            return Task.CompletedTask;
        }
    }
}
