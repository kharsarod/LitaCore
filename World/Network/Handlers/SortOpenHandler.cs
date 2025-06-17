using Enum.Main.ItemEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class SortOpenHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("sortopen", HandleOpenSort);
        }

        public static async Task HandleOpenSort(ClientSession session, string[] parts)
        {
            await session.Player.Inventory.SortInventory(InventoryType.SPECIALIST);
            await session.Player.Inventory.SortInventory(InventoryType.COSTUME);
        }
    }
}