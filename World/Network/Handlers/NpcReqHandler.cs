using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class NpcReqHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("npc_req", HandleNpcReq);
        }

        public static async Task HandleNpcReq(ClientSession session, string[] parts)
        {
            var type = int.Parse(parts[2]);
            var entityId = int.Parse(parts[3]);

            var npcEntity = session.Player.CurrentMap.NpcEntities.FirstOrDefault(x => x.NpcId == entityId);
            if (npcEntity is null) return;

            await session.Player.SendPacket($"npc_req {type} {entityId} {npcEntity.DialogId}");
        }
    }
}