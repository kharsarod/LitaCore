using Enum.Main.ChatEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.GameWorld;
using World.Network.Interfaces;
using World.Utils;
using static System.Collections.Specialized.BitVector32;

namespace World.Network.Handlers
{
    public class WalkHandler : IPacketGameHandler
    {
        private static readonly Dictionary<long, (Vector2 LastPosition, DateTime LastCheck)> _speedTrackers = new();

        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("walk", HandleWalk);
        }

        public static async Task HandleWalk(ClientSession session, string[] parts)
        {
            if (parts.Length < 4 ||
                !short.TryParse(parts[2], out var targetX) ||
                !short.TryParse(parts[3], out var targetY))
            {
                return;
            }

            var map = session.Player.CurrentMap;

            if (PathFindingMap.IsBlockedZone(targetX, targetY, map.Template.MapGrid) ||
                !PathFindingMap.IsWalkable(targetX, targetY, map.Template.MapGrid, map.Template.Width, map.Template.Height))
            {
                return;
            }

            await map.Broadcast(session.Player.Packets.GenerateMove(targetX, targetY), session);

            session.Player.WalkDisposable?.Dispose();
            var interval = Math.Max(100 - session.Player.Speed * 5 + 100, 0);
            if (!session.Player.HasCustomSpeed)
            {
                await session.Player.UpdateSpeed();
            }
            session.Player.WalkDisposable = Observable.Interval(TimeSpan.FromMilliseconds(session.Player.Speed * 5 * 0.20)).Subscribe(async _ =>
            {
                var player = session.Player;
                var currentX = player.MapPosX;
                var currentY = player.MapPosY;

                var nextCoords = PathFindingMap.GetNextStep(new Coords(currentX, currentY), new Coords(targetX, targetY), 1);

                player.SetOrientation(currentX, currentY, nextCoords.MapPosX, nextCoords.MapPosY);
                player.MapPosX = nextCoords.MapPosX;
                player.MapPosY = nextCoords.MapPosY;

                await session.SendPacket(player.Packets.GeneratePlayerMove());

                if (player.MapPosX == targetX && player.MapPosY == targetY)
                {
                    player.WalkDisposable?.Dispose();
                }
                player.LastMove = DateTime.Now;
            });
        }
    }
}