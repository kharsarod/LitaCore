using Enum.Main.PortalEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.World
{
    public class Portal
    {
        public int Id { get; set; }
        public short FromMapId { get; set; }
        public short FromMapX { get; set; }
        public short FromMapY { get; set; }
        public short ToMapId { get; set; }
        public short ToMapX { get; set; }
        public short ToMapY { get; set; }
        public PortalType Type { get; set; }
        public bool Disabled { get; set; }
    }
}