using Database.Migrations.WorldDb;
using Enum.Main.ChatEnum;
using Enum.Main.EntityEnum;
using Enum.Main.ItemEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Gameplay;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class AttackHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("u_s", HandleUseSkill);
        }

        public static async Task HandleUseSkill(ClientSession session, string[] parts)
        {
            var castId = int.Parse(parts[2]);
            var userType = int.Parse(parts[3]);
            var targetId = int.Parse(parts[4]);

            await Attack.HandleHit(session, castId, targetId, (Entity)userType);
        }
    }
}