using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network
{
    public class PacketHandler
    {
        private readonly Dictionary<string, Func<ClientSession, string[], Task>> _handlers = new();

        public void Register(string packetName, Func<ClientSession, string[], Task> handler)
        {
            _handlers[packetName] = handler;
        }

        public async Task Handle(ClientSession session, string data)
        {
            string[] parts = data.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string packetName = parts[1];


            if (_handlers.TryGetValue(packetName, out var handler))
            {
                await handler(session, parts);
               // Log.Information("Handled packet: " + packetName);
            }
            else
            {
                Log.Warning("Unknown packet: " + packetName);
            }
        }
    }
}
