using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Serilog;
using Encryption;
using Microsoft.Extensions.DependencyInjection;
using NosTalePacketsLib.Login;
using LoginServer.TcpSession;
using Configuration.ServerConf;
using System.Security.Cryptography.X509Certificates;
using NosCryptLib.Encryption;
using LoginServer.Types;
using System.Text.RegularExpressions;


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
        public ServerListener(string IpAddress, int Port)
        {
            this.IpAddress = IpAddress;
            this.Port = Port;
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
                        break; // Client disconnected.
                    }



                    // Obtener paquete
                    var data = _cryptography.Decrypt(buffer);
                    string[] splitter = data.Split(' ');
                    var regex = Regex.Match(splitter[6], @"^\d+");
                    session.SessionId = int.Parse(splitter[1]);
                    session.Username = splitter[2];
                    session.Password = Cryptography.ToSha512(splitter[3]);
                    session.Language = (LanguageType)byte.Parse(regex.Value);


                    string packet = $"{loginPacket.Packets[0]} {(byte)session.Language} {session.Username} {mdr} 0 {IPAddress.Any}:{_settings.Configuration.Port_CH1}:1:1.1.{_settings.Configuration.ServerName} -1:-1:-1:-1:-1:-1";
                    await session.SendPacket(packet);
                    
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
