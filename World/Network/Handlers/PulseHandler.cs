using Database.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class PulseHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("pulse", HandlePulse);
        }

        public static async Task HandlePulse(ClientSession session, string[] parts)
        {
            var now = DateTime.Now;
            var interval = now - session.Player.LastPulse;

            if (interval.TotalSeconds < 50)
            {
                session.Account.IsBanned = true;
                await AuthDbHelper.UpdateAsync(session.Account);
                await session.Disconnect();
                return;
            }

            session.Player.LastPulse = now;
        }
    }
}