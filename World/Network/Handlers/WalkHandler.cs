using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network.Handlers
{
    public class WalkHandler
    {
        public static async Task HandleWalk(ClientSession session, string[] parts) // Walk
        {
            session.Player.MapPosX = short.Parse(parts[2]);
            session.Player.MapPosY = short.Parse(parts[3]);
        }
    }
}
