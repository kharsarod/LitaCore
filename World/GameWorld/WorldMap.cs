using Database.MapEntity;
using Database.Migrations.WorldDb;
using Database.World;
using Enum.Main.ChatEnum;
using Enum.Main.MapEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Utils;

namespace World.GameWorld
{
    public class WorldMap : Map
    {
        public byte[] WalkData { get; set; }
        public List<Coords> _cellCoords;
        private Random _random = new Random();
        public Guid InstanceId { get; } = new Guid();
        public Dictionary<int, Player> Players { get; set; } // <PlayerCharacterId, Player>
        public List<Portal> Portals { get; set; }
        public List<Monster> Monsters { get; set; }

        public WorldMap(Map map)
        {
            Id = map.Id;
            Name = map.Name;
            MapGrid = map.MapGrid;
            Bgm = map.Bgm;
            IsShopAllowed = map.IsShopAllowed;
            IsPvpAllowed = map.IsPvpAllowed;
            GoldRate = map.GoldRate;
            ExpRate = map.ExpRate;
            DropRate = map.DropRate;
            Width = map.Width;
            Height = map.Height;
            Players = new();
            Portals = new();
            Monsters = new();
        }

        public short GetPlayersInMap() => (short)Players.Count;

        public Coords GetRandomCoord()
        {
            if (_cellCoords != null && _cellCoords.Count > 0)
                return _cellCoords[_random.Next(_cellCoords.Count - 1)];

            _cellCoords = new List<Coords>();

            for (short y = 0; y < Height; y++)
            {
                for (short x = 0; x < Width; x++)
                {
                    if (PathFindingMap.IsWalkable(x, y, MapGrid, Width, Height) && !PathFindingMap.IsBlockedZone(x, y, MapGrid))
                    {
                        _cellCoords.Add(new Coords(x, y));
                    }
                }
            }

            if (_cellCoords.Count == 0)
                return new Coords(0, 0);

            return _cellCoords[_random.Next(_cellCoords.Count)];
        }

        public int GetDistance(Coords start, Coords end)
        {
            double x = Math.Abs(start.MapPosX - end.MapPosX);
            double y = Math.Abs(start.MapPosY - end.MapPosY);

            return (int)Math.Sqrt(x * x + y * y);
        }
    }
}