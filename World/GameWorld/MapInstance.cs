using Database.Helper;
using Database.MapEntity;
using Database.World;
using Enum.Main.PlayerEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.GameWorld.Objects;
using World.Network;
using World.Utils;

namespace World.GameWorld
{
    public class MapInstance
    {
        public short Id => Template.Id;
        public Guid InstanceId { get; } = new Guid();
        public WorldMap Template { get; }
        public List<Player> Players { get; } = new List<Player>();
        public List<Portal> Portals { get; set; } = new List<Portal>();

        /// <summary>
        /// MonsterEntities must be used.
        /// </summary>
        public List<MonsterEntity> MonsterEntities { get; set; } = new List<MonsterEntity>();

        /// <summary>
        /// NpcEntities must be used.
        /// </summary>
        public List<NpcEntity> NpcEntities { get; set; } = new List<NpcEntity>();

        /// <summary>
        /// List of dropped items on the map
        /// </summary>
        public List<MapItem> Items { get; set; } = new List<MapItem>();

        public MapInstance(WorldMap template)
        {
            Template = template;
        }

        public void AddPlayer(Player player)
        {
            if (!Players.Contains(player)) Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            if (Players.Contains(player)) Players.Remove(player);
        }

        public async Task GenerateShops(ClientSession session)
        {
            var packets = new List<string>();

            foreach (var npc in WorldManager.Npcs.Where(x => x.MapId == session.Player.CurrentMap.Template.Id))
            {
                var shop = await WorldDbHelper.LoadShopByNpcId(npc.NpcId);
                if (shop is null)
                {
                    continue;
                }

                var translation = WorldManager.ShopTranslations.FirstOrDefault(x => x.Language == session.Account.Language && x.ShopId == shop.ShopId);

                
                packets.Add($"shop 2 {npc.NpcId} {shop.ShopId} {shop.MenuType} {shop.ShopType} {translation.Name}");
            }
            await session.SendPackets(packets);
        }

        public async Task Broadcast(string packet, ClientSession except = null)
        {
            if (Players is null) return;
            foreach (var player in Players)
            {
                if (player.Session != except)
                {
                    await player.Session.SendPacket(packet);
                }
            }
        }

        public async Task GetMapItems(ClientSession session)
        {
            var packet = string.Empty;
            foreach (var item in Items.Where(x => x.MapId == session.Player.CurrentMap.Template.Id))
            {
                packet = $"in 9 {item.ItemId} {item.Id} {item.X} {item.Y} {item.Amount} 0 0 0 0 0 - 0";
                await session.SendPacket(packet);
            }
        }

        public void RemoveMapItem(MapItem item)
        {
            Items.Remove(item);
        }

        public Coords GetRandomCoord()
        {
            return Template.GetRandomCoord();
        }
    }
}