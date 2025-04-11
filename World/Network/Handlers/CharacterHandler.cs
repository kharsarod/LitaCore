using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Network.Handlers
{
    public class CharacterHandler
    {
        public static async Task HandleCharacterCreation(ClientSession session, string[] parts) // Cambiar HandleCharacter -> HandleCharacterCreation
        {
            await session.SendPacket("clist_start 0");
            await session.SendPacket("clist 0 Kharsarod 0 1 2 3 4 2 12 432.123.876.94.76.589.321.877 11 1 1 192 0");
            await session.SendPacket("clist_end");
        }

        public static async Task HandleSelect(ClientSession session, string[] parts) // Cambiar Select -> HandleSelect
        {
            await session.SendPacket("OK");
            await session.SendPacket("game_start");
            await session.SendPacket("lbs 0");
            await session.SendPacket("glist 0 0");
            await session.SendPacket("in 1 [GS]Player123 - 532 123 876 2 1 3 45 2 654.321.543.678.987.345.234.123 76 89 0 123 4 2 0 5 1 456 789 321.432.543.654.765.876.987.123 -1 FamiliaLocaxd 2 0 4 1 3 45 12 0|0|1  1 99 100 23");
            await session.SendPacket("mapout");
            await session.SendPacket("rsfi 1 1 0 9 0 9");
            await session.SendPacket("c_info Kharsarod - -1 -1 - 1 0 0 1 1 1 27 0 0 0 0 0 0");
            await session.SendPacket("c_close 1");
            await session.SendPacket("c_mode 1 1 0 0 0 0 10 0");
            await session.SendPacket("at 1 1 99 99 0 0 12 2 -1");
        }

        public static async Task HandleGuri(ClientSession session, string[] parts) // Guri
        {
            if (int.Parse(parts[5]) >= 973 && int.Parse(parts[5]) <= 999)
            {
                await session.SendPacket($"eff 1 1 {int.Parse(parts[5]) + 4099}");
            }
        }
    }
}
