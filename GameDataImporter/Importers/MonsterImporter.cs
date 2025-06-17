using Database.Helper;
using Database.MapEntity;
using Enum.Main.PlayerEnum;
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
    public static class MonsterImporter
    {
        public static async Task Import()
        {
            var sw = Stopwatch.StartNew();

            var mobMvPackets = new HashSet<int>(
                PacketFileTxt.packets
                    .Where(p => p[0] == "mv" && p[1] == "3")
                    .Select(p => int.Parse(p[2]))
            );

            var existingVNums = new HashSet<short>(await WorldDbHelper.GetAllMonsterVNums());
            var existingIds = new HashSet<int>(await WorldDbHelper.GetAllMonsterIds());

            var monsters = new List<Monster>();
            var seenMonsterIds = new HashSet<int>();
            short currentMap = 0;

            foreach (var packet in PacketFileTxt.packets)
            {
                // Actualiza el mapa si es paquete "at"
                if (packet[0] == "at" && packet.Length > 5)
                {
                    currentMap = short.Parse(packet[2]);
                    continue;
                }

                if (packet[0] != "in" || packet[1] != "3" || packet.Length <= 7)
                    continue;

                short vnum = short.Parse(packet[2]);
                int monsterId = int.Parse(packet[3]);

                if (existingVNums.Contains(vnum)
                 || existingIds.Contains(monsterId)
                 || seenMonsterIds.Contains(monsterId))
                {
                    continue;
                }

                var m = new Monster
                {
                    MapId = currentMap,
                    VNum = vnum,
                    MonsterId = monsterId,
                    MapX = short.Parse(packet[4]),
                    MapY = short.Parse(packet[5]),
                    Position = byte.TryParse(packet[6], out var b) ? b : (byte)0,
                    IsMoving = mobMvPackets.Contains(monsterId)
                };

                monsters.Add(m);
                seenMonsterIds.Add(monsterId);
            }
            await WorldDbHelper.InsertMonstersAsync(monsters);

            sw.Stop();
            Log.Information("Imported {Count} monsters in {Elapsed} ms.",
                            monsters.Count, sw.ElapsedMilliseconds);
        }
    }
}