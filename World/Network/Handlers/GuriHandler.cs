using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network.Handlers
{
    public class GuriHandler
    {
        public static async Task HandleGuri(ClientSession session, string[] parts) // Guri
        {
            if (int.Parse(parts[5]) >= 973 && int.Parse(parts[5]) <= 999)
            {
                await session.SendPacket($"eff 1 1 {int.Parse(parts[5]) + 4099}");
                await session.Player.CurrentMap.Broadcast($"eff 1 {session.Player.Character.Id} {int.Parse(parts[5]) + 4099}");
            }
        }
    }
}
