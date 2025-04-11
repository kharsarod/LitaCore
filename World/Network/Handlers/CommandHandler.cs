using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
