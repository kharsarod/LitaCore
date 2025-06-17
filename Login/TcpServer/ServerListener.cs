using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Serilog;
using NosCryptLib.Encryption;
using Microsoft.Extensions.DependencyInjection;
using LoginServer.TcpSession;
using System.Security.Cryptography.X509Certificates;
using LoginServer.Types;
using System.Text.RegularExpressions;
using Enum.Main.PlayerEnum;
using Database.Player;
using Database.Server;
using Database.Context;
using Database.Helper;

namespace LoginServer.TcpServer
{
    public class ServerListener
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private string IpAddress { get; set; }
        private int Port { get; set; }
        private LoginCryptography _cryptography = new LoginCryptography();
        private int _serverPort = 0;
        private int _sessionId;

        private int GenerateSessionId()
        {
            _sessionId += 2;
            return _sessionId;
        }

        public ServerListener(string IpAddress, int Port)
        {
            this.IpAddress = IpAddress;
            this.Port = Port;
        }

        public async Task Initialize()
        {
            try
            {
                _socket.Bind(new IPEndPoint(System.Net.IPAddress.Parse(IpAddress), Port));
                _socket.Listen(10);
                Log.Information("Login Server started on {IpAddress}:{Port}", IpAddress, Port);
                while (true)
                {
                    var client = await _socket.AcceptAsync();
                    _ = Task.Run(() => HandleClient(client));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        private async Task HandleClient(Socket client)
        {
            ClientSession session = new ClientSession(client);
            session.SessionId = GenerateSessionId();

            var buffer = new byte[8192];
            string mdr = "";
            var servers = await AuthDbHelper.LoadAllServersAsync();
            var channels = await AuthDbHelper.LoadAllChannelsAsync();

            int activeServers = servers.Count;
            int repeats = 61 - activeServers;

            Log.Information($"New client connected from {((IPEndPoint)client.RemoteEndPoint).Address}:{((IPEndPoint)client.RemoteEndPoint).Port}");

            try
            {
                while (true)
                {
                    int received = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (received == 0)
                    {
                        break;
                    }
                    var data = _cryptography.Decrypt(buffer);
                    string[] splitter = data.Split(' ');

                    var account = await AuthDbHelper.LoadAccountByNameAsync(splitter[2]);

                    var regex = Regex.Match(splitter[6], @"^\d+");
                    session.Language = (LanguageType)byte.Parse(regex.Value);
                    session.Username = splitter[2];

                    if (account != null)
                    {
                        if (account.IsBanned)
                        {
                            await session.SendPacket("failc 7");
                        }
                        if (account.IsOnline)
                        {
                            await session.SendPacket("failc 4");
                        }
                        if (splitter[3] == account.Password.ToUpper())
                        {
                            account.LastLogin = DateTime.Now;
                            account.IpAddress = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
                            account.Language = (Language)session.Language;
                            string serversList = "";

                            foreach (var server in servers)
                            {
                                var characters = await CharacterDbHelper.LoadCharactersByServerIdAndAccountId(server.ServerId, account.Id);
                                serversList += $" {server.ServerId} {characters.Count}";
                            }
                            for (byte i = 1; i != repeats; i++)
                            {
                                mdr += "-99 0 ";
                            }

                            StringBuilder packet = new StringBuilder();
                            packet.Append($"NsTeST  {(byte)session.Language} {account.Username} {serversList} {mdr} {session.SessionId} ");

                            int channelCounter = 0;

                            foreach (var server in servers)
                            {
                                var serverChannels = channels.Where(x => x.ServerId == server.ServerId);
                                foreach (var channel in serverChannels)
                                {
                                    channelCounter++;
                                    var serverPlayers = Math.Round((double)channel.OnlinePlayers / channel.MaxPlayers * 20) + 1;
                                    packet.Append($"{(channel.LocalMode ? channel.IpAddress : channel.PublicAddress)}:{channel.ChannelPort}:{serverPlayers}:{server.ServerId}.{channel.ChannelId}.{server.ServerName} ");
                                }
                            }

                            packet.Append("-1:-1:-1:10000.10000.1");
                            await session.SendPacket(packet.ToString());
                            Log.Information("ChannelInfo {Packet}", packet.ToString());
                            packet.Clear();
                        }
                        else
                        {
                            await session.SendPacket("failc 5");
                        }
                    }
                    else
                    {
                        await session.SendPacket("failc 5");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            finally
            {
                client.Close();
                Log.Information("Client Disconnected");
            }
        }
    }
}