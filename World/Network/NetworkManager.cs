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
            _packetHandler.Register("Char_NEW_JOB", CharacterHandler.HandleCharacterCreationJob);
            _packetHandler.Register("rest", RestHandler.HandleRest);

            // commands
            _packetHandler.Register("$transform", CommandHandler.Morph);
            _packetHandler.Register("$tp", CommandHandler.Teleport);
            _packetHandler.Register("$speed", CommandHandler.Speed);
            _packetHandler.Register("$mlvl", CommandHandler.ModLevel);
            _packetHandler.Register("$mjlvl", CommandHandler.ModJobLevel);
            _packetHandler.Register("$pinfo", CommandHandler.PlayerInfo);
            _packetHandler.Register("$getmapinfo", CommandHandler.GetMapInfo);
            _packetHandler.Register("$getpos", CommandHandler.GetPosition);
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
            var session = new ClientSession(client, _packetHandler);
            try
            {
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
                            if (SessionManager.IsConnected(pck.Split(' ')[1]))
                            {
                                await session.SendPacket("msg 0 This account already logged in. What are you trying to do?");
                                await client.DisconnectAsync(false);
                                return;
                            }
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
            finally
            {
                Log.Information($"Client disconnected.");
                await session.Player.SaveCharacterOnDisconnect();
                await session.Disconnect();
            }
        }
    }
}
