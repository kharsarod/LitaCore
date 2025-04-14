using Enum.Main.ChatEnum;
using Enum.Main.SpecialistEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Utils;

namespace World.Network.Handlers
{
    public class CommandHandler
    {
        public static async Task Morph(ClientSession session, string[] parts)
        {
            string content = string.Join(' ', parts.Skip(2));
            await session.SendPacket($"c_mode 1 1 {content}");
            await session.SendPacket("eff 1 1 196");
            await session.Player.CurrentMap.Broadcast($"c_mode 1 {session.Player.Character.Id} {content}", session);
            await session.Player.CurrentMap.Broadcast($"eff 1 {session.Player.Character.Id} 196", session);
            session.Player.UsingSpecialist = true;
            session.Player.Morph = int.Parse(parts[2]);
            session.Player.SpUpgrade = byte.Parse(parts[3]);
            session.Player.SpWings = (SpWings)byte.Parse(parts[4]);
        }

        public static async Task Teleport(ClientSession session, string[] parts)
        {
            var mapId = short.Parse(parts[2]);
            var instance = WorldManager.GetInstance(mapId);
            if (instance is null)
            {
                await session.Player.ChatSay("No se encontró ninguna instancia.", ChatColor.Red);
                return;
            }
            if (parts.Length < 4)
            {
                var coord = instance.GetRandomCoord();

                await session.Player.ChangeMap(instance, coord.MapPosX, coord.MapPosY);
            }
            else
            {
                var x = short.Parse(parts[3]);
                var y = short.Parse(parts[4]);
                await session.Player.ChangeMap(instance, x, y);
            }
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
            await session.Player.ChatSay($"id: {session.Player.Character.MapId}, x: {session.Player.MapPosX}, y: {session.Player.MapPosY}.", ChatColor.Green);
        }

        public static async Task GetMapInfo(ClientSession session, string[] parts)
        {
            var instance = WorldManager.GetInstance(session.Player.Character.MapId);

            await session.Player.ChatSay($"name: {instance.Template.Name}, id: {instance.Template.Id}," +
                $" expRate: x{instance.Template.ExpRate}, goldRate: x{instance.Template.GoldRate}, dropRate: x{instance.Template.DropRate}," +
                $" pvpAllowed: {instance.Template.IsPvpAllowed}, shopAllowed: {instance.Template.IsShopAllowed}, players: {instance.Players.Count()}", ChatColor.Green);
        }
    }
}
