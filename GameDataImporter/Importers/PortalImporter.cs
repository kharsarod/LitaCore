using Database.Helper;
using Database.World;
using Enum.Main.PortalEnum;
using GameDataImporter.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataImporter.Importers
{
    public static class PortalImporter
    {
        public static async Task Import()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<Portal> listPortals1 = new List<Portal>();
            List<Portal> listPortals2 = new List<Portal>();
            short map = 0;

            int portalId = 0;
            foreach (string[] currentPacket in PacketFileTxt.packets.Where(o => o[0].Equals("c_map") || o[0].Equals("gp")))
            {
                if (currentPacket.Length > 3 && currentPacket[0] == "c_map")
                {
                    map = short.Parse(currentPacket[2]);
                    continue;
                }

                if (currentPacket.Length > 4 && currentPacket[0] == "gp")
                {
                    Portal portal = new Portal
                    {
                        FromMapId = map,
                        FromMapX = short.Parse(currentPacket[1]),
                        FromMapY = short.Parse(currentPacket[2]),
                        ToMapId = short.Parse(currentPacket[3]),
                        ToMapX = short.Parse(currentPacket[4]),
                        ToMapY = short.Parse(currentPacket[5]),
                        Type = (PortalType)sbyte.Parse(currentPacket[4]),
                    };
                    // Comprobar si el portal ya existe en la lista o en la base de datos
                    if (listPortals1.Any(s => s.FromMapId == map && s.FromMapX == portal.FromMapX && s.FromMapY == portal.FromMapY && s.ToMapId == portal.ToMapId) ||
                        !ExistsInMaps(portal.FromMapId) || !ExistsInMaps(portal.ToMapId))
                    {
                        continue; // Portal ya en la lista o en mapas no existentes
                    }
                    listPortals1.Add(portal);
                }
            }

            // Ordenar los portales
            listPortals1 = listPortals1.OrderBy(s => s.FromMapId)
                                       .ThenBy(s => s.ToMapId)
                                       .ThenBy(s => s.FromMapY)
                                       .ThenBy(s => s.FromMapX)
                                       .ToList();

            // Emparejar portales de ida y vuelta
            foreach (Portal portal in listPortals1)
            {
                Portal p = listPortals1.Except(listPortals2)
                                          .FirstOrDefault(s => s.FromMapId == portal.ToMapId && s.ToMapId == portal.FromMapId);

                if (p == null)
                {
                    continue;
                }

                portal.ToMapX = p.FromMapX;
                portal.ToMapY = p.FromMapY;
                p.ToMapY = portal.FromMapY;
                p.ToMapX = portal.FromMapX;

                listPortals2.Add(p);
                listPortals2.Add(portal);
            }

            await WorldDbHelper.InsertPortalsAsync(listPortals2);

            Log.Information($"Portals parsed in {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Stop();
        }

        private static bool ExistsInMaps(short mapId)
        {
            return WorldDbHelper.LoadPortalByMapId(mapId) != null;
        }
    }
}