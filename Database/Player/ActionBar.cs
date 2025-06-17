using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Player
{
    public class ActionBar
    {
        public int Id { get; set; }

        public long CharacterId { get; set; }

        public int Morph { get; set; }

        public short Pos { get; set; }

        public short Q1 { get; set; }

        public short Q2 { get; set; }

        public short Slot { get; set; }

        public short Type { get; set; }
    }
}