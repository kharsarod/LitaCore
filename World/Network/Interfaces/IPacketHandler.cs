using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network.Interfaces
{
    public interface IPacketHandler
    {
        void Register(string packetId, Func<ClientSession, string[], Task> handler);

        void RegisterPrefix(string packetId, Func<ClientSession, string[], Task> handler);

        Task Handle(ClientSession session, string packet);

        IEnumerable<string> GetRegisteredCommands();
    }
}