using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;

namespace World.GameWorld.Objects
{
    public class MapItem
    {
        private static int _id = 1;
        private static readonly object _lock = new();
        public int Id { get; set; }
        public short MapId { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short ItemId { get; set; }
        public short Amount { get; set; }
        public GameEntity Owner { get; set; }

        public MapItem(short mapId, short x, short y, short itemId, short amount, GameEntity owner, MapInstance instance = null)
        {
            lock (_lock)
            {
                Id = _id++;
            }
            MapId = mapId;
            X = x;
            Y = y;
            ItemId = itemId;
            Amount = amount;
            Owner = owner;

            Observable.Timer(TimeSpan.FromSeconds(30)).Subscribe(_ =>
            {
                Owner = null;
            });

            Observable.Timer(TimeSpan.FromMinutes(1)).Subscribe(async _ =>
            {
                if (instance != null)
                {
                    await instance.Broadcast($"out 9 {Id}");
                    instance.RemoveMapItem(this);
                }
            });
        }
    }
}
