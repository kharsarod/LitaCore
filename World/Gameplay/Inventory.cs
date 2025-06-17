using Database.Player;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enum.Main.ChatEnum;
using World.Network;
using World.Entities;
using Enum.Main.ItemEnum;
using GameWorld;
using Enum.Main.MessageEnum;
using Database.Helper;
using World.Helpers;

namespace World.Gameplay
{
    public class Inventory
    {
        public int CharacterId { get; set; }
        public List<CharacterItem> Items { get; set; }
        private ClientSession _session { get; set; }
        public short MAX_INVENTORY_SLOTS { get; set; } = 48;

        public Inventory(ClientSession session, int characterId = 0)
        {
            _session = session;
            CharacterId = _session.Player != null ? _session.Player.Id : characterId;
            Items = new List<CharacterItem>();
        }

        public async Task LoadInventory()
        {
            if (Items == null)
                Items = new List<CharacterItem>();
            else
                Items.Clear();

            var items = await CharacterDbHelper.LoadInventoryAsync(CharacterId);
            Items.AddRange(items);

            // Establecer el daño a los equipos con rareza y opti

            foreach (var item in Items.Where(x => x.InventoryType == InventoryType.EQUIPMENT))
            {
                var getItem = WorldManager.GetItem(item.ItemId);
                if (getItem is null) continue;

                //  item.MinDmg = (short)(getItem.DamageMinimum * item.Rarity * item.Upgrade);
                //  item.MaxDmg = (short)(getItem.DamageMaximum * item.Rarity * item.Upgrade);
            }
        }

        public string GenerateInv(int slot, int itemId, short amount, InventoryType type)
        {
            // EQUIPMENT faltantes: rare, isColored ? Design : Upgrade, spStoneUpgrade, runeAmount
            // MAIN, ETC y Costume .0 luego lo reviso
            switch (type)
            {
                case InventoryType.EQUIPMENT:
                    return $"ivn 0 {slot}.{itemId}.0.0.0.0";

                case InventoryType.MAIN:
                    return $"ivn 1 {slot}.{itemId}.{amount}.0";

                case InventoryType.ETC:
                    return $"ivn 2 {slot}.{itemId}.{amount}.0";

                case InventoryType.MINILAND:
                    return $"ivn 3 {slot}.{itemId}.{amount}";

                case InventoryType.SPECIALIST:
                    return $"ivn 6 {slot}.{itemId}.0.0.0";

                case InventoryType.COSTUME:
                    return $"ivn 7 {slot}.{itemId}.0.0.0";
            }
            return string.Empty;
        }

        public async Task<CharacterItem> GetItemFromSlot(int slot, InventoryType type)
        {
            var item = Items.FirstOrDefault(x => x.Slot == slot && x.InventoryType == type);
            if (item is null) return null;

            return item;
        }

        public CharacterItem GetEquippedItemFromSlot(int slot)
        {
            var item = Items.FirstOrDefault(x => x.Slot == slot && x.InventoryType == InventoryType.WEAR);
            if (item is null) return null;

            return item;
        }

        public async Task AddItemToInventory(int characterId, short itemId, short amount, byte rare, byte upgrade)
        {
            try
            {
                var item = WorldManager.GetItem(itemId);
                var inventoryItem = Items.FirstOrDefault(x => x.ItemId == itemId);

                if (inventoryItem != null && inventoryItem.InventoryType != InventoryType.EQUIPMENT
                    && inventoryItem.InventoryType != InventoryType.SPECIALIST
                    && inventoryItem.InventoryType != InventoryType.COSTUME
                    && inventoryItem.InventoryType != InventoryType.WEAR)
                {
                    inventoryItem.Amount += amount;
                    await CharacterDbHelper.UpdateAsync(inventoryItem);
                }
                else
                {
                    if (item is null) return;
                    var nextSlot = GetNextFreeSlot(item.Type);

                    var newItem = new CharacterItem
                    {
                        CharacterId = characterId,
                        ItemId = item.Id,
                        Amount = (short)(item.Type == InventoryType.EQUIPMENT ? 1 : amount),
                        Slot = nextSlot,
                        Rarity = rare,
                        Upgrade = upgrade,
                        InventoryType = item.Type
                    };

                    if (item.EquipmentTypeSlot == EquipmentType.FAIRY)
                    {
                        if (item.ElementRate < 50)
                        {
                            newItem.FairyMonsterRemaining = 50;
                        }
                        else
                        {
                            newItem.FairyMonsterRemaining = 152 * item.ElementRate + 50;
                        }
                        newItem.FairyLevel = (byte)item.ElementRate;
                    }


                    if (item.EquipmentTypeSlot == EquipmentType.COSTUME_HAT || item.EquipmentTypeSlot == EquipmentType.COSTUME_SUIT && item.ItemValidTime > 0)
                    {
                        var expireDate = DateTime.Now.AddSeconds(item.ItemValidTime);
                        newItem.TimeRemaining = expireDate;
                    }

                    await CharacterDbHelper.InsertItemAsync(newItem);
                    await CharacterItemHelper.SetItemStats(newItem);
                    Items.Add(newItem);
                }

                if (_session.Player == null) return;
                await _session.Player.Packets.GenerateInvPacket();

                await _session.Player.ChatSayById(MessageId.GET_ITEM, ChatColor.Green, item.Id, amount, MessageType.ITEM);
                Log.Information("Added {Amount} {ItemId} to inventory of character {CharacterId} - {PlayerName}", amount, _session.Player.GetItemName(item.Id), characterId, _session.Player.Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding item to inventory");
            }
        }


        public async Task EquipAdventurerItems()
        {
            foreach(var item in Items.Where(x => x.ItemId == 1 && x.ItemId == 8 && x.ItemId == 12))
            {
                var getItem = WorldManager.GetItem(item.ItemId);
                item.InventoryType = InventoryType.WEAR;
                item.Slot = (int)getItem.EquipmentTypeSlot;

                await CharacterDbHelper.UpdateAsync(item);
            }
        }

        public List<CharacterItem> GetEquippedItems() => Items.Where(x => x.InventoryType == InventoryType.WEAR).ToList();

        public CharacterItem GetItemFromInventory(int itemId)
        {
            return Items.FirstOrDefault(i => i.ItemId == itemId);
        }

        public bool HasItem(int itemId) => Items.Any(i => i.ItemId == itemId);

        public async Task ClearInventory(InventoryType type)
        {
            var itemsToRemove = Items.Where(x => x.InventoryType == type).ToList();
            foreach (var item in itemsToRemove)
            {
                await DeleteItemFromInventory(item.ItemId, item.Amount, item.Slot);
            }
        }

        public async Task DeleteItemFromInventory(int itemId, short amount, int slot)
        {
            try
            {
                var inventoryItem = Items.FirstOrDefault(i => i.ItemId == itemId && i.Slot == slot);

                if (inventoryItem != null)
                {
                    Log.Information("Deleted {ItemName} from inventory of character {CharacterId} - {PlayerName}", amount, _session.Player.GetItemName((short)itemId), CharacterId, _session.Player.Name);

                    if (amount <= 0)
                    {
                        await CharacterDbHelper.RemoveAsync(inventoryItem);
                        Items.Remove(inventoryItem);
                        await _session.Player.Packets.GenerateInvPacket();
                        return;
                    }

                    if (inventoryItem.Amount > amount)
                    {
                        inventoryItem.Amount -= amount;
                        await CharacterDbHelper.UpdateAsync(inventoryItem);
                    }
                    else
                    {
                        await CharacterDbHelper.RemoveAsync(inventoryItem);
                        Items.Remove(inventoryItem);
                        await _session.Player.Packets.GenerateInvPacket();
                    }
                }
                else
                {
                    Log.Error($"Item {itemId} not found in inventory.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error deleting item from inventory");
            }
        }

        public async Task DeleteEquippedItem(int itemId, int slot)
        {
            try
            {
                var inventoryItem = Items.FirstOrDefault(i => i.ItemId == itemId && i.Slot == slot && i.InventoryType == InventoryType.WEAR);
                if (inventoryItem != null)
                {
                    await CharacterDbHelper.RemoveAsync(inventoryItem);
                    Items.Remove(inventoryItem);
                    await _session.Player.Packets.GenerateInvPacket();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error deleting item from inventory");
            }
        }

        public bool IsInventoryFull(InventoryType type) => Items.Count(x => x.InventoryType == type) >= MAX_INVENTORY_SLOTS;

        public async Task DeleteItemFromInventory(int itemId, short amount)
        {
            try
            {
                var inventoryItem = Items.FirstOrDefault(i => i.ItemId == itemId);

                if (inventoryItem != null)
                {
                    Log.Information("Deleted {ItemName} from inventory of character {CharacterId} - {PlayerName}", amount, _session.Player.GetItemName((short)itemId), CharacterId, _session.Player.Name);

                    if (amount <= 0)
                    {
                        await CharacterDbHelper.RemoveAsync(inventoryItem);
                        Items.Remove(inventoryItem);
                        await _session.Player.Packets.GenerateInvPacket();
                        return;
                    }

                    if (inventoryItem.Amount > amount)
                    {
                        inventoryItem.Amount -= amount;
                        await CharacterDbHelper.UpdateAsync(inventoryItem);
                        await _session.Player.Packets.GenerateInvPacket();
                    }
                    else
                    {
                        await CharacterDbHelper.RemoveAsync(inventoryItem);
                        Items.Remove(inventoryItem);
                        await _session.Player.Packets.GenerateInvPacket();
                    }
                    //  await _session.Player.Packets.GenerateInvPacket();
                }
                else
                {
                    Log.Error($"Item {itemId} not found in inventory.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error deleting item from inventory");
            }
        }

        public async Task SortInventory(InventoryType type)
        {
            var playerClass = (byte)_session.Player.Class;

            if (type == InventoryType.SPECIALIST)
            {
                var itemsToSort = Items
                .Where(x => x.InventoryType == InventoryType.SPECIALIST)
                .OrderBy(x => WorldManager.GetItem(x.ItemId).RequiredClass == playerClass)
                .ThenBy(x => WorldManager.GetItem(x.ItemId).LevelJobMinimum)
                .ToList();

                foreach (var item in itemsToSort)
                    item.Slot = 255;

                byte slotIndex = 0;
                foreach (var item in itemsToSort)
                    item.Slot = slotIndex++;
            }
            if (type == InventoryType.COSTUME)
            {
                var itemsToSort = Items
                    .Where(x => x.InventoryType == InventoryType.COSTUME)
                    .ToList();

                foreach (var item in itemsToSort)
                    item.Slot = 255;

                byte slotIndex = 0;
                foreach (var item in itemsToSort)
                    item.Slot = slotIndex++;

                await CharacterDbHelper.UpdateItemsAsync(itemsToSort);
            }

            // Sort the other inventory types
            var itemsSort = Items
                .Where(x => x.InventoryType == type)
                .OrderBy(x => x.Slot)
                .ToList();

            foreach (var item in itemsSort)
                item.Slot = 255;

            byte slotIndex2 = 0;
            foreach (var item in itemsSort)
                item.Slot = slotIndex2++;

            await CharacterDbHelper.UpdateItemsAsync(itemsSort);

            await _session.Player.Packets.GenerateInvPacket();
        }

        public async Task EnsureWearSlotsLoaded()
        {
            var tasks = Enumerable.Range(0, 17)
                .Select(slot => GetItemFromSlot(slot, InventoryType.WEAR))
                .ToArray();

            await Task.WhenAll(tasks);
        }

        public byte GetNextFreeSlot(InventoryType type)
        {
            var usedSlots = Items
                .Where(i => i.InventoryType == type && i.InventoryType != InventoryType.WEAR)
                .Select(x => x.Slot)
                .ToHashSet();

            for (byte i = 0; i < MAX_INVENTORY_SLOTS; i++)
            {
                if (!usedSlots.Contains(i))
                    return i;
            }

            return 0;
        }

        public async Task UpdateItems()
        {
            await CharacterDbHelper.UpdateItemsAsync(Items);
        }
    }
}