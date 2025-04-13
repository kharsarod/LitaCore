using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network.Handlers
{
    public class RestHandler
    {
        public static async Task HandleRest(ClientSession session, string[] parts) // Rest
        {
            // rest 1 1 1 -> [1], [2], [3]
            if (parts[3] == "1" && !session.Player.IsSitting)
            {
                session.Player.IsSitting = true;
                await session.SendPacket("rest 1 1 1");
                return;
            }
            if (session.Player.IsSitting)
            {
                session.Player.IsSitting = false;
                await session.SendPacket("rest 1 1 0");
            }
        }
    }
}
