using Enum.Main.CharacterEnum;
using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class SpecialistHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("sl", HandleSpecialist);
            handler.RegisterPrefix("#sl", HandleTransform);
        }

        public static async Task HandleSpecialist(ClientSession session, string[] parts)
        {
            var delayTime = session.Account.Rank > 0 ? TimeSpan.FromSeconds(0) : TimeSpan.FromSeconds(2.75);
            var delay = TimeSpan.FromSeconds(delayTime.Seconds);
            var cd = session.Account.Rank > 0 ? TimeSpan.FromSeconds(0) : TimeSpan.FromSeconds(10);

            var sp = session.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.SPECIALIST);

            if (session.Player.IsUsingMount)
            {
                return;
            }

            if (sp is null)
            {
                await session.SendPacket($"msgi 0 {(short)MessageId.SPECIALIST_UNEQUIP} 0 0 0 0 0");
                return;
            }

            if (session.Player.LastUsedSpecialist.AddSeconds(cd.Seconds) > DateTime.Now)
            {
                var remaining = session.Player.LastUsedSpecialist.AddSeconds(10) - DateTime.Now;
                await session.SendPacket($"msgi 0 {(short)MessageId.SPECIALIST_COOLDOWN} 4 {remaining.Seconds} 0 0 0");
                return;
            }

            if (session.Player.UsingSpecialist && session.Player.Morph > 0)
            {
                await session.Player.TransformToNormal();
                return;
            }
            await session.SendPacket($"delay {delay.TotalMilliseconds} 3 #sl^1");

            await session.Player.CurrentMap.Broadcast($"guri 2 1 {session.Player.Id} 0");
        }

        public static async Task HandleTransform(ClientSession session, string[] parts)
        {
            var sp = session.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.SPECIALIST);
            var fairy = session.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.FAIRY);
            if (sp is null)
            {
                await session.SendPacket($"msgi 0 {(short)MessageId.SPECIALIST_UNEQUIP} 0 0 0 0 0");
                return;
            }

            var item = WorldManager.GetItem(sp.ItemId);
            if (item is null)
            {
                Log.Warning("Item not found in database: {ItemId}", sp.ItemId);
            }

            if (fairy is not null && item.Element != 0)
            {
                var fairyItem = WorldManager.GetItem(fairy.ItemId);
                if (fairyItem?.Element != item.Element)
                {
                    await session.Player.SendMsgi(MessageId.SPECIALIST_DIFF_FAIRY);
                    return;
                }
            }

            if (session.Player.GetReputationIcon() < item.ReputationMinimum)
            {
                await session.Player.SendMsgi(MessageId.SPECIALIST_LOW_FAME);
                return;
            }

            if (session.Player.JobLevel < item.LevelJobMinimum)
            {
                await session.Player.SendMsgi(MessageId.SPECIALIST_LOW_JOB_LEVEL);
                return;
            }

            if (session.Player.Class == ClassId.Adventurer && item.RequiredClass != 31)
            {
                await session.Player.SendMsgi(MessageId.ADVENTURER_CANT_TRANSFORM_ON_SPECIALISTS);
                return;
            }

            await session.Player.TransformToSpecialist(item.Model);
            await session.Player.CurrentMap.Broadcast($"guri 6 1 {session.Player.Id} 0"); // el characterId antes del 0 verificarlo por si algún día da bug.
        }
    }
}