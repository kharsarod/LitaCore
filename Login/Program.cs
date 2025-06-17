using Database.Context;
using Database.Helper;
using LoginServer.TcpServer;
using LoginServer.Utils;
using Serilog;
using System.Net;

namespace LitaCore
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Presentation
            Console.Title = "LitaCore | LoginServer";
            ConsoleUtils.CenterText("LitaCore LoginServer");
            ConsoleUtils.CenterText("Version: 1.0.0");
            for (byte i = 0; i < 3; i++) Console.WriteLine();

            // Serilog Initialization
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate).CreateLogger();

            Database.Main main = new Database.Main();
            await main.Initialize();
            var servers = await AuthDbHelper.LoadAllServersAsync();
            var server = servers.FirstOrDefault();

            ServerListener listener = new ServerListener(IPAddress.Any.ToString(), 4000);

            _ = Task.Run(async () =>
            {
                try
                {
                    await listener.Initialize();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Login Server failed to start.");
                }
            });

            await Task.Delay(-1);
        }
    }
}