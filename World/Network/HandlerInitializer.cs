using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Handlers;
using World.Network.Interfaces;

namespace World.Network
{
    public static class HandlerInitializer
    {
        public static void RegisterAll(IPacketHandler packetHandler)
        {
            var commandRegisters = new List<ICommandRegister>
            {
                new CommandHandler()
            };

            var packetRegisters = new List<IPacketGameHandler>
            {
                new CharacterHandler(),
                new GameStartHandler(),
                new WalkHandler(),
                new RestHandler(),
                new SortOpenHandler(),
                new MoveItemHandler(),
                new WearHandler(),
                new SpecialistHandler(),
                new GuriHandler(),
                new InventorySortHandler(),
                new UseItemHandler(),
                new RemoveItemHandler(),
                new PulseHandler(),
                new GopHandler(),
                new EqInfoHandler(),
                new SayHandler(),
                new NpInfoHandler(),
                new WhisperHandler(),
                new FinsHandler(),
                new BtkHandler(),
                new TalkHandler(),
                new QSetHandler(),
                new ReqInfoHandler(),
                new ExchangeHandler(),
                new PortalHandler(),
                new AttackHandler(),
                new SelectedTargetHandler(),
                new NpcReqHandler(),
                new ShoppingHandler(),
                new NRunHandler(),
                new PickUpHandler(),
            };

            foreach (var cmd in commandRegisters)
            {
                cmd.RegisterCommands(packetHandler);
            }

            foreach (var packet in packetRegisters)
            {
                packet.RegisterPackets(packetHandler);
            }
        }
    }
}