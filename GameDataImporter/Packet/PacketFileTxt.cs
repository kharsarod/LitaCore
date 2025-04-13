using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataImporter.Packet
{
    public static class PacketFileTxt
    {
        public static List<string[]> packets = new List<string[]>();

        public static async Task LoadPacket()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Parser", "Packet", "packets.txt");
            using (StreamReader reader = new StreamReader(path, Encoding.GetEncoding(1252)))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] splitter = line.Split(' ');
                    packets.Add(splitter);
                }
            }
        }
    }
}
