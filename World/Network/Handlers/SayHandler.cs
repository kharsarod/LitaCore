using Enum.Main.ChatEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class SayHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("say", HandleSay);
        }

        public async Task HandleSay(ClientSession session, string[] parts)
        {
            var message = string.Join(' ', parts.Skip(2));

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.Length > 60)
            {
                message = message.Substring(0, 60);
            }

            await session.Player.ChatSay(message, ChatColor.White, false, true);
            await session.Player.CurrentMap.Broadcast($"say 1 {session.Player.Id} 1 {message}", session);
        }
    }
}