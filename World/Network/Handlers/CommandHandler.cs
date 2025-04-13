using Enum.Main.ChatEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Services;

namespace World.Network.Handlers
{
    public class CommandHandler
    {
        public static async Task Morph(ClientSession session, string[] parts)
        {
            string content = string.Join(' ', parts.Skip(2));
            await session.SendPacket($"c_mode 1 1 {content}");
            await session.SendPacket("eff 1 1 196");
        }

        public static async Task Teleport(ClientSession session, string[] parts)
        {
            session.Player.Character.MapId = short.Parse(parts[2]);
            session.Player.Character.MapPosX = short.Parse(parts[3]);
            session.Player.Character.MapPosY = short.Parse(parts[4]);

            await session.SendPacket($"at 1 {session.Player.Character.MapId} {session.Player.Character.MapPosX} {session.Player.Character.MapPosY} 0 0 1 2 -1");
            await AppDbContext.UpdateAsync(session.Player.Character);
        }

        public static async Task Speed(ClientSession session, string[] parts)
        {
            await session.Player.SetSpeed(byte.Parse(parts[2]));

            await session.Player.ChatSay("Has cambiado tu velocidad de movimiento.", ChatColor.Green);
        }
    }
}
