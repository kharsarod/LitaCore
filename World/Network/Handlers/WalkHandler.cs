using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.GameWorld;
using World.Utils;
using static System.Collections.Specialized.BitVector32;

namespace World.Network.Handlers
{
    public class WalkHandler
    {
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

            session.Player.WalkDisposable = Observable.Interval(TimeSpan.FromMilliseconds(interval)).Subscribe(async _ =>
            {
                var player = session.Player;
                var currentX = player.MapPosX;
                var currentY = player.MapPosY;

                var nextCoords = PathFindingMap.GetNextStep(new Coords(currentX, currentY), new Coords(targetX, targetY), 1);

                player.SetOrientation(currentX, currentY, nextCoords.MapPosX, nextCoords.MapPosY);
                player.MapPosX = nextCoords.MapPosX;
                player.MapPosY = nextCoords.MapPosY;

                if (player.MapPosX == targetX && player.MapPosY == targetY)
                {
                    player.WalkDisposable?.Dispose();
                }
            });
        }
    }

}
