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
            // Presentation
            Console.Title = "LitaCore | LoginServer";

            // Setting
            var settting = await conf.ReadConfigAsync();
            // Serilog Initialization
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate).CreateLogger();

            Database.Main main = new Database.Main();
            await main.Initialize();

            ServerListener listener = new ServerListener(IPAddress.Any.ToString(), settting.Configuration.Port, new AppDbContext());
            await listener.Initialize();
        }
    }
}
