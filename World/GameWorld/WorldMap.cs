using Database.World;
using Enum.Main.ChatEnum;
using Enum.Main.MapEnum;
using Newtonsoft.Json.Linq;
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
        }

        public async Task OnPlayerEnter(Player player)
        {
            Players.Add(player.Id, player);
        }

        public async Task OnPlayerLeave(Player player)
        {
            Players.Remove(player.Id);
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

    }
}
