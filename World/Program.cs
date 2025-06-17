using Database.Commands;
using GameWorld;
using Serilog;
using System.Reactive.Linq;
using System.Text;
using World.Network;
using World.Utils;

namespace World
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code).CreateLogger();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ConsoleUtils.CenterText("LitaCore WorldServer");
            ConsoleUtils.CenterText("Version: 1.0.0");
            for (byte i = 0; i < 3; i++) Console.WriteLine();

            await WorldManager.Initialize();

#if RELEASE
            Log.Information("Input a server id, ex: 1");
            var inputServerId = Console.ReadLine();
            if (!int.TryParse(inputServerId, out int serverId))
            {
                Console.WriteLine("Invalid args.");
                return;
            }
            Console.Clear();
#endif
            NetworkManager networkManager = new NetworkManager(1);
            _ = Task.Run(() => networkManager.StartAsync());
            while (true)
            {
                string input = Console.ReadLine();
                Log.Information(input);

                if (input.StartsWith("acc_create"))
                {
                    string[] parts = input.Split(' ', 2);

                    if (parts.Length > 1)
                    {
                        await AccountCreate.CreateAccount(input.Split(' ')[1], input.Split(' ')[2], byte.Parse(input.Split(' ')[3]));
                    }
                }

                switch (input)
                {
                    case "reload_item":
                        await WorldManager.LoadItems();
                        break;

                    case "reload_map":
                        await WorldManager.LoadMaps();
                        break;
                }

                if (input == "exit")
                {
                    break;
                }
            }
        }
    }
}