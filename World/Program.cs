using Serilog;
using World.Network;

namespace World
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            NetworkManager networkManager = new NetworkManager(new AppDbContext());
            await networkManager.StartAsync();
        }
    }
}
