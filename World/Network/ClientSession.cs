using Database.Context;
using Database.Helper;
using Database.Player;
using Enum.Main.CharacterEnum;
using GameWorld;
using Microsoft.EntityFrameworkCore;
using NosCryptLib.Encryption;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using World.Entities;
using World.Extensions;
using World.GameWorld;
using World.Network.Handlers;
using World.Network.Interfaces;
using World.Utils;
using static System.Collections.Specialized.BitVector32;
using Enum.Main.PlayerEnum;
using Configuration;

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
        public ICommandRegister CommandHandler { get; set; }
        public bool IsAlreadyConnected { get; set; }
        private readonly PacketHandler handler = new();
        private readonly Queue<string> _PacketQueue = new();
        private bool _isProcessingPackets = false;
        private readonly SemaphoreSlim _queueLock = new(1, 1);
        private bool _isLoggedIn = false;
        private Language PlayerLanguage { get; set; }
        public bool IsInGame { get; set; }

        public int ClientId { get; private set; }

        public int SessionId { get; set; }

        public byte ChannelId { get; set; }

        public int _lastPacketId = 0;

        public ClientSession(Socket socket)
        {
            _socket = socket;
            ClientId = Interlocked.Increment(ref _idCounter);
            HandlerInitializer.RegisterAll(handler);
        }

        public async Task EnqueuePacket(string packet)
        {
            await _queueLock.WaitAsync();
            try
            {
                _PacketQueue.Enqueue(packet);
                if (!_isProcessingPackets)
                {
                    _isProcessingPackets = true;
                    _ = RunAsync();
                }
            }
            finally
            {
                _queueLock.Release();
            }
        }

        public async Task SetupAcccount(string name)
        {
            try
            {
                using (var context = new AuthDbContext())
                {
                    Account = await context.Accounts.FirstOrDefaultAsync(x => x.Username == name);
                    Log.Information($"Account: {Account.Username} with sessionId: {SessionId} is connected!");
                }

                if (SessionManager.IsConnected(Account.Username))
                {
                    IsAlreadyConnected = true;
                }
                else
                {
                    IsAlreadyConnected = false;
                    SessionManager.Register(Account.Username, this);
                    Account.IsOnline = true;
                    await AuthDbHelper.UpdateAsync(Account);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while setting up account.");
            }
        }

        public async Task Ban()
        {
            Account.IsBanned = true;
            await AuthDbHelper.UpdateAsync(Account);
            await Disconnect();
        }

        public async Task Disconnect()
        {
            try
            {
                IsAlreadyConnected = false;

                if (Account != null && !string.IsNullOrEmpty(Account.Username))
                {
                    SessionManager.Unregister(Account.Username);
                    Log.Information("Disconnected session for user {Username}", Account.Username);
                }
                else
                {
                    Log.Information("Disconnected session with no linked account.");
                }

                if (_socket?.Connected == true)
                {
                    try
                    {
                        if (Account is not null)
                        {
                            Account.IsOnline = false;
                            await AuthDbHelper.UpdateAsync(Account);
                        }
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (SocketException se)
                    {
                        Log.Warning(se, "Socket already closed or failed to shutdown cleanly.");
                    }
                }

                _socket?.Close();
                _socket?.Dispose();
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        public async Task StartReceiving()
        {
            try
            {
                while (true)
                {
                    var buffer = new byte[1024];
                    if (_socket == null) return;
                    var bytesRead = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesRead == 0) break;

                    var raw = new byte[bytesRead];
                    Array.Copy(buffer, raw, bytesRead);

                    if (SessionId == 0)
                    {
                        string sessionPacket = _cryptography.DecryptUnauthed(raw);
                        var splitter = sessionPacket.Split(' ');
                        if (splitter.Length == 0)
                        {
                            return;
                        }

                        if (!int.TryParse(splitter[0], out var packetId))
                        {
                            Log.Error("Failed to parse packet id.");
                            await Disconnect();
                            return;
                        }

                        if (splitter.Length < 2)
                        {
                            return;
                        }

                        _lastPacketId = packetId;

                        if (int.TryParse(splitter[1].Split('\\').FirstOrDefault(), out int _sessionId))
                        {
                            SessionId = _sessionId;
                        }

                        continue;
                    }

                    var data = _cryptography.Decrypt(raw, PlayerEncoding.GetPlayerEncoding(PlayerLanguage), SessionId);

                    string[] packets = data.Split(new[] { (char)0xFF }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var pck in packets)
                    {
                        await EnqueuePacket(pck);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error receiving data");
            }
            finally
            {
                Log.Information("Client disconnected.");
                if (Player != null)
                    await Player.SaveCharacterOnDisconnect();
                await Disconnect();
            }
        }

        public async Task RunAsync()
        {
            while (true)
            {
                string packet = null;

                await _queueLock.WaitAsync();
                try
                {
                    if (_PacketQueue.Count == 0)
                    {
                        _isProcessingPackets = false;
                        return;
                    }

                    packet = _PacketQueue.Dequeue();
                }
                finally
                {
                    _queueLock.Release();
                }

                try
                {
                    if (ConfigManager.WorldServerConfig.Features.AllowPacketLogging)
                    {
                        Log.Information("Processing packet: {Pck}", packet);
                    }

                    var gamePacket = packet.Split(' ');

                    if (gamePacket.Length > 1 && !_isLoggedIn)
                    {
                        await SendPacket("thisisgfmode");
                        if (gamePacket.Contains("ORG"))
                        {
                            var channels = await AuthDbHelper.LoadAllChannelsAsync();
                            var channel = channels.FirstOrDefault(x => x.ChannelId == ChannelId);
                            var serverPlayersOnline = Math.Round((double)channel.OnlinePlayers / channel.MaxPlayers * 20) + 1;
                            if (serverPlayersOnline > 18)
                            {
                                await SendPacket("infoi 354 0 0 0");
                                await SendPacket("svrlist");
                                return;
                            }
                            await SetupAcccount(gamePacket[1]);
                        }
                        _isLoggedIn = true;
                        await CharacterHandler.HandleCharacterLoad(this, gamePacket);
                    }

                    await handler.Handle(this, packet);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error handling packet");
                }
            }
        }

        public async Task SendPacket(string packet)
        {
            try
            {
                if (_socket?.Connected == true)
                {
                    await _socket.SendAsync(_cryptography.Encrypt(packet, PlayerEncoding.GetPlayerEncoding(PlayerLanguage)), SocketFlags.None);
                }
                else
                {
                    _socket?.Close();
                    _socket?.Dispose();
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to send packet to session {(Account?.Username ?? "Unknown")}: ", e);
                await Disconnect();
            }
        }

        public async Task SendPackets(List<string> packets)
        {
            foreach (var packet in packets)
            {
                await SendPacket(packet.ToString());
            }
        }
    }
}