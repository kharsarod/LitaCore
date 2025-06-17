using Enum.Main.ChatEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace World.Network.Handlers
{
    public class GuriHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("guri", HandleGuri);
            handler.RegisterPrefix("#guri", HandleGuriAction);
        }

        public static async Task HandleGuri(ClientSession session, string[] parts) // Guri
        {
            string rawPacket = string.Join(' ', parts);

            var type = rawPacket.Split(' ')[2];

            if (type == "10")
            {
                var data = int.Parse(rawPacket.Split(' ')[5]);
                if (data >= 973 && data <= 999)
                {
                    await session.Player.CurrentMap.Broadcast($"eff 1 {session.Player.Id} {int.Parse(parts[5]) + 4099}");
                }
            }
        }

        public static async Task HandleGuriAction(ClientSession session, string[] parts) // GuriAction
        {
            string rawPacket = string.Join(' ', parts);
            var data1 = short.Parse(rawPacket.Split('^')[1]);
            var data2 = short.Parse(rawPacket.Split('^')[2]);
            var value = short.Parse(rawPacket.Split('^')[3]);

            if (data1 is 770 && data2 is 2) // 770 y 2, esto es por las alas según.
            {
                var item = WorldManager.GetItem(value);
                if (item is null) return;
                if (!session.Player.Inventory.HasItem(item.Id)) return;

                await session.Player.SetSpecialistWings((short)item.EffectData);
                await session.Player.ChatSayById(MessageId.SPECIALIST_CHANGED_WINGS, ChatColor.Yellow);

                await session.Player.Inventory.DeleteItemFromInventory(item.Id, 1);
            }
        }
    }
}