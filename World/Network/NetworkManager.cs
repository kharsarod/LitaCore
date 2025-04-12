using Configuration.ServerConf;
using NosCryptLib.Encryption;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using World.Network.Handlers;

namespace World.Network
{
    public class NetworkManager
    {
        private readonly Socket _socket;
        private readonly WorldCryptography _cryptography = new WorldCryptography();
        private ServerJsonConf _conf = new ServerJsonConf();
        private AppSettings _settings;
        private PacketHandler _packetHandler = new();
        private PacketDecryptor _packetDecryptor = new();

        public NetworkManager(AppDbContext context)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _packetHandler.Register("Char_NEW", CharacterHandler.HandleCharacterCreation);
            _packetHandler.Register("ORG", CharacterHandler.HandleCharacterLoad);
            _packetHandler.Register("select", CharacterHandler.HandleSelect);
            _packetHandler.Register("guri", GuriHandler.HandleGuri);
            _packetHandler.Register("game_start", GameStartHandler.HandleGameStart);
            _packetHandler.Register("walk", WalkHandler.HandleWalk);
            _packetHandler.Register("Char_DEL", CharacterHandler.HandleCharacterDelete);

            // commands
            _packetHandler.Register("$morph", CommandHandler.Morph);
        }

        public async Task StartAsync()
        {
            _settings = await _conf.ReadConfigAsync();
            _socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), _settings.Configuration.Port_CH1));
            _socket.Listen(100);
            Console.WriteLine("Listening on port " + _settings.Configuration.Port_CH1);

            while (true)
            {
                var client = await _socket.AcceptAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private async Task HandleClient(Socket client)
        {
            try
            {
                var session = new ClientSession(client, _packetHandler);
                while (true)
                {
                    var buffer = new byte[1024];
                    var bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesRead == 0)
                        break;

                    var raw = new byte[bytesRead];
                    Array.Copy(buffer,raw, bytesRead);

                    var data = _cryptography.Decrypt(raw);

                    string[] packets = data.Split((char)0xFF);
                    string pck = string.Empty;
                    foreach (var packet in packets)
                    {
                        pck += packet;
                    }

                    string test = _cryptography.DecryptUnauthed(raw);
                    
                    var gamePacket = pck.Split(' ');

                    if (gamePacket.Length > 1)
                    {
                        pck = Regex.Replace(pck, @"game_start\d+", "game_start");
                        Log.Information($"Packet from game: {pck}");

                        if (pck.Split(' ')[1].StartsWith("$music"))
                        {
                            await session.SendPacket($"bgm {pck.Split(' ')[2]}");
                            await session.SendPacket($"say 1 1 12 BGM cambiado al {pck.Split(' ')[2]}.");
                        }

                        if (data.Contains("ORG"))
                        {
                            await session.SendPacket("clist_start 0");
                            await session.SetupAcccount(pck.Split(' ')[1]);
                            await session.SendPacket("clist_end");

                            var p = pck.Split(' ');
                            pck = $"{p[0]} ORG";
                        }



                        if (pck.Split(' ')[1].StartsWith("$packet"))
                        {
                            var args = string.Join(" ", pck.Split(' ').Skip(2));
                            await session.SendPacket($"{args}");
                        }

                        try
                        {
                            await _packetHandler.Handle(session, pck);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }
                        
                    }
                    
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
