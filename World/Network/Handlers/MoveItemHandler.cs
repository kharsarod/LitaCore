using Database.Helper;
using Enum.Main.ChatEnum;
using Enum.Main.ItemEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class MoveItemHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("mvi", HandleMove);
            handler.Register("mve", HandleMove);
        }

        public static async Task HandleMove(ClientSession session, string[] parts)
        {
            // mvi {inventory_type} {from_slot} {amount} {to_slot}
            string packet = parts[1];
            if (packet.StartsWith("mvi"))
            {
                var invType = byte.Parse(parts[2]);
                var fromSlot = int.Parse(parts[3]);
                var amount = short.Parse(parts[4]);
                var toSlot = short.Parse(parts[5]);

                var inventory = session.Player.Inventory;
                var replaceItem = await inventory.GetItemFromSlot(toSlot, (InventoryType)invType);
                var item = await inventory.GetItemFromSlot(fromSlot, (InventoryType)invType);

                if (item is null)
                {
                    return;
                }

                if (session.Player.LastMoveItem.AddSeconds(1.5) > DateTime.Now)
                {
                    await session.Player.ChatSay("Item move in cooldown.", ChatColor.Red);
                    return;
                }

                string removeItemMoveSourcePacket = $"ivn {invType} {fromSlot}.{(replaceItem is null ? 0 : replaceItem.ItemId)}.0.0.0.0.0";

                string moveItemFromSource = string.Empty;

                await session.SendPacket(removeItemMoveSourcePacket);

                if (item.InventoryType == InventoryType.EQUIPMENT)
                {
                    moveItemFromSource = $"ivn {invType} {toSlot}.{item.ItemId}.{item.Rarity}.{item.Upgrade}.0.0.0";
                    item.Slot = toSlot;
                    if (replaceItem is not null)
                    {
                        replaceItem.Slot = fromSlot;
                        await CharacterDbHelper.UpdateAsync(replaceItem);
                    }
                    await CharacterDbHelper.UpdateAsync(item);

                    await session.SendPacket(moveItemFromSource);
                }
                else
                {
                    moveItemFromSource = $"ivn {invType} {toSlot}.{item.ItemId}.{item.Amount}.0";
                    item.Slot = toSlot;
                    if (replaceItem is not null)
                    {
                        replaceItem.Slot = fromSlot;
                        await CharacterDbHelper.UpdateAsync(replaceItem);
                    }
                    await CharacterDbHelper.UpdateAsync(item);

                    await session.SendPacket(moveItemFromSource);
                }
            }
            else if (packet.StartsWith("mve"))
            {
                var invType = byte.Parse(parts[2]);
                var fromSlot = int.Parse(parts[3]);
                var toInvType = byte.Parse(parts[4]);
                var toSlot = int.Parse(parts[5]);

                var inventory = session.Player.Inventory;
                var replaceItem = await inventory.GetItemFromSlot(toSlot, (InventoryType)toInvType);
                var item = await inventory.GetItemFromSlot(fromSlot, (InventoryType)invType);
                if (item is null)
                {
                    return;
                }

                if (session.Player.LastMoveItem.AddSeconds(1.5) > DateTime.Now)
                {
                    await session.Player.ChatSay("Item move in cooldown.", ChatColor.Red);
                    return;
                }

                string removeItemMoveSourcePacket = $"ivn {invType} {fromSlot}.{(replaceItem is null ? 0 : replaceItem.ItemId)}.0.0.0.0.0";
                string moveItemFromSource = string.Empty;
                await session.SendPacket(removeItemMoveSourcePacket);

                moveItemFromSource = $"ivn {toInvType} {toSlot}.{item.ItemId}.{item.Rarity}.{item.Upgrade}.0.0.0";
                item.Slot = toSlot;
                item.InventoryType = (InventoryType)toInvType;
                if (replaceItem is not null)
                {
                    replaceItem.Slot = fromSlot;
                    await CharacterDbHelper.UpdateAsync(replaceItem);
                }
                await CharacterDbHelper.UpdateAsync(item);
                await session.SendPacket(moveItemFromSource);
            }

            session.Player.LastMoveItem = DateTime.Now;
        }

        public static async Task HandleMoveSpecialist(ClientSession session, string[] parts)
        {
            // mvi {inventory_type} {from_slot} {to_inventory_type} {to_slot}
            // session.Player.LastMoveItem = DateTime.Now;
        }
    }
}