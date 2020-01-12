using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.EscapeFromTarkovAPI
{
    public static class TarkovMaps
    {
        public class Map
        {
            public class VisualMap
            {
                public string Name { get; set; }
                public string URL { get; set; }
                public string ToClickableLink() => $"[{Name}]({URL})";
                public override string ToString() => $"{Name}, {URL}";
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public string URL { get; set; }
            public VisualMap[] VisualMaps { get; set; }
            public string ToClickableLink() => $"[{Name}]({URL})";
        }

        //public static void AddMap(Map map)
        //{
        //    try
        //    {
        //        using (var db = new LiteDatabase(TarkovAPI.DbPath))
        //        {
        //            var collection = db.GetCollection<Map>("maps");
        //            collection.Insert(map);
        //            collection.EnsureIndex(x => x.Name);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw ex;
        //    }
        //}

        //public static Map[] GetMaps()
        //{
        //    using (var db = new LiteDatabase(TarkovAPI.DbPath))
        //    {
        //        var collection = db.GetCollection<Map>("maps");
        //        var results = collection.FindAll();
        //        return results.ToArray();
        //    }
        //}
    }
}
