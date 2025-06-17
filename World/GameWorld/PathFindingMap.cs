using Enum.Main.MapEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Utils;

namespace World.GameWorld
{
    public static class PathFindingMap
    {
        public static bool IsWalkable(int x, int y, byte[] mapGrid, int width = -1, int height = -1) // Si da algún error, puede ser por el -1 en los parámetros, que no se me olvide.
        {
            if (x < 0 || y < 0)
            {
                return false;
            }

            if (x >= width || y >= height)
            {
                return false;
            }

            byte cell = mapGrid[y * width + x];

            return (cell & (byte)CellEnum.WalkDisabled) == 0;
        }


        public static bool IsBlockedZone(int x, int y, byte[] mapGrid)
        {
            try
            {
                if (mapGrid is null) return false;

                return IsWalkable(x, y, mapGrid);
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static bool IsBlockedZone(int x, int y, byte[] mapGrid, int mapX, int mapY)
        {
            try
            {
                if (mapGrid is null) return false;
                if (x < 0 || y < 0 || x >= mapX || y >= mapY)
                {
                    return true;
                }
                return IsWalkable(x, y, mapGrid, mapX, mapY);
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static Coords GetNextStep(Coords start, Coords end, double steps)
        {
            double newX = start.MapPosX;
            double newY = start.MapPosY;

            if (start.MapPosX < end.MapPosX)
            {
                newX += steps;
                if (newX > end.MapPosX)
                    newX = end.MapPosX;
            }
            else if (start.MapPosX > end.MapPosX)
            {
                newX -= steps;
                if (newX < end.MapPosX)
                    newX = end.MapPosX;
            }

            if (start.MapPosY < end.MapPosY)
            {
                newY += steps;
                if (newY > end.MapPosY)
                    newY = end.MapPosY;
            }
            else if (start.MapPosY > end.MapPosY)
            {
                newY -= steps;
                if (newY < end.MapPosY)
                    newY = end.MapPosY;
            }

            return new Coords((short)newX, (short)newY);

        }

    }
}
