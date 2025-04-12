using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTalePacketsLib.Packets.Client
{
    [Packet("NsTeST")]
    public class LoginPacket : BasePacket
    {
        public override string Header => "NsTeST";

        public override void Deserialize(string[] parts) { }
    }
}
