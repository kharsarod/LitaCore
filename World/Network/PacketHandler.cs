using Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network
{
    public class PacketHandler : IPacketHandler
    {
        private readonly Dictionary<string, Func<ClientSession, string[], Task>> _handlers = new();
        private readonly Dictionary<string, Func<ClientSession, string[], Task>> _prefixHandlers = new();
        private readonly List<string> _commands = new();

        public void Register(string packetName, Func<ClientSession, string[], Task> handler)
        {
            _handlers[packetName] = handler;

            if (packetName.StartsWith("$"))
            {
                _commands.Add(packetName);
            }
        }

        public void RegisterPrefix(string prefix, Func<ClientSession, string[], Task> handler)
        {
            _prefixHandlers[prefix] = handler;
        }

        public IEnumerable<string> GetRegisteredCommands()
        {
            return _handlers.Keys.Where(k => k.StartsWith("$"));
        }

        public async Task Handle(ClientSession session, string data)
        {
            string[] parts = data.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string packetName = parts[1];

            if (_handlers.TryGetValue(packetName, out var handler))
            {
                await handler(session, parts);
            }
            else
            {
                foreach (var (prefix, _handler) in _prefixHandlers)
                {
                    if (packetName.StartsWith(prefix))
                    {
                        await _handler(session, parts);
                        return;
                    }
                }
                
                if (ConfigManager.WorldServerConfig.Features.AllowPacketLogging)
                {
                    Log.Warning("Unknown packet: {PacketName}", packetName);
                }
            }
        }

        public IReadOnlyList<string> GetCommands() => _commands.AsReadOnly();
    }
}