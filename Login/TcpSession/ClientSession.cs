using LoginServer.Types;
using NosCryptLib.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.TcpSession
{
    public class ClientSession
    {
        private Socket _client;
        private LoginCryptography encrypt = new LoginCryptography();
        public int SessionId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public LanguageType Language { get; set; }
        public bool IsConnected { get; set; }

        public ClientSession(Socket client)
        {
            _client = client;
        }

        public async Task SendPacket(string data)
        {
            await _client.SendAsync(encrypt.Encrypt(data), SocketFlags.None);
        }

        public void Dispose()
        {
            IsConnected = false;
            _client.Close();
        }
    }
}