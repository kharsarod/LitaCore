using Configuration;
using Database.Helper;
using Database.MapEntity;
using Database.Player;
using Enum.Main.CharacterEnum;
using Enum.Main.ChatEnum;
using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using Enum.Main.SpecialistEnum;
using GameWorld;
using NosCryptLib.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;
using World.Utils;
using static System.Collections.Specialized.BitVector32;

namespace World.Network.Handlers
{
    public class CommandHandler : ICommandRegister
    {
        private static IPacketHandler _packetHandler;

        public void RegisterCommands(IPacketHandler handler)
        {
            _packetHandler = handler;
            handler.Register("$transform", Morph);
            handler.Register("$tp", Teleport);
            handler.Register("$speed", Speed);
            handler.Register("$mlvl", ModLevel);
            handler.Register("$mjlvl", ModJobLevel);
            handler.Register("$pinfo", PlayerInfo);
            handler.Register("$getmapinfo", GetMapInfo);
            handler.Register("$getpos", GetPosition);
            handler.Register("$additem", HandleAddItem);
            handler.Register("$playmusic", HandlePlayMusic);
            handler.Register("$setmapmusic", HandleSetMapMusic);
            handler.Register("$setclass", HandleSetClass);
            handler.Register("$setgold", HandleSetGold);
            handler.Register("$packet", HandlePacket);
            handler.Register("$looitem", HandleSearchItem);
            handler.Register("$commands", HandleGetCommands);
            handler.Register("$setrep", HandleSetReputation);
            handler.Register("$setdignity", HandleSetDignity);
            handler.Register("$setspap", HandleSetSpecialistAdditionPoints);
            handler.Register("$reloaditems", HandleReloadItems);
            handler.Register("$reloadmaps", HandleReloadMaps);
            handler.Register("$setzoom", HandleZoom);
            handler.Register("$createaccount", HandleCreateAccount);
            handler.Register("$tpto", HandleTeleportToThePlayer);
            handler.Register("$gmsummon", HandleGmSummon);
            handler.Register("$eff", HandleEffect);
            handler.Register("$allonlineplayers", HandleAllOnlinePlayers);
            handler.Register("$kick", HandleKick);
            handler.Register("$ban", HandleBan);
            handler.Register("$unban", HandleUnban);
            handler.Register("$resetpos", HandleResetPosition);
            handler.Register("$unstuck", HandleUnstuck);
            handler.Register("$shout", HandleShout);
            handler.Register("$sumportal", HandleSummonPortal);
            handler.Register("$reloadportals", HandleReloadPortals);
            handler.Register("$resetposp", HandleResetPositionPlayers);
            handler.Register("$reloadnpcs", HandleReloadNpcs);
            handler.Register("$reloadshops", HandleReloadShops);
            handler.Register("$reloadshopitems", HandleReloadShopItems);
            handler.Register("$setsex", HandleSetSex);
            handler.Register("$playanim", HandlePlayAnimation);
            handler.Register("$clearinv", HandleClearInventory);
            handler.Register("$hppercent", HandleGetHpPercent);
            handler.Register("$reloadnpcshop", HandleReloadNpcShop);
            handler.Register("$cd", HandleCooldown);
            handler.Register("$setflvl", HandleSetFairyLevel);
            handler.Register("$getspeed", HandleGetSpeed);
            handler.Register("$learnskill", HandleLearnSkill);
            handler.Register("$buff", HandleBuff);
            handler.Register("$reloadconfig", HandleReloadConfig);
            handler.Register("$getmapitems", HandleGetMapItems);
            handler.Register("$rates", HandleRatesInfo);
        }

        public static async Task HandleRatesInfo(ClientSession session, string[] parts)
        {
            var info = $"[Server Rates]\nExp: x{ConfigManager.WorldServerConfig.Rates.ExpRate}\n" +
                $"GoldAmount: x{ConfigManager.WorldServerConfig.Rates.GoldRate}\n" +
                $"Drop: x{ConfigManager.WorldServerConfig.Rates.DropRate}\n" +
                $"GoldDrop: x{ConfigManager.WorldServerConfig.Rates.GoldRate}\n" + // cambiar este
                $"JobExp: x{ConfigManager.WorldServerConfig.Rates.JobExpRate}";

            await session.Player.ChatSay(info, ChatColor.Green);
        }

        public static async Task HandleGetMapItems(ClientSession session, string[] parts)
        {
            await session.Player.ChatSay($"En este mapa hay {session.Player.CurrentMap.Items.Count} objetos en el suelo.", ChatColor.Green);
        }

        public static async Task HandleReloadConfig(ClientSession session, string[] parts)
        {
            ConfigManager.LoadWorldConfiguration();
            await session.Player.ChatSay("Configuración recargada.", ChatColor.Green);
        }

        public static async Task HandleBuff(ClientSession session, string[] parts)
        {
            if (parts.Length < 3)
            {
                await session.Player.ChatSay("Por favor, proporciona el ID de la habilidad que deseas usar.", ChatColor.Red);
                return;
            }
            var buffId = short.Parse(parts[2]);
            var buff = WorldManager.Getbuff(buffId);
            
            await session.Player.AddBuff(buff.BuffId);
        }

        public static async Task HandleLearnSkill(ClientSession session, string[] parts)
        {
            if (parts.Length < 3)
            {
                await session.Player.ChatSay("Por favor, proporciona el ID de la habilidad que deseas aprender.", ChatColor.Red);
                return;
            }
            var skillId = short.Parse(parts[2]);
            var skill = session.Player.Skills.FirstOrDefault(s => s.VNum == skillId);
            if (skill != null)
            {
                await session.Player.ChatSay($"Ya conoces la habilidad con ID {skillId}.", ChatColor.Yellow);
                return;
            }
            skill = new CharacterSkill { VNum = skillId };
            await session.Player.LearnSkill(skill.VNum);
            await session.Player.ChatSay($"Habilidad con ID {skillId} aprendida.", ChatColor.Green);
        }

        public static async Task HandleGetSpeed(ClientSession session, string[] parts)
        {
            await session.Player.ChatSay($"Tu velocidad es de {session.Player.Speed}.", ChatColor.Green);
        }

        public static async Task HandleSetFairyLevel(ClientSession session, string[] parts)
        {
            var level = int.Parse(parts[2]);
            var fairy = session.Player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.FAIRY);
            if (fairy == null) return;
            fairy.FairyLevel = (byte)level;

            await CharacterDbHelper.UpdateItemAsync(fairy);

            await session.Player.ChatSay($"El nivel del hada ha sido cambiado a {fairy.FairyLevel}%.", ChatColor.Green);
        }

        public static async Task HandleCooldown(ClientSession session, string[] parts)
        {
            var useSp = session.Player.UsingSpecialist ? true : false;

            if (useSp)
            {
                foreach (var skill in (session.Player.UsingSpecialist ? session.Player.SpecialistSkills : session.Player.Skills))
                {
                    var getSkill = WorldManager.GetSkill(skill.VNum);
                    await session.SendPacket($"sr {getSkill.CastId}");
                }
            }

            await session.Player.ChatSay($"Cooldowns reseteados.", ChatColor.Green);
        }

        public static async Task HandleReloadNpcShop(ClientSession session, string[] parts)
        {
            session.Player.CurrentMap.NpcEntities.Clear();
            await WorldManager.LoadNpcs();
            await WorldManager.LoadShops();
            await WorldManager.LoadShopItems();
            await WorldManager.LoadShopTranslations();

            await session.Player.ChangeMap(session.Player.CurrentMap, session.Player.MapPosX, session.Player.MapPosY);
            await session.Player.ChatSay($"Tiendas, npc e items de tiendas, recargadas.", ChatColor.Green);
        }

        public static async Task HandleReloadNpcs(ClientSession session, string[] parts)
        {
            session.Player.CurrentMap.NpcEntities.Clear();
            await WorldManager.LoadNpcs();
            await session.Player.ChatSay($"NPCs recargados.", ChatColor.Green);
        }

        public static async Task HandleGetHpPercent(ClientSession session, string[] parts)
        {
            var percent = await session.Player.Stats.HealthPercent();
            await session.Player.ChatSay($"Vida: {percent}%", ChatColor.Green);

            await session.Player.HealHp(50000);
        }

        public static async Task HandleClearInventory(ClientSession session, string[] parts)
        {
            var invType = (InventoryType)byte.Parse(parts[2]);
            _ = session.Player.Inventory.ClearInventory(invType);
            await session.Player.ChatSay($"Inventario {invType} limpiado.", ChatColor.Green);
        }

        public static async Task HandlePlayAnimation(ClientSession session, string[] parts)
        {
            var anim = byte.Parse(parts[2]);
            string packet = $"su 1 {session.Player.Id} 1 {session.Player.Id} 932 10 {anim} -1 {session.Player.MapPosX} {session.Player.MapPosY} 1 5000 0 -1 0";

            await session.Player.CurrentMap.Broadcast(packet);
            await session.Player.ChatSay($"Animación reproducida: {anim}.", ChatColor.Green);
        }

        public static async Task HandleSetSex(ClientSession session, string[] parts)
        {
            session.Player.Gender = (Gender)int.Parse(parts[2]);
            await session.Player.UpdateCharacter();
            await session.Player.ChangeMap(session.Player.CurrentMap, session.Player.MapPosX, session.Player.MapPosY);
            await session.Player.ChatSay($"Sexo cambiado.", ChatColor.Green);
        }

        public static async Task HandleReloadShopItems(ClientSession session, string[] parts)
        {
            await WorldManager.LoadShopItems();
            await session.Player.ChatSay($"Items de tiendas recargadas.", ChatColor.Green);
        }

        public static async Task HandleReloadShops(ClientSession session, string[] parts)
        {
            await WorldManager.LoadShops();
            await WorldManager.LoadShopTranslations();
            await session.Player.ChatSay($"Tiendas recargadas.", ChatColor.Green);
        }

        public static async Task Morph(ClientSession session, string[] parts)
        {
            string content = string.Join(' ', parts.Skip(2));
            await session.Player.CurrentMap.Broadcast($"c_mode 1 {session.Player.Id} {content}");
            await session.Player.CurrentMap.Broadcast($"eff 1 {session.Player.Id} 196");
            session.Player.UsingSpecialist = true;
            session.Player.Morph = int.Parse(parts[2]);
            session.Player.SpUpgrade = byte.Parse(parts[3]);
            session.Player.SpWings = (SpWings)byte.Parse(parts[4]);
        }

        public static async Task HandleSummonPortal(ClientSession session, string[] parts)
        {
            var fromMap = short.Parse(parts[2]);
            var fromX = short.Parse(parts[3]);
            var fromY = short.Parse(parts[4]);
            var toMap = short.Parse(parts[5]);
            var toX = short.Parse(parts[6]);
            var toY = short.Parse(parts[7]);

            await WorldManager.AddPortal(toMap, toX, toY, fromMap, fromX, fromY);

            await session.Player.ChatSay($"Portal agregado, usa $reloadportals y luego $resetpos o $resetposp para los jugadores en el mapa.", ChatColor.Green);
        }

        public static async Task HandleResetPositionPlayers(ClientSession session, string[] parts)
        {
            foreach (var player in session.Player.CurrentMap.Players)
            {
                await player.ChangeMap(player.CurrentMap, player.MapPosX, player.MapPosY);
            }
        }

        public static async Task HandleShout(ClientSession session, string[] parts)
        {
            string content = string.Join(' ', parts.Skip(2));
            await WorldManager.Shout(content);
        }

        public static async Task HandleUnstuck(ClientSession session, string[] parts)
        {
            await session.SendPacket($"cancel 0 {session.Player.Id} -1");
        }

        public static async Task HandleResetPosition(ClientSession session, string[] parts)
        {
            await session.Player.ChangeMap(session.Player.CurrentMap, session.Player.MapPosX, session.Player.MapPosY);
        }

        public static async Task HandleUnban(ClientSession session, string[] parts)
        {
            var accountName = parts[2];
            await AuthDbHelper.UnbanAccountByNameAsync(accountName);
            await session.Player.ChatSay($"La cuenta {accountName} ha sido desbaneado.", ChatColor.Green);
        }

        public async Task HandleKick(ClientSession session, string[] parts)
        {
            var playerName = parts[2];

            var player = WorldManager.GetPlayerByName(playerName);

            if (player is null)
            {
                await session.Player.ChatSay($"El jugador {playerName} no está conectado.", ChatColor.Red);
                return;
            }

            await player.Disconnect();

            await session.Player.ChatSay($"El jugador {playerName} ha sido desconectado.", ChatColor.Green);
        }

        public async Task HandleBan(ClientSession session, string[] parts)
        {
            var playerName = parts[2];

            var player = WorldManager.GetPlayerByName(playerName);
            await player.Ban();
            await session.Player.ChatSay($"El jugador {playerName} ha sido baneado.", ChatColor.Green);
        }

        public static async Task HandleAllOnlinePlayers(ClientSession session, string[] parts)
        {
            string message = $"Currently online players in this server: {WorldManager.GetAllOnlinePlayers()}.";
            await session.Player.ChatSay(message, ChatColor.Green);
        }

        public static async Task HandleEffect(ClientSession session, string[] parts)
        {
            var effId = int.Parse(parts[2]);

            await session.SendPacket(session.Player.Packets.GenerateEffectPacket(effId));
        }

        public static async Task HandleTeleportToThePlayer(ClientSession session, string[] parts)
        {
            var playerName = parts[2];

            var player = WorldManager.GetPlayerByName(playerName);

            if (player is not null && player.ChannelId == session.ChannelId)
            {
                short id = (player.Player.CurrentMap.Template.Id);
                short x = (short)(player.Player.MapPosX + 2);
                short y = (short)(player.Player.MapPosY + 2);
                var instance = await WorldManager.GetInstance(id, session.ChannelId);
                await session.Player.ChangeMap(instance, x, y);
            }
            else if (player is null)
            {
                await session.Player.ChatSay($"El jugador {playerName} no está conectado.", ChatColor.Red);
            }
        }

        public static async Task HandleGmSummon(ClientSession session, string[] parts)
        {
            var playerName = parts[2];

            var player = WorldManager.GetPlayerByName(playerName);

            if (player is not null && player.ChannelId == session.ChannelId)
            {
                short id = (session.Player.CurrentMap.Template.Id);
                short x = (short)(session.Player.MapPosX + 2);
                short y = (short)(session.Player.MapPosY + 2);
                var instance = await WorldManager.GetInstance(id, session.ChannelId);
                await player.Player.ChangeMap(instance, x, y);
            }
            else if (player is null)
            {
                await session.Player.ChatSay($"El jugador {playerName} no está conectado.", ChatColor.Red);
            }
        }

        public static async Task HandleCreateAccount(ClientSession session, string[] parts)
        {
            var username = parts[2];
            var password = parts[3];
            byte rank = byte.Parse(parts[4]);
            Account account = new Account { Username = username, Password = Cryptography.ToSha512(password), Rank = rank, IpAddress = "0.0.0.0" };
            await AuthDbHelper.InsertAccountAsync(account);

            await session.Player.ChatSay($"La cuenta {account.Username} ha sido creada.", ChatColor.Green);
        }

        public static async Task HandleSetSpecialistAdditionPoints(ClientSession session, string[] parts)
        {
            if (string.IsNullOrEmpty(parts[2]) || string.IsNullOrEmpty(parts[3]))
            {
                await session.Player.ChatSay("Los puntos de adición o los puntos de SP, están vacíos.", ChatColor.Red);
                return;
            }

            var addPoint = int.Parse(parts[2]);
            var points = int.Parse(parts[3]);

            await session.Player.SetSpecialistAdditionPoints(addPoint, points);

            await session.Player.ChatSay("Has cambiado tus puntos de adición y SP.", ChatColor.Green);
        }

        public static async Task HandleSetReputation(ClientSession session, string[] parts)
        {
            await session.Player.SetReputation(int.Parse(parts[2]));
        }

        public static async Task HandleSetDignity(ClientSession session, string[] parts)
        {
            await session.Player.SetDignity(int.Parse(parts[2]));
        }

        public static async Task Teleport(ClientSession session, string[] parts)
        {
            var mapId = short.Parse(parts[2]);
            var instance = await WorldManager.GetInstance(mapId, session.ChannelId);
            if (instance is null)
            {
                await session.Player.ChatSay("No se encontró ninguna instancia.", ChatColor.Red);
                return;
            }
            if (parts.Length < 4)
            {
                var coord = instance.GetRandomCoord();

                await session.Player.ChangeMap(instance, coord.MapPosX, coord.MapPosY);
            }
            else
            {
                var x = short.Parse(parts[3]);
                var y = short.Parse(parts[4]);
                await session.Player.ChangeMap(instance, x, y);
            }
        }

        public static async Task Speed(ClientSession session, string[] parts)
        {
            await session.Player.SetSpeed(byte.Parse(parts[2]));
            if (!session.Player.HasCustomSpeed)
            {
                session.Player.HasCustomSpeed = true;
            }
            await session.Player.ChatSay("Has cambiado tu velocidad de movimiento.", ChatColor.Green);
        }

        public static async Task ModLevel(ClientSession session, string[] parts)
        {
            var level = byte.Parse(parts[2]);
            await session.Player.SetLevel(level);
            await CharacterDbHelper.UpdateAsync(session.Player);
        }

        public static async Task ModJobLevel(ClientSession session, string[] parts)
        {
            var level = byte.Parse(parts[2]);
            await session.Player.SetJobLevel(level);
            await CharacterDbHelper.UpdateAsync(session.Player);
        }

        public static async Task PlayerInfo(ClientSession session, string[] parts)
        {
            await session.Player.ChatSay($"Username: {session.Account.Username}, rank: {session.Account.Rank}, characterId: {session.Player.Id}, sessionId: {session.SessionId}", ChatColor.Green);
        }

        public static async Task GetPosition(ClientSession session, string[] parts)
        {
            await session.SendPacket($"pp {session.Player.MapId} {session.Player.CurrentMap.Template.Id} {session.Player.MapPosX} {session.Player.MapPosY}");

            await session.SendPacket(session.Player.Packets.GenerateLev());
        }

        public static async Task GetMapInfo(ClientSession session, string[] parts)
        {
            var instance = await WorldManager.GetInstance(session.Player.MapId, session.ChannelId);

            await session.Player.ChatSay($"name: {instance.Template.Name}, id: {instance.Template.Id}," +
                $" expRate: x{instance.Template.ExpRate}, goldRate: x{instance.Template.GoldRate}, dropRate: x{instance.Template.DropRate}," +
                $" pvpAllowed: {instance.Template.IsPvpAllowed}, shopAllowed: {instance.Template.IsShopAllowed}, players: {instance.Players.Count()}", ChatColor.Green);
        }

        public static async Task HandleAddItem(ClientSession session, string[] parts)
        {
            var itemId = short.Parse(parts[2]);
            var amount = parts.Length > 3 ? short.Parse(parts[3]) : (short)1;
            var rare = parts.Length > 4 ? byte.Parse(parts[4]) : (byte)0;
            var upgrade = parts.Length > 5 ? byte.Parse(parts[5]) : (byte)0;

            await session.Player.AddItem(itemId, amount, rare, upgrade);
        }

        public static async Task HandlePlayMusic(ClientSession session, string[] parts)
        {
            var musicId = short.Parse(parts[2]);
            await session.SendPacket($"bgm {musicId}");
            await session.Player.ChatSay("Música establecida.", ChatColor.Green);
        }

        public static async Task HandleSetMapMusic(ClientSession session, string[] parts)
        {
            var musicId = short.Parse(parts[2]);
            var map = await WorldManager.GetMap(session.Player.CurrentMap.Template.Id);
            session.Player.CurrentMap.Template.Bgm = musicId;
            map.Bgm = musicId;
            await session.Player.CurrentMap.Broadcast($"bgm {map.Bgm}");
            var latin1 = Encoding.GetEncoding("windows-1252"); // o "iso-8859-1", según lo que necesites

            await session.Player.ChatSay("Música establecida.", ChatColor.Green);

            await WorldDbHelper.UpdateMapAsync(map);
        }

        public static async Task HandleSetClass(ClientSession session, string[] parts)
        {
            var classId = (ClassId)byte.Parse(parts[2]);
            await session.Player.ChangeProfession(classId);
        }

        public static async Task HandleSetGold(ClientSession session, string[] parts)
        {
            var gold = long.Parse(parts[2]);
            await session.Player.SetGold(gold);

            await session.Player.ChatSay("Tu oro ha sido modificado.", ChatColor.Green);
        }

        public static async Task HandlePacket(ClientSession session, string[] parts)
        {
            var content = string.Join(' ', parts.Skip(2));
            await session.SendPacket(content);
        }

        public static async Task HandleSearchItem(ClientSession session, string[] parts)
        {
            await session.SendPacket($"gmlist 0 {parts[2]}");
        }

        public static async Task HandleGetCommands(ClientSession session, string[] parts)
        {
            foreach (var command in _packetHandler.GetRegisteredCommands())
            {
                await session.Player.ChatSay($"- {command}", ChatColor.Yellow, true);
            }
        }

        public static async Task HandleReloadItems(ClientSession session, string[] parts)
        {
            await WorldManager.LoadItems();
            await session.Player.ChatSay("Los items han sido recargados.", ChatColor.Green);
        }

        public static async Task HandleReloadPortals(ClientSession session, string[] parts)
        {
            await WorldManager.LoadPortals();
            await session.Player.ChatSay("Los portales han sido recargados.", ChatColor.Green);
        }

        public static async Task HandleReloadMaps(ClientSession session, string[] parts)
        {
            await WorldManager.LoadMaps();
            await session.Player.ChatSay("Las instancias han sido recargadas.", ChatColor.Green);
        }

        public static async Task HandleZoom(ClientSession session, string[] parts)
        {
            var zoom = byte.Parse(parts[2]);
            await session.SendPacket($"guri 15 {zoom} {session.Player.Id}");
            await session.Player.ChatSay("Zoom establecido.", ChatColor.Green);
        }
    }
}