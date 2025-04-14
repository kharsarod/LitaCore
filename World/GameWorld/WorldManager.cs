
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
        public static Dictionary<short, WorldMap> Maps { get; private set; } = new();
        private static readonly Dictionary<short, WorldMap> _baseMaps = new(); // <mapid, basemap>
        private static readonly Dictionary<short, MapInstance> _instances = new(); // <instanceid, map>
        public static readonly Dictionary<short, MapInstance> TemporalInstances = new(); // <instanceid, map>

        public static async Task LoadMaps()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var maps = await AppDbContext.LoadAllMapsAsync();
            foreach(var map in maps)
            {
                var worldMap = new WorldMap(map);
                Maps[map.Id] = worldMap;

                _instances[map.Id] = new MapInstance(worldMap);
            }
            stopwatch.Stop();

            Log.Information($"Loaded {Maps.Count()} maps in {stopwatch.ElapsedMilliseconds}ms.");
        }

        public static WorldMap GetWorldMap(short mapId)
        {
            if (Maps.TryGetValue(mapId, out var map))
            {
                return map;
            }
            return null;
        }

        public static WorldMap GetBaseMap(short mapId)
        {
            if (Maps.TryGetValue(mapId, out var map))
            {
                return map;
            }
            return null;
        }

        public static MapInstance CreateInstance(WorldMap map)
        {
            var instance = new MapInstance(map);
            TemporalInstances[map.Id] = instance;
            return instance;
        }

        public static MapInstance GetInstance(short mapId)
        {
            return _instances.ContainsKey(mapId) ? _instances[mapId] : null;
        }

        public static void RemoveInstance(short mapId)
        {
            if (TemporalInstances.ContainsKey(mapId))
                TemporalInstances.Remove(mapId);
        }
    }
}
