using Configuration;
using Configuration.Config;
using Database.Context;
using Database.Helper;
using Database.Item;
using Database.MapEntity;
using Database.Player;
using Database.Server;
using Database.ShopEntity;
using Database.World;
using Enum.Main.BCardEnum;
using Enum.Main.ChatEnum;
using Enum.Main.MessageEnum;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.BCards.Handler;
using World.Gameplay.Script;
using World.GameWorld;
using World.GameWorld.Objects;
using World.Network;
using World.Utils;

namespace GameWorld
{
    public static class WorldManager
    {
        // Diccionario que asocia un canal con instancias de mapa
        private static readonly Dictionary<short, WorldMap> Maps = new();

        private static readonly Dictionary<int, Dictionary<short, MapInstance>> _channelInstances = new();
        private static readonly List<MapInstance> _instances = new();

        public static readonly List<Item> Items = new();
        public static List<Portal> Portals = new();
        public static List<Monster> Monsters = new();
        public static List<Npc> Npcs = new();
        public static List<Shop> Shops = new();
        public static List<ShopTranslations> ShopTranslations = new();
        public static List<ShopItem> ShopItems = new();
        public static List<ChannelInfo> Channels = new();
        public static List<DropData> Drops = new();
        public static List<NpcMonster> NpcMonsters = new();
        public static List<SkillData> Skills = new();
        public static List<BuffData> Buffs = new();

        public static short ServerId { get; set; }

        [DllImport("user32.dll")]
        public static extern bool MessageBeep(uint uType);

        public static async Task LoadMaps()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Maps != null) Maps.Clear();
            var maps = await WorldDbHelper.LoadAllMapsAsync();
            foreach (var map in maps)
            {
                var worldMap = new WorldMap(map);
                Maps[map.Id] = worldMap;
            }
            stopwatch.Stop();

            Log.Information("Loaded {Maps} maps in {ElapsedMilliseconds}ms.", Maps.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static void LoadScripts()
        {
            ScriptLoader.Initialize();
        }

        public static void LoadBCardHandlers()
        {
            BCardHandlerFactory.Initialize();
        }

        public static void LoadConfiguration()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ConfigManager.LoadWorldConfiguration();
            stopwatch.Stop();
            Log.Information("Loaded world configuration in {ElapsedMilliseconds}ms.", stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadItems()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Items != null) Items.Clear();
            var items = await WorldDbHelper.LoadAllItemsAsync();
            foreach (var item in items)
            {
                Items.Add(item);
            }
            stopwatch.Stop();

            Log.Information("Loaded {Items} items in {ElapsedMilliseconds}ms.", Items.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadBuffs()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Buffs != null) Buffs.Clear();
            var buffs = await WorldDbHelper.LoadAllBuffsAsync();
            foreach (var buff in buffs)
            {
                Buffs.Add(buff);
            }
            stopwatch.Stop();
            Log.Information("Loaded {Buffs} buffs in {ElapsedMilliseconds}ms.", Buffs.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadSkills()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Skills != null) Skills.Clear();
            var skills = await WorldDbHelper.LoadAllSkillsAsync();
            foreach (var skill in skills)
            {
                Skills.Add(skill);
            }
            stopwatch.Stop();

            Log.Information("Loaded {Skills} skills in {ElapsedMilliseconds}ms.", Skills.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadNpcs()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Npcs != null) Npcs.Clear();
            var npcs = await WorldDbHelper.LoadAllNpcsAsync();
            foreach (var npc in npcs)
            {
                Npcs.Add(npc);
            }

            await InitializeAllNpcs();

            stopwatch.Stop();

            Log.Information("Loaded {Npcs} npcs in {ElapsedMilliseconds}ms.", Npcs.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadShops()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Shops != null) Shops.Clear();
            var shops = await WorldDbHelper.LoadAllShopsAsync();
            foreach (var shop in shops)
            {
                Shops.Add(shop);
            }
            stopwatch.Stop();

            Log.Information("Loaded {Shops} shops in {ElapsedMilliseconds}ms.", Shops.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadShopItems()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (ShopItems != null) ShopItems.Clear();
            var shopItems = await WorldDbHelper.LoadAllShopItemsAsync();
            foreach (var shopItem in shopItems)
            {
                ShopItems.Add(shopItem);
            }
            stopwatch.Stop();

            Log.Information("Loaded {ShopItems} shop items in {ElapsedMilliseconds}ms.", ShopItems.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadShopTranslations()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (ShopTranslations != null) ShopTranslations.Clear();
            var shopTranslations = await WorldDbHelper.LoadAllShopTranslationsAsync();
            foreach (var shopTranslation in shopTranslations)
            {
                ShopTranslations.Add(shopTranslation);
            }
            stopwatch.Stop();

            Log.Information("Loaded {ShopTranslations} shop translations in {ElapsedMilliseconds}ms.", ShopTranslations.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadPortals()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Portals != null) Portals.Clear();
            var portals = await WorldDbHelper.LoadAllPortalsAsync();
            foreach (var portal in portals)
            {
                Portals.Add(portal);
            }
            foreach (var portal in Portals)
            {
                if (!Maps.ContainsKey(portal.FromMapId)) continue;
                Maps[portal.FromMapId].Portals.Add(portal);
            }
            stopwatch.Stop();

            Log.Information("Loaded {Portals} portals in {ElapsedMilliseconds}ms.", Portals.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadDrops()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Drops != null) Drops.Clear();
            var drops = await WorldDbHelper.LoadAllDropsAsync();
            foreach (var drop in drops)
            {
                Drops.Add(drop);
            }
            stopwatch.Stop();

            Log.Information("Loaded {Drops} drops in {ElapsedMilliseconds}ms.", Drops.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadNpcMonsters()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (NpcMonsters != null) NpcMonsters.Clear();
            var npcMonsters = await WorldDbHelper.LoadAllNpcMonstersAsync();
            foreach (var npcMonster in npcMonsters)
            {
                NpcMonsters.Add(npcMonster);
            }
            stopwatch.Stop();

            Log.Information("Loaded {NpcMonsters} npc monsters in {ElapsedMilliseconds}ms.", NpcMonsters.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadMonsters()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Monsters != null) Monsters.Clear();
            var monsters = await WorldDbHelper.LoadAllMonstersAsync();
            foreach (var monster in monsters)
            {
                Monsters.Add(monster);
            }
            foreach (var monster in Monsters)
            {
                if (!Maps.ContainsKey(monster.MapId)) continue;
                Maps[monster.MapId].Monsters.Add(monster);
            }

            await InitializeAllMonsters();

            stopwatch.Stop();

            Log.Information("Loaded {Monsters} monsters in {ElapsedMilliseconds}ms.", Monsters.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static async Task LoadChannels()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (Channels != null) Channels.Clear();
            var channels = await AuthDbHelper.LoadAllChannelsAsync();
            foreach (var channel in channels)
            {
                Channels.Add(channel);
            }
            stopwatch.Stop();

            Log.Information("Loaded {Channels} channels in {ElapsedMilliseconds}ms.", Channels.Count(), stopwatch.ElapsedMilliseconds);
        }

        public static short GetServerId()
        {
            return ServerId;
        }

        public static SkillData GetSkillByCastId(int castId)
        {
            return Skills.FirstOrDefault(x => x.CastId == castId);
        }

        public static short GetCastIdOfSkill(short vNum)
        {
            var skill = Skills.FirstOrDefault(x => x.SkillVNum == vNum);
            return skill?.CastId ?? 0;
        }

        public static SkillData GetSkillByCastIdAndVNum(int castId, short vnum)
        {
            return Skills.FirstOrDefault(x => x.CastId == castId && x.SkillVNum == vnum);
        }

        public static SkillData GetSkill(short vNum)
        {
            return Skills.FirstOrDefault(x => x.SkillVNum == vNum);
        }

        public static async Task<WorldLog> GetWorldLogByCharacterName(string characterName)
        {
            var log = await WorldDbHelper.LoadWorldBySource(characterName);
            return log;
        }

        public static short GetAllOnlinePlayers() => (short)SessionManager.GetOnlineSessionsAsync().Count();

        public static async Task Initialize()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            LoadConfiguration();
            await LoadMaps();
            await LoadChannels();
            await SetDictionaryChannels();
            await LoadItems();
            await LoadPortals();
            await LoadNpcMonsters();
            await LoadMonsters();
            await LoadDrops();
            await LoadNpcs();
            await LoadShopTranslations();
            await LoadShops();
            await LoadShopItems();
            await LoadSkills();
            await LoadBuffs();

            LoadScripts();
            LoadBCardHandlers();

            await UpdateChannels();
            await UpdatePeriodicallyFriends();

            MessageBeep(0x00000040);

            stopwatch.Stop();

            Log.Information("World started in {ElapsedMilliseconds}ms.", stopwatch.ElapsedMilliseconds);
        }

        private static async Task SetDictionaryChannels()
        {
            var channels = await AuthDbHelper.LoadAllChannelsAsync();
            foreach (var channel in channels)
            {
                _channelInstances[channel.ChannelId] = new Dictionary<short, MapInstance>();
            }
            foreach (var map in Maps)
            {
                foreach (var channel in channels)
                {
                    if (!_channelInstances[channel.ChannelId].ContainsKey(map.Value.Id))
                    {
                        _channelInstances[channel.ChannelId][map.Value.Id] = new MapInstance(map.Value);
                    }
                }
            }
        }

        public static async Task<Map> GetMap(short mapId)
        {
            var map = await WorldDbHelper.LoadByMapId(mapId);
            return map;
        }

        public static Item GetItem(int itemId)
        {
            return Items.FirstOrDefault(x => x.Id == itemId);
        }

        public static async Task<MapInstance> GetInstance(short mapId, int channelId)
        {
            /* if (!_channelInstances[channelId].ContainsKey(mapId))
             {
                 var map = GetWorldMap(mapId);
                 if (map != null)
                 {
                     _channelInstances[channelId][mapId] = new MapInstance(map);
                 }
             }*/

            return _channelInstances[channelId].GetValueOrDefault(mapId, null);
        }

        public static async Task InitializeAllMonsters()
        {
            var channels = await AuthDbHelper.LoadAllChannelsAsync();

            var tasks = new List<Task>();

            foreach (var channel in channels)
            {
                if (_channelInstances.TryGetValue(channel.ChannelId, out var maps))
                {
                    foreach (var mapInstance in maps.Values)
                    {
                        tasks.Add(InitializeMonstersForChannel(mapInstance, channel.ChannelId));
                    }
                }
            }

            await Task.WhenAll(tasks);
        }


        public static BuffData Getbuff(short id)
        {
            return Buffs.FirstOrDefault(x => x.BuffId == id);
        }

        public static async Task InitializeAllNpcs()
        {
            var channels = await AuthDbHelper.LoadAllChannelsAsync();

            foreach (var channel in channels)
            {
                if (_channelInstances.TryGetValue(channel.ChannelId, out var maps))
                {
                    foreach (var mapInstance in maps.Values)
                    {
                        await InitializeNpcsForChannel(mapInstance, channel.ChannelId);
                    }
                }
            }
        }

        private static async Task InitializeNpcsForChannel(MapInstance mapInstance, int channelId)
        {
            foreach (var npc in Npcs.Where(x => x.MapId == mapInstance.Template.Id))
            {
                var monster = NpcMonsters.FirstOrDefault(x => x.VNum == npc.VNum);
                if (monster == null)
                    continue;

                var entity = new NpcEntity(npc, monster)
                {
                    ChannelId = channelId
                };

                mapInstance.NpcEntities.Add(entity);
                _ = entity.InitializeMove(mapInstance);
                _ = entity.Stats.Initialize();
            }
        }

        private static async Task InitializeMonstersForChannel(MapInstance mapInstance, int channelId)
        {
            _instances.Add(mapInstance);

            foreach (var monster in Monsters.Where(x => x.MapId == mapInstance.Template.Id))
            {
                var npcMonster = NpcMonsters.FirstOrDefault(x => x.VNum == monster.VNum);
                if (npcMonster == null)
                    continue;

                var monsterEntity = new MonsterEntity(monster, npcMonster, mapInstance)
                {
                    ChannelId = channelId
                };

                mapInstance.MonsterEntities.Add(monsterEntity);
                _ = monsterEntity.Initialize();
            }

            // Creamos un token de cancelación que puedas guardar por mapa si quieres detenerlo luego
            var cts = new CancellationTokenSource();
            _ = new MonsterMovementTick(mapInstance, cts.Token).StartAsync();
        }


        public static Task<List<BCard>> GetBCardsFromSkill(short skillVNum)
        {
            return WorldDbHelper.LoadSkillBCardsBySkillVNumAsync(skillVNum);
        }

        public static Monster GetMonsterByVNum(short vNum) => Monsters.FirstOrDefault(x => x.VNum == vNum);

        public static MapItem GetMapItemById(short id, MapInstance instance)
        {
            return instance.Items.FirstOrDefault(x => x.Id == id);
        }

        public static Task<List<BCard>> GetBCardsFromBuff(short bId)
        {
            return WorldDbHelper.LoadBuffBCardsByBuffIdAsync(bId);
        }

        public static WorldMap GetWorldMap(short mapId) => Maps.GetValueOrDefault(mapId, null);

        public static void RemoveInstance(short mapId, int channelId)
        {
            if (_channelInstances.ContainsKey(channelId) && _channelInstances[channelId].ContainsKey(mapId))
            {
                _channelInstances[channelId].Remove(mapId);
            }
        }

        public static ClientSession GetPlayerByName(string name)
        {
            var sessions = SessionManager.GetOnlineSessionsAsync().ToList();
            return sessions.FirstOrDefault(x => x.Player.Name == name);
        }

        public static ClientSession GetPlayerById(int id)
        {
            var sessions = SessionManager.GetOnlineSessionsAsync().ToList();

            return sessions.FirstOrDefault(x => x.Player.Id == id);
        }

        public static async Task UpdateFriends(ClientSession session)
        {
            var friends = await CharacterDbHelper.LoadAllFriendsAsync();

            foreach (var friend in friends.Where(x => x.CharacterId == session.Player.Id))
            {
                var sessions = SessionManager.GetOnlineSessionsAsync();
                if (sessions.Find(x => friend.FriendCharacterId == x.Player.Id) is var getSession)
                {
                    if (getSession is null)
                    {
                        continue;
                    }
                    if (getSession.ChannelId == session.ChannelId)
                    {
                        await getSession.Player.ChatSayById(MessageId.FRIEND_IS_NOW_ONLINE, ChatColor.Yellow, session.Player.Name, type: MessageType.WHISP_NON_CONNECTED);
                    }
                }
            }
        }

        public static async Task<Npc> GetNpcById(int id)
        {
            var npc = await WorldDbHelper.LoadNpcById(id);
            return npc;
        }

        public static async Task Shout(string message)
        {
            var sessions = SessionManager.GetOnlineSessionsAsync().ToList();
            string admin = "(Administrador)";
            foreach (var session in sessions)
            {
                await session.SendPacket($"msg 2 {message}");
                await session.Player.ChatSay($"{admin} {message}", ChatColor.Yellow);
            }
        }

        public static async Task AddPortal(short toMapId, short toMapX, short toMapY, short fromMapId = -1, short fromMapX = -1, short fromMapY = -1)
        {
            Portal portal = new Portal
            {
                FromMapId = fromMapId,
                FromMapX = fromMapX,
                FromMapY = fromMapY,
                ToMapId = toMapId,
                ToMapX = toMapX,
                ToMapY = toMapY
            };

            await WorldDbHelper.InsertPortalAsync(portal);
        }

        public static bool IsPlayerOnline(int id)
        {
            var sessions = SessionManager.GetOnlineSessionsAsync().ToList();
            return sessions.Any(x => x.IsInGame && x.Player.Id == id);
        }

        public static bool IsPlayerOnline(string name) => GetPlayerByName(name) != null;

        private static async Task UpdatePeriodicallyFriends()
        {
            Observable.Interval(TimeSpan.FromSeconds(15)).Subscribe(async _ =>
            {
                try
                {
                    foreach (var session in SessionManager.GetOnlineSessionsAsync())
                    {
                        if (session.Player is null) continue;
                        await session.Player.GetAllFriends();
                    }
                    Log.Information("Updated friends.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error on UpdatePeriodicallyFriends");
                }
            });
        }

        public static async Task<CharacterItem> GetCharacterItem(int slot, int characterId)
        {
            return await CharacterDbHelper.LoadItemBySlot(slot, characterId);
        }

        public static async Task<Character> GetCharacterById(int id) => await CharacterDbHelper.LoadCharacterById(id);

        public static async Task<Player> GetPlayerByCharacterId(int id)
        {
            var sessions = SessionManager.GetOnlineSessionsAsync().ToList();
            return sessions.FirstOrDefault(x => x.Player.Id == id)?.Player;
        }

        public static async Task<CharacterItem> GetCharacterItemById(int charId, int id) => await CharacterDbHelper.LoadItemById2(charId, id);

        private static async Task UpdateChannels()
        {
            Observable.Interval(TimeSpan.FromSeconds(20)).Subscribe(async _ =>
            {
                try
                {
                    var sessions = SessionManager.GetOnlineSessionsAsync();
                    var aliveSessions = sessions.Where(x => x.IsAlreadyConnected).ToList();

                    var groupedByChannel = sessions
                        .GroupBy(s => s.ChannelId)
                        .ToDictionary(g => g.Key, g => g.Count());

                    if (!groupedByChannel.Any())
                    {
                        foreach (var channel in await AuthDbHelper.LoadAllChannelsAsync())
                        {
                            await AuthDbHelper.UpdateChannelOnlinePlayers(channel.ChannelId, 0);
                        }
                    }

                    foreach (var (channelId, count) in groupedByChannel)
                    {
                        Log.Information("Updating channel {ChannelId} with {Count} players.", channelId, count);
                        await AuthDbHelper.UpdateChannelOnlinePlayers(channelId, count);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error on UpdateChannels");
                }
            });
        }
    }
}