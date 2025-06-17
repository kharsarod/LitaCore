using Database.Helper;
using GameWorld;
using NosCryptLib.Encryption;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using World.Network.Handlers;

namespace World.Network
{
    public class NetworkManager
    {
        private readonly int _serverId;

        public NetworkManager(int serverId)
        {
            _serverId = serverId;
        }

        public async Task StartAsync()
        {
            var channels = await AuthDbHelper.LoadAllChannelsAsync();
            var serverChannels = channels.Where(c => c.ServerId == _serverId);

            if (!serverChannels.Any())
            {
                Log.Error("No channels found for serverId: {ServerId}." +
                    " Create a server in the table 'servers' and in the ServerId column you must place the ID you want and then place that ID in the .bat.\n" +
                    "if you have already created a server, check that the serverId is correct or if channel with that serverId exists.", _serverId);
                return;
            }

            foreach (var channel in serverChannels)
            {
                var listener = new ChannelListener(channel);
                _ = listener.StartAsync();
            }
        }
    }
}