using Database.Helper;
using Database.Item;
using Database.Player;
using Enum.Main.ItemEnum;
using GameWorld;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Gameplay.ItemBehaviors;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class UseItemHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("u_i", HandleUse);
            handler.RegisterPrefix("#u_i", HandleUsePrefix);
        }

        public static async Task HandleUse(ClientSession session, string[] parts)
        {
            var type = byte.Parse(parts[2]);
            var characterId = int.Parse(parts[3]);
            var fromInvType = (InventoryType)byte.Parse(parts[4]);
            var fromSlot = int.Parse(parts[5]);
            // 0 0 no sé que son

            if (type is 1)
            {
                switch (fromInvType)
                {
                    case InventoryType.SPECIALIST:
                        if (session.Player.UsingSpecialist && session.Player.Morph > 0) return;
                        await SwapItem(session, fromSlot, InventoryType.SPECIALIST, (byte)EquipmentType.SPECIALIST, InventoryType.WEAR);
                        await session.Player.Packets.GenerateEquipmentPacket();
                        break;

                    case InventoryType.MAIN:
                        var item = await session.Player.Inventory.GetItemFromSlot(fromSlot, InventoryType.MAIN);
                        var getItem = WorldManager.GetItem(item.ItemId);

                        var handler = ItemHandlerFactory.GetHandler(getItem.ItemType);
                        await handler.HandleUse(session, getItem);

                        break;
                }

                if (fromInvType == InventoryType.COSTUME)
                {
                    var item = await session.Player.Inventory.GetItemFromSlot(fromSlot, InventoryType.COSTUME);
                    var costume = WorldManager.GetItem(item.ItemId);

                    if (costume.EquipmentTypeSlot == EquipmentType.COSTUME_HAT)
                    {
                        await SwapItem(session, fromSlot, InventoryType.COSTUME, (byte)EquipmentType.COSTUME_HAT, InventoryType.WEAR);
                    }
                    else if (costume.EquipmentTypeSlot == EquipmentType.COSTUME_SUIT)
                    {
                        await SwapItem(session, fromSlot, InventoryType.COSTUME, (byte)EquipmentType.COSTUME_SUIT, InventoryType.WEAR);
                    }
                    await session.Player.Packets.GenerateEquipmentPacket();
                }
            }
        }

        public static async Task HandleUsePrefix(ClientSession session, string[] parts)
        {
            var splitter = parts[1].Split('^');
            var invType = (InventoryType)byte.Parse(splitter[1]);
            var charId = int.Parse(splitter[2]);
            var fromSlot = int.Parse(splitter[3]);
            var activatorType = int.Parse(splitter[4]);
            var unknownField05 = int.Parse(splitter[5]);

            var item = await session.Player.Inventory.GetItemFromSlot(fromSlot, invType);

            if (item is null) return;

            var getItem = WorldManager.GetItem(item.ItemId);
            if (getItem is null) return;

            if (activatorType == 2) // is mount i think.
            {
                await session.Player.TransformToMount(getItem.Model);
                await session.Player.AddSpeed(getItem.Speed);
            }
        }

        private static async Task SwapItem(ClientSession session, int fromSlot, InventoryType fromInventory, int targetSlot, InventoryType targetInventory)
        {
            var itemFromInv = await session.Player.Inventory.GetItemFromSlot(fromSlot, fromInventory);
            var equippedItem = await session.Player.Inventory.GetItemFromSlot(targetSlot, targetInventory);

            if (equippedItem is not null)
            {
                equippedItem.Slot = fromSlot;
                equippedItem.InventoryType = fromInventory;
                await session.SendPacket($"ivn {(byte)fromInventory} {fromSlot}.{equippedItem.ItemId}.{equippedItem.Rarity}.{equippedItem.Upgrade}.0.0.0");
                await CharacterDbHelper.UpdateAsync(equippedItem);
            }
            else
            {
                await session.SendPacket($"ivn {(byte)fromInventory} {fromSlot}.0.0.0.0.0.0");
            }

            itemFromInv.Slot = targetSlot;
            itemFromInv.InventoryType = targetInventory;
            await CharacterDbHelper.UpdateAsync(itemFromInv);
        }
    }
}