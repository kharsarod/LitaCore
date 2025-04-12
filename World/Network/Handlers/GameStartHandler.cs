using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network.Handlers
{
    public class GameStartHandler
    {
        public static async Task HandleGameStart(ClientSession session, string[] parts) // GameStart
        {
            await session.SendPacket("say 1 1 12 Bienvenido!");
        }
    }
}
