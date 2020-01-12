using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.EscapeFromTarkovAPI
{
    public static class TarkovLinks
    {
        public class Link
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string URL { get; set; }
            public string ToClickableLink() => $"[{Title}]({URL})";
        }

        //public static void AddLink(string title, string url)
        //{
        //    using (var db = new LiteDatabase(TarkovAPI.DbPath))
        //    {
        //        var collection = db.GetCollection<Link>("links");

        //        var link = new Link()
        //        {
        //            Title = title,
        //            URL = url
        //        };

        //        collection.Insert(link);
        //        collection.EnsureIndex(x => x.Title);
        //    }
        //}

        //public static Link[] GetLinks()
        //{
        //    using (var db = new LiteDatabase(TarkovAPI.DbPath))
        //    {
        //        var collection = db.GetCollection<Link>("links");
        //        var results = collection.FindAll();
        //        return results.ToArray();
        //    }
        //}

        //public static void RemoveLink(string title)
        //{
        //    using (var db = new LiteDatabase(TarkovAPI.DbPath))
        //    {
        //        var collection = db.GetCollection<Link>("links");
        //        collection.Delete(Query.EQ("Title", title));
        //    }
        //}
    }
}
