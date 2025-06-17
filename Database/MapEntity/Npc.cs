using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.MapEntity
{
    public class Npc
    {
        public int Id { get; set; }
        public int NpcId { get; set; }
        public string? Name { get; set; }
        public short VNum { get; set; }
        public short MapId { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public byte Dir { get; set; }
        public bool IsMoving { get; set; }
        public int DialogId { get; set; }
    }
}