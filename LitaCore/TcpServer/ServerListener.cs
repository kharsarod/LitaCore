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
using Configuration.ServerConf;
using System.Security.Cryptography.X509Certificates;
using NosCryptLib.Encryption;
using LoginServer.Types;
using System.Text.RegularExpressions;
using Database.Data.Repositories;
using NosTalePacketsLib.Packets.Client;


namespace LoginServer.TcpServer
{
    public class ServerListener
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private string IpAddress { get; set; }
        private int Port { get; set; }
        private LoginPacket loginPacket = new LoginPacket();
        ServerJsonConf conf = new ServerJsonConf();
        private AppSettings _settings = new AppSettings();
        private LoginCryptography _cryptography = new LoginCryptography();
        private readonly AccountRepository accountRepository;

        public ServerListener(string IpAddress, int Port, AppDbContext dbContext)
        {
            this.IpAddress = IpAddress;
            this.Port = Port;
            this.accountRepository = new AccountRepository(dbContext);
        }

        public async Task Initialize()
        {
            _settings = await conf.ReadConfigAsync();
            _socket.Bind(new IPEndPoint(System.Net.IPAddress.Parse(IpAddress), Port));
            _socket.Listen(10);
            Log.Information($"Login Server started on {IpAddress}:{Port}");

            try
            {
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
            
            var buffer = new byte[8192];
            string mdr = string.Empty;
            for (int i = 0; i != 60; i++)
            {
                mdr += "-99 0 ";
            }

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

                    var account = await accountRepository.LoadByName(splitter[2]);

                    var regex = Regex.Match(splitter[6], @"^\d+");
                    session.SessionId = int.Parse(splitter[1]);
                    session.Language = (LanguageType)byte.Parse(regex.Value);
                    session.Username = splitter[2];
                    if (account != null)
                    {
                        if (account.IsBanned)
                        {
                            await session.SendPacket("failc 7");
                        }
                        if (splitter[3] == account.Password.ToUpper())
                        {
                            string packet = $"{loginPacket.Header} {(byte)session.Language} {account.Username} {mdr} 0 127.0.0.1:{_settings.Configuration.Port_CH1}:1:1.1.{_settings.Configuration.ServerName} -1:-1:-1:-1:-1:-1";
                            await session.SendPacket(packet);
                            session.IsConnected = true;
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
