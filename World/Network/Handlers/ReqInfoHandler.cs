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
    public class ReqInfoHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("req_info", HandleReqInfo);
        }

        public static async Task HandleReqInfo(ClientSession session, string[] parts)
        {
            var type = byte.Parse(parts[2]);
            var entityId = int.Parse(parts[3]);
            switch (type)
            {
                case 1:
                    await GetPlayerInfo(entityId, session);
                    break;
            }
        }

        private static async Task GetPlayerInfo(int characterId, ClientSession session)
        {
            StringBuilder tc_info = new StringBuilder();
            tc_info.Append("tc_info");

            //tc_info level name fairyAttribute fairyElementRate morphId 0 -1 - 1 1 1 0 0 1 0 0 1 0 0 0 0 0 0 0 0 -1 0 0 0 0 10 0 0 0 0 0 0 0 833

            var player = WorldManager.GetPlayerById(characterId);

            var level = player.Player.Level;
            var name = player.Player.Name;
            var morph = player.Player.UsingSpecialist ? player.Player.Morph : -1;
            var classId = (int)player.Player.Class;
            var gender = (int)player.Player.Gender;
            var rep = player.Player.GetReputationIcon();
            var digIcon = player.Player.GetDignityIcon;
            var mainWeapon = player.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.MAIN_WEAPON);
            var secondWeapon = player.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.SECONDARY_WEAPON);
            var armor = player.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.ARMOR);
            var fairy = player.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.FAIRY);
            var act4Kills = player.Player.Act4Victims;
            var act4deads = player.Player.Act4DeadCount;
            var reputation = player.Player.Reputation;
            var talentWin = player.Player.TalentArenaWins;
            var talentLoss = player.Player.TalentArenaLoses;
            var talentSurrender = player.Player.TalentArenaSurrender;
            var masterPoints = 0;
            var compls = player.Player.Compliments;
            var act4Points = player.Player.Act4Points;
            var bio = string.IsNullOrEmpty(session.Player.Biography) ? "" : session.Player.Biography;

            var isWeaponPvP = 0;
            var isArmorPvP = 0;
            var isSeconaryWeaponPvP = 0;
            if (mainWeapon is not null)
            {
                isWeaponPvP = WorldManager.GetItem(mainWeapon.ItemId).ReputPrice > 1000 ? 1 : 0;
            }
            if (secondWeapon is not null)
            {
                isSeconaryWeaponPvP = WorldManager.GetItem(secondWeapon.ItemId).ReputPrice > 1000 ? 1 : 0;
            }
            if (armor is not null)
            {
                isArmorPvP = WorldManager.GetItem(armor.ItemId).ReputPrice > 1000 ? 1 : 0;
            }

            tc_info.Append($" {level} {name} {(fairy is null ? 0 : WorldManager.GetItem(fairy.ItemId).Element)} {(fairy is null ? 0 : WorldManager.GetItem(fairy.ItemId).ElementRate)} {classId} {gender} -1 - {rep} {digIcon} {(mainWeapon is null ? 0 : 1)}" +
                $" {(mainWeapon is null ? 0 : mainWeapon.Rarity)} {(mainWeapon is null ? 0 : mainWeapon.Upgrade)} {(secondWeapon is null ? 0 : 1)}" +
                $" {(secondWeapon is null ? 0 : secondWeapon.Rarity)}" +
                $" {(secondWeapon is null ? 0 : secondWeapon.Upgrade)}" +
                $" {(armor is null ? 0 : 1)} {(armor is null ? 0 : armor.Rarity)} {(armor is null ? 0 : armor.Upgrade)} {act4Kills} {act4deads} {reputation} 0 0 0 {morph}" +
                $" {talentWin} {talentLoss} {talentSurrender} 0 {masterPoints} {compls} {act4Points} {isArmorPvP} {isWeaponPvP} {isSeconaryWeaponPvP} 0 0 {bio}");

            await session.SendPacket(tc_info.ToString());
        }
    }
}