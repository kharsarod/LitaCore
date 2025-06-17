using Database.Helper;
using Database.Server;
using GameWorld;
using NosCryptLib.Encryption;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using World.Network.Handlers;

namespace World.Network
{
    public class ChannelListener
    {
        private readonly ChannelInfo _channel;
        private readonly Socket _socket;
        private PacketHandler _packetHandler = new();
        private readonly WorldCryptography _cryptography = new WorldCryptography();

        public ChannelListener(ChannelInfo channel)
        {
            _channel = channel;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task StartAsync()
        {
            try
            {
                var servers = await AuthDbHelper.LoadAllServersAsync();
                var server = servers.FirstOrDefault(x => x.ServerId == _channel.ServerId);
                _socket.Bind(new IPEndPoint(IPAddress.Parse(_channel.IpAddress), _channel.ChannelPort));
                _socket.Listen(100);
                var accounts = await AuthDbHelper.LoadAllAccountsAsync();
                var channels = await AuthDbHelper.LoadAllChannelsAsync();
                foreach (var account in accounts.Where(x => x.IsOnline))
                {
                    account.IsOnline = false;
                    await AuthDbHelper.UpdateAsync(account);
                }

                foreach (var channel in channels.Where(x => x.OnlinePlayers > 0))
                {
                    channel.OnlinePlayers = 0;
                    await AuthDbHelper.UpdateChannelAsync(channel);
                }

                WorldManager.ServerId = server.ServerId;

                while (true)
                {
                    var client = await _socket.AcceptAsync();
                    _ = Task.Run(() => HandleClient(client, _channel.ChannelId));
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while listening on channel {Id}", _channel.Id);
            }
        }

        private async Task HandleClient(Socket client, byte channelId)
        {
            var session = new ClientSession(client);
            session.ChannelId = channelId;

            await session.StartReceiving();
        }
    }
}