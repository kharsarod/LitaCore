using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Utils
{
    public class Coords
    {
        public short MapPosX { get; set; }
        public short MapPosY { get; set; }

        public Coords(short x, short y)
        {
            MapPosX = x;
            MapPosY = y;
        }

        public static bool operator ==(Coords a, Coords b) => a.MapPosX.Equals(b);
        public static bool operator !=(Coords a, Coords b) => !a.MapPosX.Equals(b);
        public bool Equals(Coords other) => MapPosX == other.MapPosX && MapPosY == other.MapPosY;
        public override bool Equals(object obj) => (obj is Coords other) && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int _hash = 17;
                _hash = _hash * 23 + MapPosX.GetHashCode();
                _hash = _hash * 23 + MapPosY.GetHashCode();
                return _hash;
            }
        }
    }
}
