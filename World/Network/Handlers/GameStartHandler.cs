using Configuration;
using Database.Helper;
using Database.Player;
using Enum.Main.CharacterEnum;
using Enum.Main.ChatEnum;
using GameWorld;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Extensions;
using World.Gameplay.Script;
using World.Network.Interfaces;
using static System.Collections.Specialized.BitVector32;

namespace World.Network.Handlers
{
    public class GameStartHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("game_start", HandleGameStart);
        }

        public static async Task HandleGameStart(ClientSession Session, string[] parts) // GameStart
        {
            var instance = await WorldManager.GetInstance(Session.Player.MapId, Session.ChannelId);
            Console.WriteLine(Session.Player.MapId);
            // Registración de scripts
            await ScriptLoader.RegisterAllScripts(Session.Player);

            Log.Information("Player {Name} has entered in the game.", Session.Player.Name);
            await Session.SendPacket("lbs 0");
            await Session.SendPacket("glist 0 0");
            await Session.SendPacket("rsfi 1 1 0 9 0 9");
            await Session.SendPacket(Session.Player.Packets.GeneratePInfo());
            await Session.SendPacket("mall 50");
            await Session.SendPacket("c_close 1");
            await Session.SendPacket("c_info_reset");
            await Session.Player.LoadInventory();
            await Session.SendPacket(Session.Player.Packets.GenerateLev());
            await Session.Player.Packets.GenerateScPacket();
            // ski packet here
            Session.Player.Speed = (byte)(Session.Player.Class != ClassId.Mage ? 12 : 11);
            await Session.Player.ChangeMap(instance, Session.Player.MapPosX, Session.Player.MapPosY);
            await Session.Player.Packets.GenerateRepAndDignityPacket();
            await Session.SendPacket(Session.Player.Packets.GenerateScale());
            await Session.Player.Packets.GenerateEquipmentPacket();
            await Session.Player.Packets.GeneratePairyPacket();
            await Session.Player.Packets.GenerateRsfiPacket();
            await Session.SendPacket(Session.Player.Packets.GenerateRage());
            await Session.SendPacket(Session.Player.Packets.GenerateFood());
            await Session.Player.Packets.GenerateSpAdditionPointsPacket();
            await Session.Player.GetAllFriends();
            await Session.Player.Packets.GenerateScrPacket();
            await Session.Player.Packets.GenerateScrollingMessagesPacket();
            await Session.SendPacket(Session.Player.Packets.GenerateGidx());
            await Session.Player.UpdateItemsExpirationTime();
            await Session.SendPacket("zzim");
            await Session.SendPacket(Session.Player.Packets.GenerateClientAuthPacket());
            await WorldManager.UpdateFriends(Session);
            Session.Player.Stats.Initialize();
            await Session.Player.LoadSkills();
            await Session.Player.GetActionBarList();
            await Session.SendPacket(Session.Player.Packets.GenerateStat());

            foreach (var script in ScriptLoader.GetScriptsForPlayer(Session.Player))
            {
                await script.OnLogin();
            }

            var firstLogin = await WorldManager.GetWorldLogByCharacterName(Session.Player.Name);
            if (firstLogin is not null && firstLogin.Type == "NewCharacter")
            {
                await WorldDbHelper.RemoveWorldLogBySourceAndType(Session.Player.Name, "NewCharacter");
                
                if (ConfigManager.WorldServerConfig.Features.PlayVideoOnFirstLogin)
                {
                    await Session.SendPacket("scene 40 2");
                }
            }

            Session.IsInGame = true;

            _ = Session.Player.StartStatsFunction();
            _ = Session.Player.StartItemExpirationTimer();
            _ = Session.Player.SaveActionBars();
            _ = Session.Player.CharacterFeatures();
        }
    }
}