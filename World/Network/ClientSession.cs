using Database.Player;
using Microsoft.EntityFrameworkCore;
using NosCryptLib.Encryption;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace World.Network
{
    public class ClientSession
    {
        private Socket _socket;
        private readonly WorldCryptography _cryptography = new WorldCryptography();
        public Account Account { get; set; }
        private PacketHandler _packetHandler;
        
        public int SessionId { get; set; }

        public ClientSession(Socket socket, PacketHandler handler)
        {
            _socket = socket;
            _packetHandler = handler;
        }

        public async Task SetupAcccount(string name)
        {
            using (var context = new AppDbContext())
            {
                Account = await context.Accounts.FirstOrDefaultAsync(x => x.Username == name);
                Log.Information($"Account: {Account.Username} is connected!");
            }
        }

        public async Task SendPacket(string packet)
        {
            await _socket.SendAsync(_cryptography.Encrypt(packet), SocketFlags.None);
        }

        public async Task ReceivePacket(string packet)
        {
            try
            {
                string pck = Regex.Replace(packet, @"game_start\d+", "game_start");
                Log.Information($"Simulated packet from game: {pck}");

                var parts = pck.Split(' ');
                Console.WriteLine(pck);
                if (parts.Length > 1)
                {
                    await _packetHandler.Handle(this, "5555 " + pck); // 5555 lol xd
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error in simulated ReceivePacket: {e.Message}");
            }
        }
    }
}
