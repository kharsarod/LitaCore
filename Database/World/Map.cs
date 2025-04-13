using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.World
{
    public class Map
    {

        public short Id { get; set; }

        public string? Name { get; set; }

        public int Bgm { get; set; }

        public bool IsShopAllowed { get; set; }

        public byte[] Data { get; set; }

        public byte ExpRate { get; set; }

        public byte GoldRate { get; set; }

        public byte DropRate { get; set; }

        public bool IsPvpAllowed { get; set; }

    }
}
