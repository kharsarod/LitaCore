using Database.Commands;
using Serilog;
using World.Network;

namespace World
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code).CreateLogger();
            NetworkManager networkManager = new NetworkManager(new AppDbContext());

            _ = Task.Run(() => networkManager.StartAsync());

            while (true)
            {
                string input = Console.ReadLine();
                Log.Information(input);

                if (input.StartsWith("!acc_create"))
                {
                    string[] parts = input.Split(' ', 2);

                    if (parts.Length > 1)
                    {
                        await AccountCreate.CreateAccount(input.Split(' ')[1], input.Split(' ')[2], byte.Parse(input.Split(' ')[3]));
                    }
                    
                }

                if (input == "exit")
                {
                    break;
                }
            }
        }
    }
}
