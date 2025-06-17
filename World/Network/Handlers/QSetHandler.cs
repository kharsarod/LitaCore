using Database.Helper;
using Database.Player;
using Enum.Main.ChatEnum;
using Enum.Main.ItemEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;
using static System.Collections.Specialized.BitVector32;

namespace World.Network.Handlers
{
    public class QSetHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("qset", HandleQSet);
        }

        public static async Task HandleQSet(ClientSession session, string[] parts)
        {
            /* var type = byte.Parse(parts[2]);
             var actionBar2 = byte.Parse(parts[3]); // tab
             var toSlot = short.Parse(parts[4]); // quicklist slot
             var fromInvType = byte.Parse(parts[5]); // destType
             var fromSlot = short.Parse(parts[6]); //  destinationSlotOrVnum*/

            byte type = byte.Parse(parts[2]);
            short q1 = short.Parse(parts[3]);
            short q2 = short.Parse(parts[4]);
            short data1 = 0;
            short data2 = 0;

            if (parts.Length > 6 && parts[5].Length > 0 && parts[6].Length > 0)
            {
                data1 = short.Parse(parts[5]);
                data2 = short.Parse(parts[6]);
            }

            switch (type)
            {
                case 0:
                case 1:
                    session.Player.ActionBars.RemoveAll(n =>
                    n.Q1 == q1 && n.Q2 == q2);

                    session.Player.ActionBars.Add(new ActionBar
                    {
                        CharacterId = session.Player.Id,
                        Type = type,
                        Q1 = q1,
                        Q2 = q2,
                        Slot = data1,
                        Pos = data2,
                        Morph = session.Player.UsingSpecialist ? session.Player.Morph : 0
                    });
                    await session.SendPacket($"qset {q1} {q2} {type}.{data1}.{data2}.0");

                    // if actionbar already exists don't update it, simply add another one with the same data but ignoring the same data of Pos variable.

                    var alreadyExists = session.Player.ActionBars.SingleOrDefault(n =>
                        n.Q1 == q1 && n.Q2 == q2 && n.Pos == data2 && n.Morph == session.Player.Morph);

                    try
                    {
                        if (alreadyExists is null)
                        {
                            await CharacterDbHelper.UpdateActionBarListAsync(session.Player.ActionBars, session.Player.Id);
                            return;
                        }
                        else
                        {
                            await CharacterDbHelper.InsertActionBarListAsync(session.Player.ActionBars);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    break;

                case 2:
                    var qlFrom = session.Player.ActionBars.SingleOrDefault(n =>
                        n.Q1 == data1 && n.Q2 == data2);

                    if (qlFrom is not null)
                    {
                        var qlTo = session.Player.ActionBars.SingleOrDefault(n =>
                            n.Q1 == q1 && n.Q2 == q2);

                        qlFrom.Q1 = q1;
                        qlFrom.Q2 = q2;
                        if (qlTo == null)
                        {
                            await session.SendPacket(
                                $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            await session.SendPacket($"qset {data1} {data2} 7.7.-1.0");
                        }
                        else
                        {
                            await session.SendPacket(
                                $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");
                            qlTo.Q1 = data1;
                            qlTo.Q2 = data2;
                            await session.SendPacket($"qset {qlTo.Q1} {qlTo.Q2} {qlTo.Type}.{qlTo.Slot}.{qlTo.Pos}.0");
                        }
                    }
                    break;

                case 3:
                    session.Player.ActionBars.RemoveAll(x => x.Q1 == q1 && x.Q2 == q2);
                    await session.SendPacket($"qset {q1} {q2} 7.7.-1.0");
                    break;
            }
        }
    }
}