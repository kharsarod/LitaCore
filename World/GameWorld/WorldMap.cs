using Database.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.GameWorld
{
    public class WorldMap : Map
    {
        public Dictionary<short, WorldMap> Maps { get; set; }
    }
}
