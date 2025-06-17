using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;

namespace World.Network
{
    public static class SessionManager
    {
        private static readonly ConcurrentDictionary<string, ClientSession> _sessions = new();

        public static bool IsConnected(string username)
        {
            return _sessions.ContainsKey(username);
        }

        public static int GetClientId(ClientSession session)
        {
            return session.ClientId;
        }

        public static ClientSession GetSessionByPlayer(Player player)
        {
            return _sessions.FirstOrDefault(x => x.Value.Player == player).Value;
        }

        public static void Register(string username, ClientSession session)
        {
            _sessions[username] = session;
        }

        public static void Unregister(string username)
        {
            _sessions.TryRemove(username, out _);
        }

        public static ClientSession? GetSession(string username)
        {
            return _sessions.TryGetValue(username, out var session) ? session : null;
        }

        public static List<ClientSession> GetOnlineSessionsAsync()
        {
            return _sessions.Values.ToList();
        }
    }
}