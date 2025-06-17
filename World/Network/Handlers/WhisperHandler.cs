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
    public class WhisperHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.RegisterPrefix("/", HandleWhisper);
        }

        public static async Task HandleWhisper(ClientSession session, string[] parts)
        {
            var playerName = parts[1].Substring(1);
            var msg = string.Join(' ', parts.Skip(2));

            await session.SendPacket($"spk 1 {session.Player.Id} 5 {session.Player.Name} {msg}");

            var getPlayer = WorldManager.GetPlayerByName(playerName);
            if (getPlayer == null)
            {
                await session.Player.SendMsgi(MessageId.PLAYER_WHISP_NON_CONNECTED, 0, 7, 0, 0, playerName);
                return;
            }
            if (getPlayer.ChannelId != session.ChannelId)
            {
                await getPlayer.SendPacket($"spk 1 -1 5 {session.Player.Name} {msg} <Channel: {session.ChannelId}>");
                return;
            }
            await getPlayer.SendPacket($"spk 1 {session.Player.Id} 5 {session.Player.Name} {msg}");
        }
    }
}