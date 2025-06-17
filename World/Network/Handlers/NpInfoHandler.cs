using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class NpInfoHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("npinfo", HandleNpInfo);
        }

        public static async Task HandleNpInfo(ClientSession session, string[] parts)
        {
            await session.Player.Packets.GenerateScPacket();
        }
    }
}