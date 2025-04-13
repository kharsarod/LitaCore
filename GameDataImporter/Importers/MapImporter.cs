using Database.World;
using GameDataImporter.Packet;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataImporter.Importers
{
    public static class MapImporter
    {
        public static async Task Import()
        {
            Stopwatch sw = Stopwatch.StartNew();
            string mapIdDataPath = $"{Path.Combine(Environment.CurrentDirectory, "Parser", "Dat", "MapIDData.dat")}";
            string mapLangPath = $"{Path.Combine(Environment.CurrentDirectory, "Parser", "Txt", "_code_uk_MapIDData.txt")}";
            var dicZts = new Dictionary<int, string>();
            var dicIdLang = new Dictionary<string, string>();
            var dicBgm = new Dictionary<int, int>();
            var maps = new ConcurrentDictionary<short, Map>();

            await foreach(var line in ReadFileAsync(mapIdDataPath))
            {
                var splitter = line.Split(' ');
                if (splitter.Length > 4 && int.TryParse(splitter[0], out int mapId))
                {
                    dicZts[mapId] = splitter[4];
                }
            }

            await foreach (var line in ReadFileAsync(mapLangPath))
            {
                var splitLine = line.Split('\t');
                if (splitLine.Length > 1)
                {
                    dicIdLang[splitLine[0]] = splitLine[1];
                }
            }

            foreach (var line in PacketFileTxt.packets.Where(o => o[0] == "at"))
            {
                if (line.Length > 7 && !dicBgm.ContainsKey(int.Parse(line[2])))
                {
                    dicBgm[int.Parse(line[2])] = int.Parse(line[7]);
                }
            }

            var mapFiles = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Parser", "Maps")).GetFiles();
            await Task.WhenAll(mapFiles.Select(file => ProcessMapFileAsync(file, dicZts, dicIdLang, dicBgm, maps)));

            Log.Information($"Maps imported {maps.Count}, took {sw.ElapsedMilliseconds} ms.");
            sw.Stop();
        }

        private static async Task ProcessMapFileAsync(FileInfo file, Dictionary<int, string> dictionaryZts, Dictionary<string, string> dictionaryIdLang, Dictionary<int, int> dictionaryMusic, ConcurrentDictionary<short, Map> maps)
        {
            var mapId = short.Parse(file.Name);
            var mapData = await File.ReadAllBytesAsync(file.FullName);
            var name = dictionaryZts.ContainsKey(mapId) && dictionaryIdLang.TryGetValue(dictionaryZts[mapId], out var mapName) ? mapName : "";
            var music = dictionaryMusic.TryGetValue(mapId, out var musicId) ? musicId : 0;

            var map = new Map
            {
                Name = name,
                Bgm = music,
                Id = mapId,
                Data = mapData,
                IsShopAllowed = mapId == 147,
            };

            maps[map.Id] = map;
        }

        private static async IAsyncEnumerable<string> ReadFileAsync(string filePath)
        {
            using(var reader = new StreamReader(filePath, Encoding.GetEncoding(1252)))
            {
                string? line;
                while((line = await reader.ReadLineAsync()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
