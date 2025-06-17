using AutoMapper;
using Configuration;
using Configuration.Config;
using Database.Helper;
using Database.Item;
using Database.Player;
using Database.World;
using Enum.Main.BCardEnum;
using Enum.Main.BuffEnum;
using Enum.Main.CharacterEnum;
using Enum.Main.ChatEnum;
using Enum.Main.EffectEnum;
using Enum.Main.EntityEnum;
using Enum.Main.ItemEnum;
using Enum.Main.MessageEnum;
using Enum.Main.OptionEnum;
using Enum.Main.SpecialistEnum;
using GameWorld;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using World.Entities.Components;
using World.Extensions;
using World.Gameplay;
using World.Gameplay.Script;
using World.Gameplay.Script.Objects;
using World.GameWorld;
using World.Network;
using World.Utils.Configuration;
using static System.Collections.Specialized.BitVector32;

namespace World.Entities
{
    public class Player : Character
    {
        public ClientSession Session { get; set; }
        public byte BeforeOrientation { get; set; }
        public byte Orientation { get; set; }
        public byte Scale { get; set; } = 100;
        public byte Speed { get; set; }
        public int Morph { get; set; }
        public int SpecialMorph { get; set; }
        public byte SpUpgrade { get; set; }
        public SpWings SpWings { get; set; }
        public bool UsingSpecialist { get; set; }
        public PlayerPackets Packets => new PlayerPackets(Session);
        public bool IsSitting { get; set; }
        public MapInstance CurrentMap { get; set; }
        public IDisposable WalkDisposable;
        public Inventory Inventory { get; set; }
        public DateTime LastMoveItem { get; set; }
        public DateTime LastSortedInventory { get; set; }
        public Dictionary<EquipmentType, CharacterItem> EquippedItems { get; set; } = new();
        public DateTime LastUsedSpecialist { get; set; }
        public bool IsInvisible { get; set; }
        public DateTime LastMove { get; set; }
        public int SpeedHackSuspectCount { get; set; }
        public DateTime LastPulse { get; set; } = DateTime.Now;
        public bool IsUsingMount { get; set; }
        public List<ActionBar> ActionBars { get; set; } = new();
        public int TargetMonsterId { get; set; }

        public GameEntity TargetEntity { get; set; }
        public GameEntity GameEntity { get; set; }
        public DateTime LastUsedSkill { get; set; }

        public DateTime LastMoveFromPortals { get; set; }

        public List<CharacterSkill> SpecialistSkills { get; set; } = new();
        public List<CharacterSkill> Skills { get; set; } = new();
        public List<BuffData> Buffs { get; set; } = new();

        public long Experience { get; set; }

        public int ChargeData { get; set; }

        // Crear evento que maneje cuando el jugador mata un monstruo.
        public event Action<Player, MonsterEntity> OnMonsterKilled;

        public async Task<CharacterItem> GetEquippedSpecialist() => Inventory.GetEquippedItemFromSlot((byte)EquipmentType.SPECIALIST);

        public bool HasCustomSpeed { get; set; }

        public bool IsOnline
        {
            get
            {
                return Session != null;
            }
        }

        public Dictionary<ClassId, byte> BaseSpeed = new()
        {
            { ClassId.Adventurer, 12 },
            { ClassId.Swordsman, 12 },
            { ClassId.Archer, 12 },
            { ClassId.Mage, 11 },
            { ClassId.MartialArtist, 12 }
        };

        public bool IsSpTransformed
        {
            get
            {
                return Morph != 0;
            }
        }

        public bool IsMountTransformed
        {
            get
            {
                return SpecialMorph != 0;
            }
        }

        public bool IsWalking()
        {
            return LastMove != DateTime.Now;
        }

        private IDisposable _timerSave;

        public PlayerStatComponent Stats { get; set; }

        public Player() { } // ignore

        public Player(ClientSession session, Character character)
        {
            Session = session;
            MapperConfig.Mapper.Map(character, this);
            Stats = new PlayerStatComponent(this);

            GameEntity = new GameEntity(this);
        }

        public bool IsMage => Class == ClassId.Mage;

        public bool IsArcher => Class == ClassId.Archer;

        public bool IsSwordsman => Class == ClassId.Swordsman;

        public bool IsMartialArtist => Class == ClassId.MartialArtist;

        public bool IsAdventurer => Class == ClassId.Adventurer;

        public ClientSession OtherSessionExchange { get; set; }

        public Dictionary<CharacterItem, short> ExchangeItems { get; set; } = new(); // <Item, Amount>

        public long ExchangeGoldAmount { get; set; }

        public bool IsExchanging { get; set; }

        public bool ExchangeConfirmed { get; set; }

        public async Task LoadInventory()
        {
            if (Inventory == null)
                Inventory = new Inventory(Session);

            await Inventory.LoadInventory();
            await Session.Player.Packets.GenerateInvPacket();
            await Session.Player.Packets.GenerateInventoryExts();
            await Session.Player.Packets.GenerateGoldPacket();
        }

        public async Task AddItem(short id, short amount, byte rare, byte upgrade)
        {
            await Inventory.AddItemToInventory(Id, id, amount, rare, upgrade);
        }

        public async Task SpawnEffect(int effId)
        {
            await Session.SendPacket(Packets.GenerateEffectPacket(effId));
        }

        public async Task HealHp(int amount)
        {
            Stats.CurrentHealth += Math.Min(Stats.CurrentHealth + amount, await Stats.MaxHealth());

            if (Stats.CurrentHealth > await Stats.MaxHealth())
                Stats.CurrentHealth = await Stats.MaxHealth();

            if (Stats.CurrentHealth <= 0)
            {
            }

            await Session.SendPacket(Packets.GenerateStat());
            await Packets.GenerateHealPacket(amount);
        }

        public async Task GetDamage(int amount)
        {
            Stats.CurrentHealth -= amount;
            await Session.SendPacket(Packets.GenerateStat());

            // Reduce damage by defenses

            if (Stats.CurrentHealth <= 0)
            {
                Die();
            }

            await Packets.GenerateGetDamagePacket(amount);
        }

        // Manejar el evento de cuando el jugador mata un monstruo y otorgarle experiencia al jugador.
        public async Task HandleMonsterKilled(MonsterEntity monster)
        {
            OnMonsterKilled?.Invoke(this, monster);
            int exp = monster.Stats.GetExpForPlayer();
            int jExp = monster.Stats.GetJExpForPlayer();
            if (exp <= 0) return;
            if (jExp <= 0) return;
            await AddExp(exp);
            await AddJobExp(jExp);
            await Session.SendPacket(Packets.GenerateLev());
        }

        public async Task AddExp(int amount)
        {
            // crear una variable long con la variable del parametro amount y agregarle los rates de exp del servidor.

            var expRate = ConfigManager.WorldServerConfig.Rates.ExpRate;

            var bonus = 1.0;

            foreach(var buff in Buffs)
            {
                var buffCards = await WorldManager.GetBCardsFromBuff(buff.BuffId);

                foreach(var bCard in buffCards)
                {
                    if (bCard.Type == BCardType.AdvancedExp && bCard.SubType == (BCardEffect)41)
                    {
                        bonus += bCard.FirstEffectValue / 100.0;
                    }

                    if (bCard.Type == BCardType.Experience && bCard.SubType == (BCardEffect)11)
                    {
                        bonus += bCard.FirstEffectValue / 100.0;
                    }
                }
            }

            var amountExp = (long)amount * expRate * (1 + bonus / 100.0);

            if (Level >= ConfigManager.WorldServerConfig.Gameplay.MaxLevel) return;

            if (Exp >= Stats.ExperienceToUp)
            {
                await AddLevel(1);
                Exp = 0;
            }
            else
            {
                Exp += (long)amountExp;
                await Session.SendPacket(Packets.GenerateLev());
            }
            await UpdateCharacter();
        }

        public async Task AddJobExp(int amount)
        {
            var jobExpRate = ConfigManager.WorldServerConfig.Rates.JobExpRate;
            var bonus = 1.0;
            foreach (var buff in Buffs)
            {
                var buffCards = await WorldManager.GetBCardsFromBuff(buff.BuffId);

                foreach (var bCard in buffCards)
                {
                    if (bCard.Type == BCardType.Buff && bCard.SubType == (BCardEffect)41)
                    {
                        bonus += bCard.FirstEffectValue / 100.0;
                    }

                    if (bCard.Type == BCardType.Experience && bCard.SubType == (BCardEffect)11)
                    {
                        bonus += bCard.FirstEffectValue / 100.0;
                    }
                }
            }

            var amountExp = (long)amount * jobExpRate * (1 + bonus / 100.0);

            if (JobLevel >= ConfigManager.WorldServerConfig.Gameplay.MaxJobLevel) return;

            if (JobExp >= Stats.JobExperienceToUp)
            {
                await AddJobLevel(1);
                JobExp = 0;
            }
            else
            {
                JobExp += (long)amountExp;
                await Session.SendPacket(Packets.GenerateLev());
            }
            await UpdateCharacter();
        }

        public async Task SetSpeed(byte speed)
        {
            Speed = speed;
            await Session.SendPacket(Packets.GeneratePlayerMove());
        }

        public async Task AddSpeed(byte speed)
        {
            Speed += speed;
            await Session.SendPacket(Packets.GeneratePlayerMove());
        }

        public async Task ReduceSpeed(byte speed)
        {
            Speed -= speed;
            await Session.SendPacket(Packets.GeneratePlayerMove());
        }

        public async Task ChatSay(string message, ChatColor color)
        {
            await Session.SendPacket($"say 1 {Id} {(byte)color} {message}");
        }

        public async Task ChatSay(string message, ChatColor color, bool ignoreName)
        {
            await Session.SendPacket($"say {(byte)(ignoreName ? 2 : 1)} {Id} {(byte)color} {message}");
        }

        public async Task ChatSay(string message, ChatColor color, bool ignoreName, bool isForAll)
        {
            if (isForAll)
            {
                await Session.Player.CurrentMap.Broadcast($"say {(byte)(ignoreName ? 2 : 1)} {Id} {(byte)color} {message}", Session);
            }
        }

        public async Task ChatSayById(MessageId id, ChatColor color, short param = 0, short param2 = 0, MessageType type = MessageType.MESSAGE)
        {
            // param puede ser item id o cualquier otro valor que no sea un item, puede variar.
            // param 2 puede ser el amount del item mo cualquier otro valor que no sea referente al item, puede variar.
            await Session.SendPacket($"sayi 1 {Id} {(byte)color} {(short)id} {(byte)type} {param} {param2} 0 0"); // luego de short id, hay un 2, ese 2 es el tipo de búsqueda ITEM.
        }

        public async Task ChatSayById(MessageId id, ChatColor color, string param, short param2 = 0, MessageType type = MessageType.MESSAGE)
        {
            await Session.SendPacket($"sayi 1 {Id} {(byte)color} {(short)id} {(byte)type} {param} {param2} 0 0"); // luego de short id, hay un 2, ese 2 es el tipo de búsqueda ITEM.
        }

        public async Task ChatSayById(MessageId id, ChatColor color, byte param, string param2, string param3, MessageType type = MessageType.MESSAGE)
        {
            await Session.SendPacket($"sayi 1 {Id} {(byte)color} {(short)id} {(byte)type} {param} {param2} {param3} 0"); // luego de short id, hay un 2, ese 2 es el tipo de búsqueda ITEM.
        }

        public async Task SpawnEffect(Effect effect)
        {
            await CurrentMap.Broadcast(Packets.GenerateEffect(effect));
        }

        public async Task SetSpecialistWings(short wings)
        {
            var sp = await GetEquippedSpecialist();
            if (sp is null) return;
            sp.Rarity = (byte)wings;
            await CharacterDbHelper.UpdateItemAsync(sp);
            await Packets.GenerateCModePacket();
        }

        public async Task AddLevel(byte level)
        {
            Level += level;
            await SendMsgi(MessageId.LEVEL_UP);
            await Session.SendPacket(Packets.GenerateLev());
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP));
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP_SHINY));

            Stats.loadHPData();
            Stats.loadMPData();
            Stats.loadEXPData();
            await Session.SendPacket(Packets.GenerateStat());

            foreach(var script in ScriptLoader.GetScriptsForPlayer(this))
            {
                await script.OnLevelUp(Level);
            }

            await UpdateCharacter();
        }

        public async Task LoadSkills()
        {
            Skills = await CharacterDbHelper.LoadCharacterSkillsByCharacterIdAsync(Id);
            if (Skills.Count == 0)
            {
                Log.Warning($"Player {Name} has no skills loaded.");
                return;
            }
            await GetSkills();
        }

        public async Task GetSkills()
        {
            // Como obtengo todos los elementos de una lista?
            // List<CharacterSkill> skills = UsingSpecialist ? SpecialistSkills : Skills;

            var getSkills = UsingSpecialist
                ? SpecialistSkills.Concat(Skills.OrderBy(x => x.CastId).Where(x => x.VNum < 200)).ToList()
                : Skills;

            // Crea una lista de CharacterSkill y condiciona si se usa especialista para concatenar las habilidades de especialista con las normales, de lo contrario sólo las normales.
            List<CharacterSkill> skills = getSkills;

            string packet = "ski 0";

            if (skills.Count >= 2)
            {
                if (UsingSpecialist)
                {
                    packet += $" {skills[0].VNum} {skills[0].VNum}";
                }
                else
                {
                    packet += $" {skills[0].VNum} {skills[1].VNum}";
                }
            }



            packet = skills.Aggregate(packet, (pck, charskill) => $"{pck} {charskill.VNum}");
            await Session.SendPacket(packet);
        }

        public async Task GetSpecialistSkills()
        {
            SpecialistSkills.Clear();
            var skills = WorldManager.Skills
                .Where(s => s.UpgradeType == Morph && s.SkillType == 1)
                .OrderBy(s => s.Level)
                .ToList();


            foreach (var skill in skills)
            {
                SpecialistSkills.Add(new CharacterSkill
                {
                    CharacterId = Id,
                    VNum = skill.SkillVNum,
                    CastId = (byte)skill.CastId
                });
            }
        }

        public SkillData GetSkillByCastId(int castId)
        {
            var skill = WorldManager.GetSkillByCastId(castId);
            Console.WriteLine(skill.SkillVNum);
            if (Skills.Any(x => x.CastId == castId))
            {
                var sk = WorldManager.GetSkillByCastIdAndVNum(castId, skill.SkillVNum);
                if (sk != null)
                {
                    skill = sk;
                }
            }
            else
            {
                Log.Error($"Skill with castId {castId} not found in player's skills.");
                return null;
            }
            return skill;
        }

        public async Task LearnSkill(short vnum)
        {
            // Check if the skill already exists
            if (Skills.Any(s => s.VNum == vnum))
            {
                return;
            }
            // Create a new CharacterSkill instance
            CharacterSkill newSkill = new CharacterSkill
            {
                CharacterId = Id,
                VNum = vnum,
                CastId = (byte)WorldManager.GetCastIdOfSkill(vnum),
            };

            // Add the new skill to the Skills list
            Skills.Add(newSkill);
            // Update the database with the new skill
            await CharacterDbHelper.InsertCharacterSkillAsync(newSkill);
            // Send the updated skills to the client
            await GetSkills();

        }

        public async Task ChangeProfession(ClassId prof)
        {
            await CharacterDbHelper.RemoveCharacterSkillsAsync(Skills);
            Skills.Clear();
            Class = prof;
            await UpdateCharacter();

            Stats.loadHPData();
            Stats.loadMPData();

            await Session.SendPacket("npinfo 0");
            await Session.SendPacket("p_clear");
            await Session.Player.Packets.GenerateCModePacket();
            await Session.SendPacket(Packets.GeneratePlayerMove());
            await Session.Player.CurrentMap.Broadcast(await Packets.GenerateEqPacket());
            await Session.SendPacket(Packets.GenerateLev());
            await CurrentMap.Broadcast(await Packets.GenerateIn(), Session);
            await CurrentMap.Broadcast(Packets.GenerateGidx());
            await Session.SendPacket(Packets.GenerateStat());
            await SpawnEffect(Effect.CHANGE_CLASS);

            switch (Class)
            {
                case ClassId.Swordsman:
                    await LearnSkill(220);
                    await LearnSkill(221);
                    break;

                case ClassId.Archer:
                    await LearnSkill(240);
                    await LearnSkill(241);
                    break;

                case ClassId.Mage:
                    await LearnSkill(260);
                    await LearnSkill(261);
                    break;
            }
            await LearnSkill(209);
            await GetSkills();
        }

        public async Task SetReputation(int amount)
        {
            Reputation = amount;
            await ChatSayById(MessageId.INCREASED_REPUTATION, ChatColor.Green, (short)amount, type: MessageType.MESSAGE);
            await Packets.GenerateRepAndDignityPacket();
            await UpdateCharacter();
        }

        public async Task AddReputation(int amount)
        {
            Reputation += amount;
            await ChatSayById(MessageId.INCREASED_REPUTATION, ChatColor.Green, (short)amount, type: MessageType.MESSAGE);
            await Packets.GenerateRepAndDignityPacket();
            await UpdateCharacter();
        }

        public async Task SetDignity(int amount)
        {
            Dignity = amount;
            await Packets.GenerateRepAndDignityPacket();
            await ChatSayById(MessageId.INCREASED_DIGNITY, ChatColor.Green, (short)amount, type: MessageType.MESSAGE);
            await UpdateCharacter();
        }

        public async Task AddDignity(int amount)
        {
            Dignity += amount;
            await Packets.GenerateRepAndDignityPacket();
            await ChatSayById(MessageId.INCREASED_DIGNITY, ChatColor.Green, (short)amount, type: MessageType.MESSAGE);
            await UpdateCharacter();
        }

        public int GetDignityIcon()
        {
            if (Dignity <= -800) return 6;
            if (Dignity <= -600) return 5;
            if (Dignity <= -400) return 4;
            if (Dignity <= -200) return 3;
            if (Dignity <= -100) return 2;
            return 1;
        }

        public int GetReputationIcon()
        {
            if (Reputation > 5_000_000)
            {
                /* return IsReputationHero() switch se le asigna un icon según el hero level creo.
                 {
                     1 => 28,
                     2 => 29,
                     3 => 30,
                     4 => 31,
                     5 => 32,
                     _ => 27
                 };*/
            }

            var reputationThresholds = new Dictionary<int, int>
            {
                { 50, 1 },
                { 150, 2 },
                { 250, 3 },
                { 500, 4 },
                { 750, 5 },
                { 1000, 6 },
                { 2250, 7 },
                { 3500, 8 },
                { 5000, 9 },
                { 9500, 10 },
                { 19000, 11 },
                { 25000, 12 },
                { 40000, 13 },
                { 60000, 14 },
                { 85000, 15 },
                { 115000, 16 },
                { 150000, 17 },
                { 190000, 18 },
                { 235000, 19 },
                { 285000, 20 },
                { 350000, 21 },
                { 500000, 22 },
                { 1_500_000, 23 },
                { 2_500_000, 24 },
                { 3_750_000, 25 },
                { 5_000_000, 26 }
            };

            foreach (var threshold in reputationThresholds)
            {
                if (Reputation <= threshold.Key)
                    return threshold.Value;
            }

            return 27;
        }

        public async Task SetLevel(byte level)
        {
            Level = level;
            Exp = 0;
            await SendMsgi(MessageId.LEVEL_UP);
            await Session.SendPacket(Packets.GenerateLev());
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP));
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP_SHINY));
            Stats.loadHPData();
            Stats.loadMPData();
            Stats.loadEXPData();
            await Session.SendPacket(Packets.GenerateStat());

            await UpdateCharacter();
        }

        public async Task AddJobLevel(byte level)
        {
            JobLevel += level;
            await SendMsgi(MessageId.JOB_LEVEL_UP);
            await Session.SendPacket(Packets.GenerateLev());
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP));
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP_SHINY));
            Stats.loadJobXPData();
            foreach (var script in ScriptLoader.GetScriptsForPlayer(this))
            {
                await script.OnJobLevelUp(JobLevel);
            }

            await UpdateCharacter();
        }

        public async Task SetJobLevel(byte level)
        {
            JobLevel = level;
            await SendMsgi(MessageId.JOB_LEVEL_UP);
            await Session.SendPacket(Packets.GenerateLev());
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP));
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateEffect(Effect.LEVEL_UP_SHINY));
            Stats.loadJobXPData();
            await UpdateCharacter();
        }

        public async Task UpdateCharacter()
        {
            await CharacterDbHelper.UpdateAsync(this);
        }

        public async Task GetActionBarList()
        {
            ActionBars = await CharacterDbHelper.LoadAllActionBarAsyncByPlayerId(Id);
            if (ActionBars.Count == 0) return;

            string qslot0 = "qslot 0";
            string qslot1 = "qslot 1";

            for (int i = 0; i < 30; i++)
            {
                var q0 = ActionBars.FirstOrDefault(x => x.Q1 == 0 && x.Q2 == i && x.Morph == (UsingSpecialist ? Morph : 0));
                var q1 = ActionBars.FirstOrDefault(x => x.Q1 == 1 && x.Q2 == i && x.Morph == (UsingSpecialist ? Morph : 0));

                qslot0 += $" {(q0?.Type ?? 7)}.{(q0?.Slot ?? 7)}.{(q0?.Pos ?? -1)}";
                qslot1 += $" {(q1?.Type ?? 7)}.{(q1?.Slot ?? 7)}.{(q1?.Pos ?? -1)}";
            }

            await Session.SendPacket(qslot0);
            await Session.SendPacket(qslot1);
        }

        public async Task AddBuff(short id)
        {
            var buff = WorldManager.Getbuff(id);
            if (buff == null)
            {
                Log.Warning("A buff is null.");
                return;
            }

            if (!Buffs.Contains(buff))
            {
                Buffs.Add(buff);
            }

            int chargeData = 7000;
            var buffTiming = buff.DurationMs;
            string buffPacket = $"bf 1 {Id} {(buff.BuffId is 0 ? ChargeData > chargeData ? chargeData : ChargeData : 0)}.{buff.BuffId}.{buffTiming} {buff.Level}";

            await Session.SendPacket(buffPacket);
            // sayi 1 58373 20 26 3 93 0 0 0
            await Session.SendPacket($"sayi 1 {GameEntity.Id} 20 {(int)MessageId.UNDER_BUFF_EFFECT} 3 {buff.BuffId} 0 0 0");

            var buffBCards = await WorldDbHelper.LoadBuffBCardsByBuffIdAsync(buff.BuffId);
            var bCardShadowAppear = buffBCards?.FirstOrDefault(x => x.Type == BCardType.ShadowAppear && x.SubType == (BCardEffect)51);
            if (bCardShadowAppear != null)
            {
                if (bCardShadowAppear.Type == BCardType.ShadowAppear && bCardShadowAppear.SubType == (BCardEffect)51)
                {
                    await CurrentMap.Broadcast($"guri 0 1 {GameEntity.Id} {bCardShadowAppear.FirstEffectValue} {bCardShadowAppear.SecondaryEffectValue}");
                }
            }
            await Session.SendPacket(Packets.GeneratePlayerMove());
            if (buff.EffectId > 0)
            {
                await Packets.GenerateEffect(buff.EffectId);
            }

            foreach(var script in ScriptLoader.GetScriptsForPlayer(this))
            {
                await script.OnReceiveBuff(buff);
            }

            // Crear un temporizador para eliminar el buff después de su duración
            if (buffTiming > 0)
            {
                Observable.Timer(TimeSpan.FromMilliseconds(buffTiming * 100)).Subscribe(async _ =>
                {
                    if (Buffs.Contains(buff))
                    {
                        if (bCardShadowAppear != null)
                        {
                            if (bCardShadowAppear.Type == BCardType.ShadowAppear && bCardShadowAppear.SubType == (BCardEffect)51)
                            {
                                await CurrentMap.Broadcast($"guri 0 1 {GameEntity.Id} 0 0");
                            }
                        }
                        Buffs.Remove(buff);
                        await Session.SendPacket($"bf 1 {Id} 0.{buff.BuffId}.0 {buff.Level}");
                        await Session.SendPacket($"sayi 1 {GameEntity.Id} 20 {(int)MessageId.BUFF_DISAPPEARED} 3 {buff.BuffId} 0 0 0");
                        await Session.SendPacket(Packets.GeneratePlayerMove());
                    }
                });
            }
        }

        public bool HasBuff(short buffId)
        {
            return Buffs.Any(x => x.BuffId == buffId);
        }

        public async Task RemoveBuff(short id)
        {
            var buff = Buffs.SingleOrDefault(x => x.BuffId == id);
            if (buff is null) return;

            await Session.SendPacket($"bf 1 {Id} 0.{buff.BuffId}.0 {buff.Level}");
            await Session.SendPacket($"sayi 1 {GameEntity.Id} 20 {(int)MessageId.BUFF_DISAPPEARED} 3 {buff.BuffId} 0 0 0");
            Buffs.Remove(buff);
        }
        public async Task RemoveBuffs()
        {
            foreach(var buff in Buffs)
            {
                var buffBCards = await WorldDbHelper.LoadBuffBCardsByBuffIdAsync(buff.BuffId);
                var bCardShadowAppear = buffBCards?.FirstOrDefault(x => x.Type == BCardType.ShadowAppear && x.SubType == (BCardEffect)51);
                if (bCardShadowAppear != null)
                {
                    if (bCardShadowAppear.Type == BCardType.ShadowAppear && bCardShadowAppear.SubType == (BCardEffect)51)
                    {
                        await CurrentMap.Broadcast($"guri 0 1 {GameEntity.Id} 0 0");
                    }
                }
                await Session.SendPacket($"bf 1 {Id} 0.{buff.BuffId}.0 {buff.Level}");
                await Session.SendPacket($"sayi 1 {GameEntity.Id} 20 {(int)MessageId.BUFF_DISAPPEARED} 3 {buff.BuffId} 0 0 0");
            }
            Buffs.Clear();
        }

        public async Task SetGold(long gold)
        {
            Gold = gold;
            await Packets.GenerateGoldPacket();
        }

        public async Task AddGold(long gold)
        {
            Gold += gold;
            await Packets.GenerateGoldPacket();
        }

        public async Task ReduceGold(long gold)
        {
            Gold -= gold;
            await Packets.GenerateGoldPacket();
        }

        public async Task SendMsgi(MessageId id, byte type = 0, byte param1 = 0, byte param2 = 0, byte param3 = 0, string param4 = "0", byte param5 = 0)
        {
            await Session.SendPacket($"msgi {type} {(int)id} {param1} {param2} {param3} {param4} {param5}");
        }

        public async Task GetAllFriends()
        {
            var friends = await CharacterDbHelper.LoadAllFriendsAsync();
            StringBuilder packet = new StringBuilder();
            packet.Append("finit");
            foreach (var friend in friends.Where(x => x.CharacterId == Session.Player.Id))
            {
                packet.Append($" {friend.FriendCharacterId}|{(friend.Married ? 5 : 0)}|{(WorldManager.IsPlayerOnline(friend.FriendCharacterId) ? 1 : 0)}|{friend.FriendName} ");
            }

            await Session.SendPacket(packet.ToString());
        }

        public int GetItemExpirationTime(short itemId)
        {
            var fromInv = Session.Player.Inventory.GetItemFromInventory(itemId);

            if (fromInv == null)
                return 0;
            var item = WorldManager.GetItem(fromInv.ItemId);

            if (item == null) return 0;

            var now = DateTime.Now;
            if (fromInv.TimeRemaining <= now) return 0;

            var remainingTime = fromInv.TimeRemaining - now;
            return (int)Math.Ceiling(remainingTime.TotalHours);
        }

        public async Task UpdateItemsExpirationTime()
        {
            var items = Inventory.Items.Where(x => x.TimeRemaining < DateTime.Now).ToList();
            foreach (var item in items)
            {
                var getItem = WorldManager.GetItem(item.ItemId);
                if (getItem.ItemValidTime == 0) continue;

                if (getItem.EquipmentTypeSlot == EquipmentType.AMULET || getItem.EquipmentTypeSlot == EquipmentType.COSTUME_HAT ||
                    getItem.EquipmentTypeSlot == EquipmentType.COSTUME_SUIT)
                {
                    await ChatSayById(MessageId.ITEM_EXPIRED, ChatColor.Red, (short)item.ItemId, type: MessageType.ITEM);

                    await Inventory.DeleteItemFromInventory(item.ItemId, 0, item.Slot);

                    await Packets.GenerateEquipmentPacket();
                }
            }
        }

        public async Task StartStatsFunction()
        {
            while (IsOnline && Stats.CurrentHealth > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));

                // Regeneration
                if (Stats.CurrentHealth < await Stats.MaxHealth())
                {
                    Stats.CurrentHealth += (int)(await Stats.MaxHealth() * 0.01 + Level * 1.5);
                    Health = Stats.CurrentHealth;
                    await Session.SendPacket(Packets.GenerateStat());
                }

                if (Stats.CurrentMana < await Stats.MaxMana())
                {
                    Stats.CurrentMana += (int)(await Stats.MaxMana() * 0.01 + Level * 1.5);
                    Mana = Stats.CurrentMana;
                    await Session.SendPacket(Packets.GenerateStat());
                }

                if (Stats.CurrentHealth <= 0)
                {
                    Stats.CurrentHealth = 0;
                    Stats.CurrentMana = 0;

                    await Die();
                }

                
            }
        }

        public async Task CharacterFeatures()
        {
            IDisposable timer = null;
            timer = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(async _ =>
            {
                if (Stats.CurrentHealth <= 0)
                {
                    timer?.Dispose();
                    return;
                }

                await Session.SendPacket(Packets.GenerateStat());

                // Haperdam buff
                if (Level < 80 && !HasBuff(684))
                {
                    await AddBuff(684);
                }
                else if (Level > 79 && HasBuff(684))
                {
                    await RemoveBuff(684);
                }
            });
        }

        public async Task StartItemExpirationTimer()
        {
            while (IsOnline)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                await UpdateItemsExpirationTime();
            }
        }

        public async Task SaveActionBars()
        {
            while (IsOnline)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                await CharacterDbHelper.UpdateActionBarListAsync(Session.Player.ActionBars, Id); // por qué lo había comentado?
            }
        }

        public async Task ChangeMap(MapInstance map, short x, short y)
        {
            await LeaveMap();

            if (map.Template == null)
            {
                if (Session.Account.Rank >= 3)
                {
                    await ChatSay("El mapa (Template) no existe.", ChatColor.Red);
                }
                return;
            }

            Session.Player.CurrentMap = map;
            Session.Player.MapId = map.Template.Id;
            Session.Player.MapPosX = x;
            Session.Player.MapPosY = y;
            Session.Player.MapPosX = x;
            Session.Player.MapPosY = y;

            Session.Player.CurrentMap.AddPlayer(this);

            await SetPlayerPacketsOnChangeMap();

            foreach (var player in CurrentMap.Players.Where(x => x.Id != Session.Player.Id))
            {
                await Session.SendPacket(await player.Packets.GenerateIn());
                await player.Packets.GeneratePairyPacket();
            }
            await Packets.GeneratePairyPacket();
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateGidx(), Session);

            foreach (var portal in WorldManager.Portals.Where(x => x.FromMapId == Session.Player.CurrentMap.Template.Id))
            {
                await Session.SendPacket($"gp {portal.FromMapX} {portal.FromMapY} {portal.ToMapId} {(sbyte)portal.Type} {portal.Id} {(portal.Disabled ? 1 : 0)}");
            }

            await CurrentMap.GetMapItems(Session);

            await UpdateCharacter();

            _ = SpawnMonsters();
        }

        private async Task SpawnMonsters()
        {
            foreach (var monster in CurrentMap.MonsterEntities.Where(x => x.MapId == Session.Player.CurrentMap.Template.Id))
            {
                await Session.SendPacket(monster.GenerateIn());
            }

            foreach (var npc in CurrentMap.NpcEntities.Where(x => x.MapId == Session.Player.CurrentMap.Template.Id))
            {
                await Session.SendPacket(npc.GenerateIn());
            }
            await SpawnShops();
        }

        private async Task SpawnShops()
        {
            await Session.Player.CurrentMap.GenerateShops(Session);
        }

        public async Task SetPlayerPacketsOnChangeMap()
        {
            await Session.SendPacket(Session.Player.Packets.GeneratePInfo());
            await Session.Player.Packets.GenerateCModePacket();
            await Session.Player.Packets.GenerateEquipmentPacket();
            await Session.SendPacket(Session.Player.Packets.GenerateLev());

            await Session.SendPacket(await Session.Player.Packets.GeneratePlayerMapInfo());
            await Session.SendPacket(Session.Player.Packets.GeneratePlayerMove());
            //titinfo
            await Session.SendPacket(Packets.GenerateMapInfo());
            // sc packet

            await Session.Player.CurrentMap.Broadcast(Session.Player.Packets.GenerateScale());
            await Session.Player.CurrentMap.Broadcast(Packets.GenerateGidx());
            await Session.Player.Packets.GenerateRsfpPacket();
            await Session.Player.Packets.GeneratePinitPacket();
            await Session.Player.Packets.GeneratePairyPacket();
            await Session.SendPacket(Packets.GenerateFood());
            await Session.Player.Packets.GenerateRepAndDignityPacket();
        }

        public async Task LeaveMap()
        {
            if (Session.Player.CurrentMap != null)
            {
                await Session.SendPacket($"c_map 0 {Session.Player.CurrentMap.Template.Id} 0");
                await Session.Player.Packets.MapOutPacket();
                await Session.Player.Packets.OutPacket();
                Session.Player.CurrentMap.Portals.Clear();
                Session.Player.CurrentMap.RemovePlayer(this);
            }
        }

        public async Task UpdateSpeed()
        {
            Speed = await Stats.Speed();
        }

        public async Task TpMove(short x, short y)
        {
            MapPosX = x;
            MapPosY = y;
            await CurrentMap.Broadcast($"tp 1 {Id} {x} {y} 0 0");
        }

        public async Task Die()
        {
            await CurrentMap.Broadcast($"die 1 {Id} 1 {Id}");
            await Session.SendPacket(Packets.GenerateStat());
        }

        public async Task GenerateCancel(byte type = 0, long id = 0)
        {
            await Session.SendPacket($"cancel {type} {id} -1");
        }

        public bool IsInRange(short x, short y, int range)
        {
            return Math.Abs(MapPosX - x) <= range && Math.Abs(MapPosY - y) <= range;
        }

        public string GetItemName(short id)
        {
            var item = WorldManager.GetItem(id).Translations.FirstOrDefault(x => x.Language == Session.Account.Language);
            string name = item is null ? "none" : item.Name;
            var bytes = Encoding.UTF8.GetBytes(name);
            var txt = Encoding.UTF8.GetString(bytes);
            return txt;
        }

        public async Task TransformToSpecialist(short model)
        {
            var sp = Inventory.GetEquippedItemFromSlot((byte)EquipmentType.SPECIALIST);
            if (sp is null) return;

            UsingSpecialist = true;
            Morph = model;
            SpUpgrade = sp.Upgrade;
            SpWings = (SpWings)sp.Rarity;
            await Packets.GenerateEffect(196);
            await Packets.GenerateCModePacket();
            await GetSpecialistSkills();
            await GetSkills();
            await GetActionBarList();
        }

        public async Task TransformToMount(short model)
        {
            IsUsingMount = true;
            SpecialMorph = model;
            await Packets.GenerateEffect(196);
            await Packets.GenerateCModePacket();
        }

        public async Task TransformToNormal()
        {
            if (UsingSpecialist)
            {
                var time = Session.Account.Rank > 0 ? 0 : 10;
                await Session.SendPacket($"sayi 1 {Id} 11 754 4 10 0 0 0");
                await Session.SendPacket($"sd {time}");

                UsingSpecialist = false;
                Morph = 0;
                LastUsedSpecialist = DateTime.Now;
                await Packets.GenerateCModePacket();
                await RemoveBuffs();
                await GetSkills();
                await GetActionBarList();
                await Session.SendPacket(Packets.GenerateStat());

                Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(async _ =>
                {
                    await Session.SendPacket("sd 0");
                    await Session.SendPacket($"sayi 1 {Id} 11 {(int)MessageId.SPECIALIST_REMAINING_COOLDOWN} 4 10 0 0 0");
                });
            }
        }

        public async Task TransformMountToNormal()
        {
            if (IsUsingMount)
            {
                IsUsingMount = false;
                SpecialMorph = 0;
                await Packets.GenerateCModePacket();
            }
        }

        public async Task SetSpecialistAdditionPoints(int additionPoint, int spPoint)
        {
            var maxAddPoints = 10000000;
            var maxSpPoints = 10000;
            if (additionPoint > maxAddPoints) additionPoint = maxAddPoints;
            if (spPoint > maxSpPoints) spPoint = maxSpPoints;

            Session.Player.SpecialistAddPts = additionPoint;
            Session.Player.SpecialistPts = spPoint;
            await Packets.GenerateSpAdditionPointsPacket();
            await UpdateCharacter();
        }

        public async Task AddSpecialistAdditionPoints(int additionPoint, int spPoint)
        {
            Session.Player.SpecialistAddPts += additionPoint;
            Session.Player.SpecialistPts += spPoint;
            await Packets.GenerateSpAdditionPointsPacket();
            await UpdateCharacter();
        }

        public async Task SendPacket(string packet) => await Session.SendPacket(packet);

        public void SetOrientation(int pX, int pY, int nX, int nY)
        {
            BeforeOrientation = Orientation;

            int dx = nX - pX;
            int dy = nY - pY;

            if (dx == 0 && dy > 0)
                Orientation = 2;
            else if (dx < 0 && dy == 0)
                Orientation = 3;
            else if (dx == 0 && dy < 0)
                Orientation = 0;
            else if (dx > 0 && dy == 0)
                Orientation = 1;
            else if (dx > 0 && dy > 0)
                Orientation = 6;
            else if (dx < 0 && dy > 0)
                Orientation = 7;
            else if (dx < 0 && dy < 0)
                Orientation = 4;
            else if (dx > 0 && dy < 0)
                Orientation = 5;
        }

        public async Task SaveCharacterOnDisconnect()
        {
            MapId = CurrentMap.Id;
            MapPosX = MapPosX;
            MapPosY = MapPosY;

            await Session.Player.Packets.OutPacket();

            // Eliminar personaje del mapa.
            CurrentMap.RemovePlayer(this);

            await CharacterDbHelper.UpdateAsync(this);
            var items = Session.Player.Inventory.Items;

            await CharacterDbHelper.UpdateItemsAsync(items);
            await CharacterDbHelper.UpdateActionBarListAsync(ActionBars, Id);

            Log.Information($"Character {Name} saved successfully.");
        }
    }
}