using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class ExchangeHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("req_exc", HandleReqExc);
            handler.RegisterPrefix("#req_exc", HandleReqExcPrefix);
            handler.Register("exc_list", HandleExcList);
        }

        public static async Task HandleReqExc(ClientSession session, string[] parts)
        {
            var type = byte.Parse(parts[2]);
            var entityId = type == 4 || type == 3 ? 0 : int.Parse(parts[3]);

            var player = WorldManager.GetPlayerById(entityId);

            if (type is 4)
            {
                await session.SendPacket("exc_close 0");
                await session.Player.OtherSessionExchange.SendPacket("exc_close 0");
                session.Player.IsExchanging = false;
                session.Player.OtherSessionExchange.Player.IsExchanging = false;
                session.Player.ExchangeItems.Clear();
                session.Player.OtherSessionExchange.Player.ExchangeItems.Clear();
                session.Player.ExchangeConfirmed = false;
                session.Player.OtherSessionExchange.Player.ExchangeConfirmed = true;
            }

            if (type is 3)
            {
                if (session.Player.OtherSessionExchange.Player.ExchangeConfirmed is false && session.Player.ExchangeConfirmed is false)
                {
                    session.Player.ExchangeConfirmed = true;
                    session.Player.OtherSessionExchange.Player.ExchangeConfirmed = true;
                    await session.SendPacket($"infoi {(int)MessageId.EXCHANGE_WAIT_CONFIRM} 0 0 0");
                    return;
                }

                if (session.Player.Gold < session.Player.ExchangeGoldAmount || session.Player.OtherSessionExchange.Player.Gold < session.Player.OtherSessionExchange.Player.ExchangeGoldAmount)
                {
                    await session.SendPacket("exc_close 0");
                    await session.Player.OtherSessionExchange.SendPacket("exc_close 0");
                    session.Player.OtherSessionExchange.Player.IsExchanging = false;
                    session.Player.ExchangeItems.Clear();
                    session.Player.OtherSessionExchange.Player.ExchangeItems.Clear();
                    session.Player.ExchangeConfirmed = false;
                    session.Player.OtherSessionExchange.Player.ExchangeConfirmed = false;
                    return;
                }
                await session.SendPacket("exc_close 1");
                await session.Player.OtherSessionExchange.SendPacket("exc_close 1");

                foreach (var item in session.Player.ExchangeItems)
                {
                    await session.Player.OtherSessionExchange.Player.Inventory.AddItemToInventory(session.Player.OtherSessionExchange.Player.Id, (short)item.Key.ItemId, item.Value, item.Key.Rarity, item.Key.Upgrade);
                    await session.Player.Inventory.DeleteItemFromInventory(item.Key.ItemId, (short)(item.Value <= 0 ? 1 : item.Value));
                }

                foreach (var item in session.Player.OtherSessionExchange.Player.ExchangeItems)
                {
                    await session.Player.Inventory.AddItemToInventory(session.Player.Id, (short)item.Key.ItemId, item.Value, item.Key.Rarity, item.Key.Upgrade);
                    await session.Player.OtherSessionExchange.Player.Inventory.DeleteItemFromInventory(item.Key.ItemId, (short)(item.Value <= 0 ? 1 : item.Value));
                }

                if (session.Player.ExchangeGoldAmount > 0 && session.Player.Gold >= session.Player.ExchangeGoldAmount)
                {
                    await session.Player.OtherSessionExchange.Player.AddGold(session.Player.ExchangeGoldAmount);
                    await session.Player.ReduceGold(session.Player.ExchangeGoldAmount);
                    session.Player.ExchangeGoldAmount = 0;
                }

                if (session.Player.OtherSessionExchange.Player.ExchangeGoldAmount > 0 && session.Player.OtherSessionExchange.Player.Gold >= session.Player.OtherSessionExchange.Player.ExchangeGoldAmount)
                {
                    await session.Player.AddGold(session.Player.OtherSessionExchange.Player.ExchangeGoldAmount);
                    await session.Player.OtherSessionExchange.Player.ReduceGold(session.Player.OtherSessionExchange.Player.ExchangeGoldAmount);
                    session.Player.OtherSessionExchange.Player.ExchangeGoldAmount = 0;
                }
                session.Player.OtherSessionExchange.Player.IsExchanging = false;
                session.Player.ExchangeItems.Clear();
                session.Player.OtherSessionExchange.Player.ExchangeItems.Clear();
                session.Player.ExchangeConfirmed = false;
                session.Player.OtherSessionExchange.Player.ExchangeConfirmed = false;
            }

            if (type != 4 && type != 3)
            {
                session.Player.OtherSessionExchange = player;
                player.Player.OtherSessionExchange = session;
                await session.SendPacket($"infoi2 170 1 {player.Player.Name}");
                await player.SendPacket($"dlgi2 #req_exc^2^{session.Player.Id} #req_exc^5^{session.Player.Id} {(int)MessageId.EXCHANGE_ACCEPT_REQUEST_ASK} 2 {session.Player.Level} {session.Player.Name}");
            }
        }

        public static async Task HandleReqExcPrefix(ClientSession session, string[] parts)
        {
            // #req_exc^2^294532
            var splitter = parts[1].Split('^');
            var type = byte.Parse(splitter[1]);
            var entityId = int.Parse(splitter[2]);

            var player = WorldManager.GetPlayerById(entityId);
            await session.SendPacket($"exc_list 1 {entityId} -1 -1");
            await player.SendPacket($"exc_list 1 {session.Player.Id} -1 -1");

            var tax = 280000;
            await session.Player.SendPacket($"gbex 0 {session.Player.Gold} 11 {tax}");
            await player.SendPacket($"gbex 0 {player.Player.Gold} 11 {tax}");
            session.Player.IsExchanging = true;
            session.Player.OtherSessionExchange.Player.IsExchanging = true;
        }

        public static async Task HandleExcList(ClientSession session, string[] parts)
        {
            // exc_list 0 0 invType fromSlot amount
            // exc_list 0 0 2 46 99 2 47 1
            var gold = 0;
            var goldBank = 0;
            var invType = 0;
            var amount = 0;

            if (parts.Length > 3)
            {
                gold = int.Parse(parts[2]);
            }
            if (parts.Length > 4)
            {
                goldBank = int.Parse(parts[3]);
            }

            var itemStartIndex = 3;
            int itemCount = (parts.Length - itemStartIndex) / 3;

            var maxSlots = 10;
            var availableSlots = 10;

            StringBuilder packet = new StringBuilder();

            var maxGoldInv = gold > session.Player.Gold ? session.Player.Gold : gold;
            packet.Append($"exc_list 1 {session.Player.Id} {maxGoldInv} {goldBank}");
            for (short i = 0; i < itemCount; i++)
            {
                int index = itemStartIndex + i * 3;
                var fromSlot = int.Parse(parts[index + 2]);
                var fromInvType = (InventoryType)byte.Parse(parts[index + 1]);
                var fromAmount = short.Parse(parts[index + 3]);
                var getItem = await session.Player.Inventory.GetItemFromSlot(fromSlot, fromInvType);
                var item = WorldManager.GetItem(getItem.ItemId);
                var isNotWearItem = item.Type == InventoryType.MAIN || item.Type == InventoryType.ETC;
                packet.Append($" {i}.{(isNotWearItem ? 1 : 0)}.{getItem.ItemId}.{(isNotWearItem ? fromAmount : getItem.Rarity)}.{(isNotWearItem ? 0 : getItem.Upgrade)}.0.0");
                session.Player.ExchangeItems.Add(getItem, fromAmount);
            }
            session.Player.ExchangeGoldAmount = maxGoldInv;
            await session.Player.OtherSessionExchange.SendPacket(packet.ToString());
        }
    }

    // dlgi2 #req_exc^2^341863 #req_exc^5^341863 169 2 19 Nefarian!
    // exc_list 1 294532 otherPlayerGold otherPlayerGoldBank slot.enableAmount.VNum.amountIfNotWear.0.0.0

    // exc_list 0 0 1 2 99
}