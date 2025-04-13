
using Database.World;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.GameWorld;

namespace GameWorld
{
    public static class WorldManager
    {
        public static WorldMap Map { get; set; }

        public static async Task LoadMaps()
        {
            Map = new();
            Map.Maps = new Dictionary<short, WorldMap>();
            var maps = await AppDbContext.LoadAllMapsAsync();
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach(var map in maps)
            {
                Map.Maps[map.Id] = new WorldMap
                {
                    Id = map.Id,
                    Name = map.Name,
                    Data = map.Data,
                    Bgm = map.Bgm,
                    IsShopAllowed = map.IsShopAllowed,
                    IsPvpAllowed = map.IsPvpAllowed,
                    GoldRate = map.GoldRate,
                    ExpRate = map.ExpRate,
                    DropRate = map.DropRate
                };
            }
            stopwatch.Stop();

            Log.Information($"Loaded {Map.Maps.Count()} maps in {stopwatch.ElapsedMilliseconds}ms.");
        }

        public static WorldMap GetWorldMap(short mapId)
        {
            if (Map.Maps.TryGetValue(mapId, out var map))
            {
                return map;
            }
            return null;
        }

    }
}
