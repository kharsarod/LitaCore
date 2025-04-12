using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTalePacketsLib
{
    public abstract class BasePacket
    {
        public abstract string Header { get; }
        public abstract void Deserialize(string[] parts);
    }
}
