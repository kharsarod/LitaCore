using Database.Player;
using Enum.Main.ChatEnum;
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
    public class PickUpHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("get", HandleGet);
        }

        public static async Task HandleGet(ClientSession session, string[] parts)
        {
            var instance = session.Player.CurrentMap;
            var pickerType = int.Parse(parts[2]);
            var pickerId = int.Parse(parts[3]);
            var dropId = int.Parse(parts[4]);

            var mapItem = WorldManager.GetMapItemById((short)dropId, instance);
            if (mapItem is null) return;

            bool canPickItem = false;

            switch (pickerType)
            {
                case 1: // player
                    canPickItem = session.Player.IsInRange(mapItem.X, mapItem.Y, 8);
                    break;

                case 2: // pets, not yet implemented
                    break;
            }

            if (canPickItem)
            {
                if (mapItem.Owner is not null && session.Player.GameEntity != mapItem.Owner)
                {
                    await session.Player.ChatSayById(MessageId.CANT_PICKUP_ANOTHER_PLAYER_ITEM, ChatColor.Yellow, 0, 0);
                    return;
                }

                await session.Player.Inventory.AddItemToInventory(session.Player.Id, mapItem.ItemId, mapItem.Amount, 0, 0);

                if (pickerType == 2)
                {
                    await instance.Broadcast($"eff 2 {pickerId} 5004");
                    await session.SendPacket(session.Player.Packets.GenerateIcon(1, 1, mapItem.ItemId));
                }

                await session.SendPacket(session.Player.Packets.GenerateIcon(1, 1, mapItem.ItemId));
                instance.Items.Remove(mapItem);
                await instance.Broadcast($"out 9 {dropId}");
                await instance.Broadcast($"get {pickerType} {pickerId} {mapItem.ItemId} 0");
            }
        }
    }
}
