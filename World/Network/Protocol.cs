using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network
{
    public static class Protocol
    {
        public static string FormatPacket(string packetName, params object[] args)
        {
            return $"{packetName} {string.Join(" ", args)}";
        }
    }
}
