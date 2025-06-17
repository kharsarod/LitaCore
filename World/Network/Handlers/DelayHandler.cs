using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class DelayHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("delay", HandleDelay);
        }

        public static async Task HandleDelay(ClientSession session, string[] parts)
        {
            var delay = int.Parse(parts[2]);
            var value = int.Parse(parts[3]);
            var packet = parts[4];
            byte progress = 0;

            Observable.Interval(TimeSpan.FromMilliseconds(delay)).Subscribe(async _ =>
            {
                await session.SendPacket(packet);
            });
        }
    }
}