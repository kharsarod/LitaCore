using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class InventorySortHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("isort", HandleISortQuestion);
            handler.RegisterPrefix("#isort", HandleISortConfirm);
        }

        public static async Task HandleISortQuestion(ClientSession session, string[] parts)
        {
            var invType = (InventoryType)byte.Parse(parts[2]);
            if (session.Player.LastSortedInventory.AddSeconds(5) > DateTime.Now)
            {
                await session.SendPacket($"msgi 3 {(short)MessageId.INVENTORY_SORT_COOLDOWN} 0 0 0 0 0"); // 0 0 0 0 0 ? no sé que son.
                return;
            }
            string packet = $"qnai #isort^{(byte)invType}^1 {(short)MessageId.SORT_INVENTORY_QUESTION} 0 0 0"; // 0 0 0 ? no sé que son.
            await session.SendPacket(packet);
        }

        public static async Task HandleISortConfirm(ClientSession session, string[] parts)
        {
            var splitter = parts[1].Split('^');
            var invType = (InventoryType)byte.Parse(splitter[1]);

            if (splitter[2] == "1")
            {
                await session.Player.Inventory.SortInventory(invType);
                session.Player.LastSortedInventory = DateTime.Now;
            }
        }
    }
}