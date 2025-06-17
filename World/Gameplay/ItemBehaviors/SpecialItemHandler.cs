using Database.Item;
using Database.Player;
using Enum.Main.ChatEnum;
using Enum.Main.MessageEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Gameplay.ItemBehaviors.Interfaces;
using World.Network;
using static System.Collections.Specialized.BitVector32;

namespace World.Gameplay.ItemBehaviors
{
    public class SpecialItemHandler : IItemHandler
    {
        public async Task HandleUse(ClientSession session, Item item)
        {
            if (item is null) return;

            var charItem = session.Player.Inventory.GetItemFromInventory(item.Id);
            if (charItem is null) return;

            if (item.Effect is 1000)
            {
                var timeToMount = TimeSpan.FromSeconds(3).TotalMilliseconds;

                if (session.Player.IsUsingMount)
                {
                    await session.Player.TransformMountToNormal();
                    if (session.Player.BaseSpeed.TryGetValue(session.Player.Class, out var speed))
                    {
                        await session.Player.SetSpeed(speed);
                    }
                    return;
                }

                await session.SendPacket($"delay {timeToMount} 3 #u_i^{(byte)charItem.InventoryType}^{session.Player.Id}^{charItem.Slot}^2^2");
                await session.Player.CurrentMap.Broadcast($"guri 2 1 {session.Player.Id} 0");
            }

            if (item.Effect is 650)
            {
                var sp = await session.Player.GetEquippedSpecialist();

                if (!session.Player.UsingSpecialist)
                {
                    await session.Player.ChatSayById(MessageId.SPECIALIST_TRANSFORM_WINGS_REQUIRED, ChatColor.Yellow);
                    return;
                }

                if (sp.Upgrade <= 0)
                {
                    await session.Player.ChatSayById(MessageId.SPECIALIST_UPGRADE_BELOW_LEVEL, ChatColor.Yellow);
                    return;
                }

                await session.Player.Packets.SendQNaiAsk(770, 2, item.Id, MessageId.SPECIALIST_CHANGE_WINGS_ASK);
            }

            switch (item.Effect)
            {
                case 150:
                    var addition = item.EffectData;
                    int maxAdditionPoints = 10000000; // 10KK.
                    if (session.Player.SpecialistAddPts >= maxAdditionPoints) return;

                    await session.Player.AddSpecialistAdditionPoints(addition, 0);
                    await session.SendPacket($"msgi 0 {(short)1279} 0 0 0 0 0 0");
                    await session.Player.Inventory.DeleteItemFromInventory(item.Id, 1);
                    break;
            }

            switch (item.Id)
            {
                case 9573:
                    await session.Player.Inventory.AddItemToInventory(session.Player.Id, 1011, 50, 0, 0);
                    await session.Player.Inventory.AddItemToInventory(session.Player.Id, 1012, 80, 0, 0);
                    await session.Player.Inventory.AddItemToInventory(session.Player.Id, 9041, 3, 0, 0);
                    await session.Player.Inventory.AddItemToInventory(session.Player.Id, 1907, 1, 0, 0);
                    await session.Player.Inventory.DeleteItemFromInventory(item.Id, 1);
                    break;
            }
        }
    }
}