﻿using Enum.Main.CharacterEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Extensions;

namespace World.Network.Handlers
{
    public class GameStartHandler
    {
        public static async Task HandleGameStart(ClientSession session, string[] parts) // GameStart
        {
            var instance = WorldManager.GetInstance(session.Player.Character.MapId);
            await session.SendPacket($"say 1 1 12 Bienvenido {session.Player.Name}!");

            await session.SendPacket("glist 0 0");
            
          //  await session.SendPacket($"in 1 {session.Player.Name} - 532 123 876 2 1 3 45 2 654.321.543.678.987.345.234.123 76 89 0 123 4 2 0 5 1 456 789 321.432.543.654.765.876.987.123 -1 FamiliaLocaxd 2 0 4 1 3 45 12 0|0|1  1 99 100 23");
            await session.SendPacket("mapout");
            await session.SendPacket("rsfi 1 1 0 9 0 9");
            await session.SendPacket(session.Player.Packets.GeneratePInfo());
            await session.SendPacket(session.Player.Packets.GenerateLev());
            await session.SendPacket("c_close 1");
            await session.SendPacket("c_mode 1 1 0 0 0 0 10 0");
            session.Player.Speed = (byte)(session.Player.Class != ClassId.Mage ? 12 : 11);
            await session.Player.ChangeMap(instance, session.Player.Character.MapPosX, session.Player.Character.MapPosY);
            await session.SendPacket(session.Player.Packets.GenerateMapInfo());
            await session.SendPacket(session.Player.Packets.GenerateScale());
            await session.SendPacket(session.Player.Packets.GenerateStat());
            await session.SendPacket(session.Player.Packets.GenerateFood());
            await session.SendPacket(session.Player.Packets.GenerateRage());
        }
    }
}
