using Enum.Main.ChatEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class PortalHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("preq", HandlePortal);
        }

        public static async Task HandlePortal(ClientSession session, string[] parts)
        {
            var portalCd = TimeSpan.FromSeconds(5);
            if (session.Player.LastMoveFromPortals.AddSeconds(portalCd.TotalSeconds) > DateTime.Now && session.Account.Rank < 3)
            {
                await session.Player.ChatSayById(MessageId.PORTAL_MOVE_COOLDOWN, ChatColor.Yellow);
                return;
            }

            var pX = session.Player.MapPosX;
            var pY = session.Player.MapPosY;

            var portal = WorldManager.Portals.FirstOrDefault(x => pY >= x.FromMapY - 1 && pY <= x.FromMapY + 1 && pX >= x.FromMapX - 1 && pX <= x.FromMapX + 1);

            if (portal is null)
            {
                if (session.Account.Rank >= 3)
                {
                    await session.Player.ChatSay("No se encuentra el portal.", ChatColor.Red);
                }
                return;
            }

            var map = await WorldManager.GetInstance(portal.ToMapId, session.ChannelId);

            await session.Player.ChangeMap(map, portal.ToMapX, portal.ToMapY);

            session.Player.LastMoveFromPortals = DateTime.Now;
        }
    }
}