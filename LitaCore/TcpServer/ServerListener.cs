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
            Log.Information($"Server started on {IpAddress}:{Port}");

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
            
            try
            {
                while (true)
                {
                    int received = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (received == 0)
                    {
                        break; // Client disconnected.
                    }


                    string packet = $"{loginPacket.Packets[0]} 0 admin {mdr} 0 {IPAddress.Any}:{_settings.Configuration.Port_CH1}:1:1.1.{_settings.Configuration.ServerName} -1:-1:-1:-1:-1:-1";

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
