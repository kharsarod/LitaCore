using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class NRunHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("n_run", HandleNRun);
        }

        public async Task HandleNRun(ClientSession session, string[] parts)
        {
            // n_run 14 17 2 1500

            var runId = byte.Parse(parts[2]);
            var type = int.Parse(parts[3]);
            var value = int.Parse(parts[4]);
            var entityId = int.Parse(parts[5]);

            if (type is 17) // shop supongo
            {
            }
        }
    }
}