using Communication.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public static class CommunicationService
    {
        private static List<WorldServer> _worldServers = new List<WorldServer>();

        public static void AddWorldServer(WorldServer worldServer)
        {
            _worldServers.Add(worldServer);
        }

        public static int GetWorldServersCount()
        {
            return _worldServers.Count;
        }
    }
}