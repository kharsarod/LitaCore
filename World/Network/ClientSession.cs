using NosCryptLib.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace World.Network
{
    public class ClientSession
    {
        private Socket _socket;
        private readonly WorldCryptography _cryptography = new WorldCryptography();

        public ClientSession(Socket socket)
        {
            _socket = socket;
        }

        public async Task SendPacket(string packet)
        {
            await _socket.SendAsync(_cryptography.Encrypt(packet), SocketFlags.None);
        }
    }
}
