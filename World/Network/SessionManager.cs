using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network
{
    public static class SessionManager
    {
        private static readonly Dictionary<string, ClientSession> _sessions = new();



        public static bool IsConnected(string username)
        {
            return _sessions.ContainsKey(username);
        }

        public static int GetClientId(ClientSession session)
        {
            return session.ClientId;
        }

        public static void Register(string username, ClientSession session)
        {
            _sessions[username] = session;
        }

        public static void Unregister(string username)
        {
            if (_sessions.ContainsKey(username))
                _sessions.Remove(username);
        }

        public static ClientSession? GetSession(string username)
        {
            return _sessions.TryGetValue(username, out var session) ? session : null;
        }
    }
}
