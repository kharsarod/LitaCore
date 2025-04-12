using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NosTalePacketsLib.Packets.Parser
{
    public static class PacketParser
    {
        public static BasePacket Parse(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return null;

            var parts = data.Split(' ');
            var packetName = parts[0];

            var packetType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(ass => ass.GetTypes())
                .FirstOrDefault(type => type.GetCustomAttribute<PacketAttribute>()?.Header.Equals(packetName, StringComparison.OrdinalIgnoreCase) == true);

            if (packetType == null) return null;

            if (Activator.CreateInstance(packetType) is BasePacket packet)
            {
                packet.Deserialize(parts);
                return packet;
            }

            return null;
        }
    }
}
