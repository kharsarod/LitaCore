using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class BtkHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("btk", HandleBtk);
        }

        public static async Task HandleBtk(ClientSession session, string[] parts)
        {
            var playerId = int.Parse(parts[2]);
            var playerName = parts[3];
            var message = string.Join(' ', parts.Skip(4));
            var playerDest = WorldManager.GetPlayerById(playerId);
            if (playerDest is null) return;

            await playerDest.SendPacket($"talk {session.Player.Id} {message}");
        }
    }
}