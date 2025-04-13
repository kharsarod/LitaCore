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
using World.Entities;

namespace World.Network
{
    public class ClientSession
    {
        private Socket _socket;
        private static int _idCounter = 0;
        private readonly WorldCryptography _cryptography = new WorldCryptography();
        public Account Account { get; set; }
        private Character Character { get; set; }
        public Player Player { get; set; }
        private PacketHandler _packetHandler;
        public bool IsAlreadyConnected { get; set; }

        public int ClientId { get; private set; }
        
        public int SessionId { get; set; }

        public ClientSession(Socket socket, PacketHandler handler)
        {
            _socket = socket;
            _packetHandler = handler;
            ClientId = Interlocked.Increment(ref _idCounter);
        }

        public async Task SetupAcccount(string name)
        {
            using (var context = new AppDbContext())
            {
                Account = await context.Accounts.FirstOrDefaultAsync(x => x.Username == name);
                Log.Information($"Account: {Account.Username} with clientId: {ClientId} is connected!");
            }

            if (SessionManager.IsConnected(Account.Username))
            {
                IsAlreadyConnected = true;
            }
            else
            {
                IsAlreadyConnected = false;
                SessionManager.Register(Account.Username, this);
            }
        }

        public async Task Disconnect()
        {
            if ( Account != null && !string.IsNullOrEmpty(Account.Username))
            {
                SessionManager.Unregister(Account.Username);
            }
            _socket.Close();
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
