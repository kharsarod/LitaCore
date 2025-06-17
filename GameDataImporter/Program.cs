using GameDataImporter.Importers;
using GameDataImporter.Packet;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataImporter
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            await PacketFileTxt.LoadPacket();

            // Specify if you want to import items, maps or another thing pressing 1,2,3, depends on what you want and write in console the importers enumerated.
            // Make the message better an aesthetically better.
            Log.Information("What do you want to import?");
            Log.Information("\n1. Items\n2. Maps\n3. Portals\n4. NpcMonsterData\n5. Monsters\n6. Skills\n7. Buffs\n50 All");
            var input = Console.ReadLine();

            if (input == "1")
            {
                Log.Information("Importing items...");
                await ItemImporter.Import();
            }
            else if (input == "2")
            {
                Log.Information("Importing maps...");
                await MapImporter.Import();
            }
            else if (input == "3")
            {
                Log.Information("Importing portals...");
                await PortalImporter.Import();
            }
            else if (input == "4")
            {
                Log.Information("Importing NpcMonsterData...");
                await NpcMonsterImporter.Import();
            }
            else if (input == "5")
            {
                Log.Information("Importing monsters...");
                await MonsterImporter.Import();
            }
            else if (input == "6")
            {
                Log.Information("Importing skills...");
                await SkillImporter.Import();
            }
            else if (input == "7")
            {
                Log.Information("Importing buffs...");
                await BuffImporter.Import();
            }
            else if (input == "50")
            {
                Log.Information("Importing items...");
                await ItemImporter.Import();
                Log.Information("Importing maps...");
                await MapImporter.Import();
                Log.Information("Importing portals...");
                await PortalImporter.Import();
                Log.Information("Importing NpcMonsterData...");
                await NpcMonsterImporter.Import();
                Log.Information("Importing monsters...");
                await MonsterImporter.Import();
                Log.Information("Importing skills...");
                await SkillImporter.Import();
            }

            Log.Information("Import completed successfully.");
            Log.Information("Press any key to exit.");
            Console.ReadKey();
        }
    }
}