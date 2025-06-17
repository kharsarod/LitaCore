using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class RemoveItemHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("b_i", HandleRemove);
            handler.RegisterPrefix("#b_i", HandleRemoveConfirm);
        }

        public static async Task HandleRemove(ClientSession session, string[] parts)
        {
            // b_i {inventory_type} {from_slot}
            var invType = (InventoryType)byte.Parse(parts[2]);
            var fromSlot = int.Parse(parts[3]);

            // 1 luego del fromSlot ? Hay que investigar. Los dos 0 en el segundo campo no sé que son ni tampoco el 5, los dos últimos 0 tampoco sé.

            //                    dlgi #b_i^0^1^1 #b_i^0^0^5 1173 0 0

            string dlgiPacket = $"dlgi #b_i^{(byte)invType}^{fromSlot}^1 #b_i^0^0^5 {(short)MessageId.DELETE_ITEM} 0 0";

            await session.SendPacket(dlgiPacket);
        }

        public static async Task HandleRemoveConfirm(ClientSession session, string[] parts)
        {
            var args = parts[1].Split('^');
            var invType = (InventoryType)byte.Parse(args[1]);
            var fromSlot = int.Parse(args[2]);

            if (args[3] == "2")
            {
                var getItem = await session.Player.Inventory.GetItemFromSlot(fromSlot, invType);

                if (getItem is null) return;

                await session.Player.Inventory.DeleteItemFromInventory(getItem.ItemId, 0, fromSlot);
                return;
            }
            else if (args[3] == "5")
            {
                return;
            }

            string dlgiPacket = $"dlgi #b_i^{(byte)invType}^{fromSlot}^2 #b_i^0^0^5 {(short)MessageId.DELETE_ITEM_CONFIRM} 0 0";

            await session.SendPacket(dlgiPacket);
        }
    }
}