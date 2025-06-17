using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.World
{
    public class DropData
    {
        public int Id { get; set; }

        public short DropId { get; set; }

        public short VNum { get; set; }

        public int Amount { get; set; }

        public int Chance { get; set; }

        public short? MonsterVNum { get; set; }
    }
}