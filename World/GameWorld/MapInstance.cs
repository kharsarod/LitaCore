using Database.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Network;
using World.Utils;

namespace World.GameWorld
{
    public class MapInstance
    {
        public short Id => Template.Id;
        public Guid InstanceId { get; } = new Guid();
        public WorldMap Template { get; }
        public List<Player> Players { get; } = new List<Player>();

        public MapInstance(WorldMap template)
        {
            Template = template;
        }

        public void AddPlayer(Player player)
        {
            if (!Players.Contains(player)) Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            if (Players.Contains(player)) Players.Remove(player);
        }

        public async Task Broadcast(string packet, ClientSession except = null)
        {
            foreach(var player in Players)
            {
                if (player.Session != except)
                {
                    await player.Session.SendPacket(packet);
                }
            }
        }

        public Coords GetRandomCoord()
        {
            return Template.GetRandomCoord();
        }
    }
}
