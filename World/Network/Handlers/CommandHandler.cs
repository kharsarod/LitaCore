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
            await session.Player.ChangeMap(short.Parse(parts[2]), short.Parse(parts[3]), short.Parse(parts[4]));
        }

        public static async Task Speed(ClientSession session, string[] parts)
        {
            await session.Player.SetSpeed(byte.Parse(parts[2]));

            await session.Player.ChatSay("Has cambiado tu velocidad de movimiento.", ChatColor.Green);
        }

        public static async Task ModLevel(ClientSession session, string[] parts)
        {
            var level = byte.Parse(parts[2]);
            await session.Player.SetLevel(level);
            await AppDbContext.UpdateAsync(session.Player.Character);
        }

        public static async Task ModJobLevel(ClientSession session, string[] parts)
        {
            var level = byte.Parse(parts[2]);
            await session.Player.SetJobLevel(level);
            await AppDbContext.UpdateAsync(session.Player.Character);
        }

        public static async Task PlayerInfo(ClientSession session, string[] parts)
        {
            await session.Player.ChatSay($"Username: {session.Account.Username}, rank: {session.Account.Rank}, characterId: {session.Player.Character.Id}", ChatColor.Green);
        }

        public static async Task GetPosition(ClientSession session, string[] parts)
        {
            await session.Player.ChatSay($"id: {session.Player.Character.MapId}, x: {session.Player.Character.MapPosX}, y: {session.Player.Character.MapPosY}.", ChatColor.Green);
        }

        public static async Task GetMapInfo(ClientSession session, string[] parts)
        {
            await session.Player.ChatSay($"name: {session.Player.CurrentMap.Name}, id: {session.Player.CurrentMap.Id}," +
                $" expRate: x{session.Player.CurrentMap.ExpRate}, goldRate: x{session.Player.CurrentMap.GoldRate}, dropRate: x{session.Player.CurrentMap.DropRate}," +
                $" pvpAllowed: {session.Player.CurrentMap.IsPvpAllowed}, shopAllowed: {session.Player.CurrentMap.IsShopAllowed}.", ChatColor.Green);
        }
    }
}
