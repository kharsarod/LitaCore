using Database.Helper;
using Database.Social;
using Enum.Main.ChatEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class FinsHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("fins", HandleFins);
            handler.RegisterPrefix("#fins", HandleFinsPrefix);
        }

        public static async Task HandleFins(ClientSession session, string[] parts)
        {
            byte type = byte.Parse(parts[2]);
            int toCharacterId = int.Parse(parts[3]);

            var player = WorldManager.GetPlayerById(toCharacterId);
            if (player is null)
            {
                Log.Information($"Player {toCharacterId} not found.");
                return;
            }
            await player.SendPacket($"dlgi2 #fins^-1^{session.Player.Id} #fins^-99^{session.Player.Id} {(int)MessageId.FRIENDS_ASK_REQUEST} 1 {session.Player.Name}");
        }

        public static async Task HandleFinsPrefix(ClientSession session, string[] parts)
        {
            // #fins^-1^1
            var splitter = parts[1].Split('^');
            // var response = byte.Parse(splitter[1]);
            var toCharacterId = int.Parse(splitter[2]);

            var getPlayer = WorldManager.GetPlayerById(toCharacterId);
            if (getPlayer is null)
            {
                Log.Information($"Player {toCharacterId} not found.");
                return;
            }

            Friend friend = new Friend
            {
                CharacterId = (short)session.Player.Id,
                FriendCharacterId = (short)toCharacterId,
                Married = false,
                FriendName = getPlayer.Player.Name
            };

            Friend friendOtherPlayer = new Friend
            {
                CharacterId = (short)toCharacterId,
                FriendCharacterId = (short)session.Player.Id,
                Married = false,
                FriendName = session.Player.Name
            };

            var alreadyExists = await CharacterDbHelper.VerifyIfFriendAlreadyAdded(friendOtherPlayer);
            if (alreadyExists is not null)
            {
                await session.SendPacket($"infoi {(int)MessageId.ALREADY_ADDED_FRIEND} 0 0 0");
                await getPlayer.SendPacket($"infoi {(int)MessageId.ALREADY_ADDED_FRIEND} 0 0 0");
                return;
            }

            await CharacterDbHelper.InsertFriendAsync(friend);
            await CharacterDbHelper.InsertFriendAsync(friendOtherPlayer);

            var friends = await CharacterDbHelper.LoadAllFriendsAsync();
            StringBuilder packet = new StringBuilder();
            StringBuilder packetOtherPlayer = new StringBuilder();
            packet.Append("finit");
            packetOtherPlayer.Append("finit");

            foreach (var playerFriend in friends.Where(x => x.CharacterId == session.Player.Id))
            {
                packet.Append($" {playerFriend.FriendCharacterId}|{(playerFriend.Married ? 5 : 0)}|{(WorldManager.GetPlayerById(playerFriend.FriendCharacterId) is null ? 0 : 1)}|{playerFriend.FriendName} ");
            }

            foreach (var playerFriend in friends.Where(x => x.FriendCharacterId == getPlayer.Player.Id))
            {
                packetOtherPlayer.Append($" {playerFriend.CharacterId}|{(playerFriend.Married ? 5 : 0)}|{(WorldManager.GetPlayerById(playerFriend.CharacterId) is null ? 0 : 1)}|{playerFriend.FriendName} ");
            }

            await getPlayer.Player.GetAllFriends();
            await session.Player.GetAllFriends();
        }
    }
}