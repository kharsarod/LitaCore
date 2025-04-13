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
    static class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            await PacketFileTxt.LoadPacket();
            await MapImporter.Import();

            Log.Information("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
