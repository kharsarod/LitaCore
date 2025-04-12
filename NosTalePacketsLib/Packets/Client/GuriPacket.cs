using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTalePacketsLib.Packets.Client
{
    [Packet("guri")]
    public class GuriPacket : BasePacket
    {
        public byte Type { get; set; }
        public byte Field1 { get; set; }
        public byte Field2 { get; set; }
        public byte Value { get; set; }

        public override string Header => "guri";

        public override void Deserialize(string[] parts)
        {
            if (parts.Length > 1)
            {
                Type = byte.Parse(parts[1]);
                Field1 = byte.Parse(parts[2]);
                Field2 = byte.Parse(parts[3]);
                Value = byte.Parse(parts[4]);
            }
        }
    }
}
