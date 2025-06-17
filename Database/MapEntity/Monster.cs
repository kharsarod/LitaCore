using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.MapEntity
{
    public class Monster
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }
        public short VNum { get; set; }
        public string? Name { get; set; }
        public short MapId { get; set; }
        public short MapX { get; set; }
        public short MapY { get; set; }
        public bool IsMoving { get; set; }
        public byte Position { get; set; }
    }
}