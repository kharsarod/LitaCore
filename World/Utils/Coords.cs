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

        public static bool operator ==(Coords a, Coords b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.MapPosX == b.MapPosX && a.MapPosY == b.MapPosY;
        }

        public static bool operator !=(Coords a, Coords b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj is not Coords other)
                return false;

            return this == other;
        }

        public bool Equals(Coords other) => MapPosX == other.MapPosX && MapPosY == other.MapPosY;

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