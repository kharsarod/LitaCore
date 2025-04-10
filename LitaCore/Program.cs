using Configuration.ServerConf;
using LoginServer.TcpServer;
using Serilog;
using System.Net;

namespace LitaCore
{
    internal class Program
    {
        private static protected ServerJsonConf conf = new ServerJsonConf();
        static async Task Main(string[] args)
        {
            var settting = await conf.ReadConfigAsync();
            // Serilog Initialization
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

            ServerListener listener = new ServerListener(IPAddress.Any.ToString(), settting.Configuration.Port);
            await listener.Initialize();
        }
    }
}
