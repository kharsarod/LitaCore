using Database.Helper;
using Database.Player;
using Enum.Main.CharacterEnum;
using Enum.Main.ChatEnum;
using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class WearHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("wear", HandleWear);
            handler.Register("remove", HandleRemoveWear);
        }

        public static async Task HandleWear(ClientSession session, string[] parts)
        {
            var fromSlot = int.Parse(parts[2]);
            var invType = (InventoryType)byte.Parse(parts[3]);
            var item = await session.Player.Inventory.GetItemFromSlot(fromSlot, invType);
            var getItem = WorldManager.GetItem(item.ItemId);

            if (item is null) return;

            if (!CanUseItem(session.Player.Class, getItem.RequiredClass, getItem.RequiredClass == 14, getItem.RequiredClass == 30, getItem.RequiredClass == 31)
                && getItem.ItemType != ItemType.JEWELERY)
            {
                await session.Player.ChatSayById(MessageId.CANT_WEAR_ITEM, ChatColor.Yellow);

                if (session.Account.Rank == 3)
                {
                    await session.Player.ChatSay($"ClassId del item {getItem.RequiredClass}.", ChatColor.Yellow);
                }
                return;
            }

            if (session.Player.Level < getItem.LevelMinimum)
            {
                await session.Player.ChatSayById(MessageId.LOW_LEVEL_WEAR, ChatColor.Yellow);
                return;
            }

            switch (getItem.EquipmentTypeSlot)
            {
                case EquipmentType.ARMOR:
                    await SwapWithEquippedItem(session, item, EquipmentType.ARMOR);
                    item.Slot = (int)EquipmentType.ARMOR;
                    item.InventoryType = InventoryType.WEAR;
                    break;

                case EquipmentType.MAIN_WEAPON:
                    await SwapWithEquippedItem(session, item, EquipmentType.MAIN_WEAPON);
                    item.Slot = (int)EquipmentType.MAIN_WEAPON;
                    item.InventoryType = InventoryType.WEAR;
                    break;

                case EquipmentType.SECONDARY_WEAPON:
                    await SwapWithEquippedItem(session, item, EquipmentType.SECONDARY_WEAPON);
                    item.Slot = (int)EquipmentType.SECONDARY_WEAPON;
                    item.InventoryType = InventoryType.WEAR;
                    break;

                case EquipmentType.AMULET:
                    await SwapWithEquippedItem(session, item, EquipmentType.AMULET);
                    item.Slot = (int)EquipmentType.AMULET;
                    item.InventoryType = InventoryType.WEAR;
                    break;

                case EquipmentType.COSTUME_HAT:
                    await SwapWithEquippedItem(session, item, EquipmentType.COSTUME_HAT);
                    item.Slot = (int)EquipmentType.COSTUME_HAT;
                    item.InventoryType = InventoryType.WEAR;
                    break;

                case EquipmentType.COSTUME_SUIT:
                    await SwapWithEquippedItem(session, item, EquipmentType.COSTUME_SUIT);
                    item.Slot = (int)EquipmentType.COSTUME_SUIT;
                    item.InventoryType = InventoryType.WEAR;
                    break;

                case EquipmentType.FAIRY:
                    if (session.Player.UsingSpecialist)
                    {
                        var sp = session.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.SPECIALIST);
                        if (sp is not null)
                        {
                            var spItem = WorldManager.GetItem(sp.ItemId);
                            if (spItem is not null && spItem.Element != 0 && spItem.Element != getItem.Element)
                            {
                                await session.Player.SendMsgi(MessageId.SPECIALIST_DIFF_FAIRY);
                                return;
                            }
                        }
                    }

                    await SwapWithEquippedItem(session, item, EquipmentType.FAIRY);
                    item.Slot = (int)EquipmentType.FAIRY;
                    item.InventoryType = InventoryType.WEAR;
                    await session.Player.Packets.GeneratePairyPacket();
                    break;

                case EquipmentType.SPECIALIST:
                    if (session.Player.UsingSpecialist && session.Player.Morph > 0)
                    {
                        return;
                    }
                    await SwapWithEquippedItem(session, item, EquipmentType.SPECIALIST);
                    item.Slot = (int)EquipmentType.SPECIALIST;
                    item.InventoryType = InventoryType.WEAR;
                    break;
            }

            await CharacterDbHelper.UpdateAsync(item);

            await session.Player.Packets.GenerateEquipmentPacket();
            await session.SendPacket(session.Player.Packets.GenerateStat());
        }

        public static async Task HandleRemoveWear(ClientSession session, string[] parts)
        {
            var fromSlot = int.Parse(parts[2]);
            var toInvType = (InventoryType)byte.Parse(parts[3]);
            var item = session.Player.Inventory.GetEquippedItemFromSlot(fromSlot);

            if (item is null) return;

            // los tres ceros verificar luego.

            if (item.InventoryType == InventoryType.WEAR
                && item.Slot == (int)EquipmentType.SPECIALIST
                && session.Player.Morph > 0
                && session.Player.UsingSpecialist)
            {
                await session.Player.TransformToNormal();
            }

            if (item.InventoryType == InventoryType.WEAR
                && item.Slot == (int)EquipmentType.FAIRY)
            {
                await session.Player.CurrentMap.Broadcast($"pairy 1 {session.Player.Id} 0 0 0 0");
            }

            var nextSlot = session.Player.Inventory.GetNextFreeSlot(toInvType);
            item.InventoryType = toInvType;
            item.Slot = nextSlot;

            await CharacterDbHelper.UpdateAsync(item);
            await session.Player.Packets.GenerateEquipmentPacket();
            await session.Player.Packets.GenerateScPacket();
            await session.SendPacket($"ivn {(byte)toInvType} {nextSlot}.{item.ItemId}.{item.Rarity}.{item.Upgrade}.0.0.0");
            await session.SendPacket(session.Player.Packets.GenerateStat());
        }

        private static async Task SwapWithEquippedItem(ClientSession session, CharacterItem item, EquipmentType equipSlot)
        {
            var slot = (int)equipSlot;
            var equippedItem = await session.Player.Inventory.GetItemFromSlot(slot, InventoryType.WEAR);

            if (equippedItem != null)
            {
                equippedItem.Slot = item.Slot;
                equippedItem.InventoryType = item.InventoryType;

                await session.SendPacket($"ivn {(byte)item.InventoryType} {equippedItem.Slot}.{equippedItem.ItemId}.{equippedItem.Rarity}.{equippedItem.Upgrade}.0.0.0");
                await CharacterDbHelper.UpdateAsync(equippedItem);
            }
            else
            {
                await session.SendPacket($"ivn {(byte)item.InventoryType} {item.Slot}.0.0.0.0.0.0");
            }

            item.Slot = slot;
            item.InventoryType = InventoryType.WEAR;
            await session.Player.Packets.GenerateScPacket();
            await CharacterDbHelper.UpdateAsync(item);
        }

        private static bool CanUseItem(ClassId prof, byte requiredClass, bool isThreeClass = false, bool isMultiClass = false, bool multiClassSp = false)
        {
            return GetItemClassId(prof, isThreeClass, isMultiClass, multiClassSp) == requiredClass;
        }

        private static int GetItemClassId(ClassId playerClass, bool isThreeClass = false, bool isMultiClass = false, bool multiClassSp = false)
        {
            if (isThreeClass)
            {
                return playerClass switch
                {
                    ClassId.Swordsman or ClassId.Archer or ClassId.Mage => 14,
                    _ => -1
                };
            }
            else if (isMultiClass)
            {
                return playerClass switch
                {
                    ClassId.Swordsman or ClassId.Archer or ClassId.Mage or ClassId.MartialArtist => 30,
                    _ => -1
                };
            }
            else if (multiClassSp)
            {
                return playerClass switch
                {
                    ClassId.Swordsman or ClassId.Archer or ClassId.Mage or ClassId.MartialArtist => 31,
                    _ => -1
                };
            }
            else
            {
                return playerClass switch
                {
                    ClassId.Adventurer => 1,
                    ClassId.Swordsman => 2,
                    ClassId.Archer => 4,
                    ClassId.Mage => 8,
                    _ => -1
                };
            }
        }
    }
}