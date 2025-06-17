using Database.MapEntity;
using Database.ShopEntity;
using Enum.Main.ChatEnum;
using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class ShoppingHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("shopping", HandleShopping);
            handler.Register("buy", HandleBuy);
            handler.Register("sell", HandleSell);
        }

        public static async Task HandleShopping(ClientSession session, string[] parts)
        {
            var type = byte.Parse(parts[2]);
            var unknownValue1 = int.Parse(parts[3]);
            var entityId = int.Parse(parts[5]);
            var shopType = 0; // 0 = normal, 1 = skills

            var npc = session.Player.CurrentMap.NpcEntities.FirstOrDefault(x => x.NpcId == entityId);
            if (npc is null) return;

            StringBuilder packet = new StringBuilder();
            packet.Append($"n_inv 2 {npc.NpcId} {shopType} 100");

            var shop = WorldManager.Shops.FirstOrDefault(x => x.NpcId == npc.NpcId);
            if (shop is null) return;

            foreach (var item in WorldManager.ShopItems.Where(x => x.ShopId == shop.ShopId && x.Window == type))
            {
                var getItem = WorldManager.GetItem(item.ItemId);

                var price = getItem.ItemType != ItemType.WEAPON && getItem.ItemType != ItemType.SPECIALIST && getItem.ItemType != ItemType.ARMOR ? item.Price : item.Upgrade;
                var val = getItem.ItemType == ItemType.WEAPON || getItem.ItemType == ItemType.SPECIALIST || getItem.ItemType == ItemType.ARMOR ? item.Upgrade : 0;
                var valueType = getItem.ItemType == ItemType.SPECIALIST ? 0 : getItem.ItemType;

                if ((byte)getItem.Type != 0)
                {
                    packet.Append($" {(byte)getItem.Type}.{item.Slot}.{item.ItemId}.{((byte)getItem.Type != 0 ? -1 : item.Rarity)}.{item.Price}");
                }
                else
                {
                    packet.Append($" {(byte)getItem.Type}.{item.Slot}.{item.ItemId}.{((byte)getItem.Type != 0 ? -1 : item.Rarity)}.{val}.{item.Price}");
                }

              //  await session.Player.ChatSay($"{(byte)getItem.Type}.{item.Slot}.{item.ItemId}.{((byte)getItem.Type != 0 ? -1 : item.Rarity)}.{val}.{item.Price}", ChatColor.Yellow);
            }

            await session.SendPacket(packet.ToString());
        }

        public static async Task HandleBuy(ClientSession session, string[] parts)
        {
            var type = byte.Parse(parts[2]);
            var entityId = int.Parse(parts[3]);
            var fromSlot = int.Parse(parts[4]);
            var amount = int.Parse(parts[5]);

            var npc = session.Player.CurrentMap.NpcEntities.FirstOrDefault(x => x.NpcId == entityId);
            if (npc is null) return;

            var shop = WorldManager.Shops.FirstOrDefault(x => x.NpcId == npc.NpcId);
            if (shop is null) return;

            var item = WorldManager.ShopItems.FirstOrDefault(x => x.ShopId == shop.ShopId && x.Slot == fromSlot);
            if (item is null) return;

            var getItem = WorldManager.GetItem(item.ItemId);

            if (session.Player.Gold < item.Price || session.Player.Gold < item.Price * amount)
            {
                await session.SendPacket($"s_memoi 2 {(int)MessageId.SHOP_ITEM_NOT_ENOUGH_GOLD} 0");
                return;
            }

            if (session.Player.Inventory.IsInventoryFull(getItem.Type))
            {
                await session.SendPacket($"s_memoi 2 {(int)MessageId.SHOP_ITEM_INVENTORY_FULL} 0");
                return;
            }

            await session.Player.ReduceGold(item.Price * amount);
            await session.SendPacket($"s_memoi 0 {(int)MessageId.SHOP_ITEM_SUCCESS} 0");
            await session.Player.Inventory.AddItemToInventory(session.Player.Id, item.ItemId, (short)amount, item.Rarity, item.Upgrade);
        }

        public static async Task HandleSell(ClientSession session, string[] parts)
        {
            // sell 2 1 2 47 1 0
            var type = byte.Parse(parts[2]);
            var entityId = int.Parse(parts[3]);
            var fromInvType = (InventoryType)int.Parse(parts[4]);
            var fromSlot = int.Parse(parts[5]);
            var amount = int.Parse(parts[6]);

            var npc = session.Player.CurrentMap.NpcEntities.FirstOrDefault(x => x.NpcId == entityId);
            if (npc is null) return;

            var shop = WorldManager.Shops.FirstOrDefault(x => x.NpcId == npc.NpcId);
            if (shop is null) return;

            var item = await session.Player.Inventory.GetItemFromSlot(fromSlot, fromInvType);
            if (item is null) return;

            var getItem = WorldManager.GetItem(item.ItemId);

            if (!getItem.IsSoldable && !getItem.IsLimited)
            {
                await session.SendPacket($"s_memoi 2 {(int)MessageId.SHOP_ITEM_CANNOT_SELL} 0");
                return;
            }

            if (getItem.SellToNpcPrice < 0)
            {
                getItem.SellToNpcPrice = 1;
            }

            await session.SendPacket($"s_memoi 0 {(int)MessageId.SHOP_ITEM_SUCCESS} 0");
            await session.Player.AddGold(getItem.SellToNpcPrice * amount);
            await session.Player.Inventory.DeleteItemFromInventory(getItem.Id, (short)amount);
        }
    }
}