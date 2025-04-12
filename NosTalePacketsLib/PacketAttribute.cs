using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTalePacketsLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketAttribute : Attribute
    {
        public string Header { get; }

        public PacketAttribute(string header)
        {
            Header = header;
        }
    }
}
