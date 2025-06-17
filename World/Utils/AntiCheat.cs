using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network;

namespace World.Utils
{
    public class AntiCheat
    {
        private int _packetCount = 0;
        private int _warningCount = 0;

        private readonly int _maxPacketsPerSecond = 50; // puedes ajustar este valor
        private readonly int _maxWarnings = 3;

        private readonly Timer _checkTimer;
        private readonly ClientSession _session;

        public AntiCheat(ClientSession session)
        {
            _session = session;
            _checkTimer = new Timer(CheckPacketRate, null, 500, 500); // cada segundo
        }

        public void RegisterPacket()
        {
            Interlocked.Increment(ref _packetCount);
        }

        private void CheckPacketRate(object state)
        {
            int count = Interlocked.Exchange(ref _packetCount, 0);

            if (count > 2)
            {
                _warningCount++;
                Console.WriteLine($"[AntiCheat] Exceso de paquetes: {count} paquetes/s. Warn {_warningCount}/{_maxWarnings}");

                if (_warningCount >= _maxWarnings)
                {
                    KickPlayer($"Flood de paquetes detectado ({count} paquetes/s)");
                }
            }
            else
            {
                // Si se comporta bien, reducimos una advertencia lentamente
                if (_warningCount > 0)
                    _warningCount--;
            }
        }

        private void KickPlayer(string reason)
        {
            Console.WriteLine($"Jugador expulsado: {reason}");
        }
    }
}