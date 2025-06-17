using Database.Player;
using Enum.Main.ChatEnum;
using Enum.Main.ItemEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class EqInfoHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("eqinfo", HandleEqInfo);
        }

        public static async Task HandleEqInfo(ClientSession session, string[] parts)
        {
            var invType = (InventoryType)byte.Parse(parts[2]);
            var fromSlot = int.Parse(parts[3]);
            var inventory = session.Player.Inventory;
            var packet = new StringBuilder();
            packet.Append("e_info");

            CharacterItem charItem = null;

            if ((byte)invType == 5) // eqinfo 5 (exchanges)
            {
                var charId = int.Parse(parts[5]);
                var getSession = WorldManager.GetPlayerById(charId);
                var inv = getSession.Player.Inventory;

                var getItemFromExchangeList = session.Player.OtherSessionExchange.Player.ExchangeItems.FirstOrDefault(x => x.Key.Slot == fromSlot);

                var getItem = session.Player.OtherSessionExchange.Player.ExchangeItems.ElementAtOrDefault(fromSlot);

                charItem = await WorldManager.GetCharacterItem(getItem.Key.Slot, session.Player.OtherSessionExchange.Player.Id);
            }

            if (invType == InventoryType.EQUIPMENT)
            {
                charItem = inventory.GetEquippedItemFromSlot(fromSlot);
            }
            else if (invType == InventoryType.MAIN)
            {
                invType = InventoryType.EQUIPMENT;
                charItem = await inventory.GetItemFromSlot(fromSlot, invType);
            }

            if (charItem is null)
            {
                return;
            }

            var item = WorldManager.GetItem(charItem.ItemId);
            var isFixed = charItem.IsFixed ? 1 : 0;
            switch (item.EquipmentTypeSlot)
            {
                case EquipmentType.MAIN_WEAPON or EquipmentType.SECONDARY_WEAPON:

                    packet.Append($" {(item.RequiredClass is 8 ? 5 : 0)} {(charItem.ItemId != 1 ? $"-{charItem.ItemId}" : $"{charItem.ItemId}")} {charItem.Rarity} {charItem.Upgrade} {isFixed} {item.LevelMinimum}" +
                        $" {item.DamageMinimum + charItem.MinDmg} {item.DamageMaximum + charItem.MaxDmg} {item.HitRate + charItem.HitRate}" +
                        $" {item.CriticalLuckRate + charItem.CritLuckRate} {item.CriticalRate + charItem.CritRate} {charItem.Ammo}" +
                        $" {item.MaximumAmmo} {item.Price} -1 {charItem.HoldingModel} 0 0 0"); // El holdingModel podría ir -1 si no es isPartnerEq.
                    break;

                case EquipmentType.ARMOR:
                    packet.Append($" 2 {charItem.ItemId} {charItem.Rarity} {charItem.Upgrade} {isFixed} {item.LevelMinimum}" +
                        $" {item.CloseDefence + charItem.CloseDefence} {item.DistanceDefence + charItem.DistDefence} {item.MagicDefence + charItem.MagicDefence}" +
                        $" {item.DefenceDodge + charItem.DefDodge} {item.Price} -1 0 0 0 0");
                    break;

                case EquipmentType.FAIRY:
                    packet.Append($" 4 {item.Id} 2 {item.ElementRate + charItem.FairyLevel} 0 3 280 0 2 {charItem.FairyMonsterRemaining} 0 0 0");
                    break;

                case EquipmentType.COSTUME_HAT or EquipmentType.COSTUME_SUIT:
                    var expirationDate = session.Player.GetItemExpirationTime(item.Id);
                    packet.Append($" 3 {item.Id} {item.LevelMinimum} {item.CloseDefence} {item.DistanceDefence} {item.MagicDefence} {item.DefenceDodge} 0 0 0 0 0 0 0 {expirationDate} 0"); // 1 = equippable, 0 = skin i think.
                                                                                                                                                                                            // e_info 3 8121 20 0 0 0 0 0 0 0 0 2400 0 0 407 0
                    break;

                case EquipmentType.SPECIALIST:
                    packet.Clear();
                    var xp = -0;
                    var splvl = 99;

                    StringBuilder skills = new StringBuilder();

                    foreach (var skill in WorldManager.Skills.Where(x => x.UpgradeType == item.Model && x.SkillType == 1 && x.CastId > 0).OrderBy(x => x.Level).ToList())
                    {
                        skills.Append($"{skill.SkillVNum}.");
                    }

                    var transportId = 1;
                    var freePoint = 100;
                    var slHit = 0;
                    var slDefence = 0;
                    var slElement = 0;
                    var slHp = 0;
                    var destroyed = 0;
                    var spPerfUpgrade = 0;
                    var wingId = WorldManager.Items.FirstOrDefault(x => x.ItemType == ItemType.SPECIAL && x.Effect == 650 && x.EffectData == charItem.Rarity)?.Id ?? 0;

                    await session.Player.ChatSay($"Speed: {item.Speed}, VNum: {item.Id}", ChatColor.Green);

                    packet.Append($"slinfo 0 {item.Id} {item.Model} {splvl} {item.LevelJobMinimum} {item.ReputationMinimum} 0 {item.Speed} 0 0 0 0 0 {1}" +
                        $" {item.FireResistance} {item.WaterResistance} {item.LightResistance} {item.DarkResistance} {xp} {5000} {skills.ToString()} {freePoint} {slHit} {slDefence} {slElement} {slHp} " +
                        $"{charItem.Upgrade} 0 0 {(charItem.IsFixed ? 1 : 0)} 0 0 0 0 {spPerfUpgrade} 0 0 0 0 0 0 0 0 0 0 {wingId} 0 0");
                    break;
            }

            await session.SendPacket(packet.ToString());
            //  await session.Player.ChatSay($"{packet.ToString()}", ChatColor.Green);
        }
    }
}