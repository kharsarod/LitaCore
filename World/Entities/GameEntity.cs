using Database.Helper;
using Database.Migrations.WorldDb;
using Database.Player;
using Database.World;
using Enum.Main.BCardEnum;
using Enum.Main.MessageEnum;
using GameWorld;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network;
using World.Utils;
using static System.Collections.Specialized.BitVector32;

namespace World.Entities
{
    public class GameEntity
    {
        public Player? Player { get; set; }
        public MonsterEntity? Monster { get; set; }
        public NpcEntity? Npc { get; set; }

        private ClientSession Session => Player?.Session;

        public bool IsPlayer => Player != null;
        public bool IsMonster => Monster != null;
        public bool IsNpc => Npc != null;

        public int Id => IsPlayer ? Player!.Id : Monster!.MonsterId;
        public short MapX => IsPlayer ? Player!.MapPosX : IsNpc ? Npc!.X : Monster!.CoordX;

        public short MapY => IsPlayer ? Player!.MapPosY : IsNpc ? Npc!.Y : Monster!.CoordY;

        public int Health => IsPlayer ? Player!.Stats.CurrentHealth : IsNpc ? Npc!.Stats.CurrentHealth : Monster!.Stats.CurrentHealth;
        public int Mana => IsPlayer ? Player!.Stats.CurrentMana : IsNpc ? Npc!.Stats.CurrentMana : Monster!.Stats.CurrentMana;

        public List<BuffData> Buffs { get; set; } = new();

        public GameEntity(Player player)
        {
            Player = player;
        }

        public GameEntity(MonsterEntity monster)
        {
            Monster = monster;
        }

        public GameEntity(NpcEntity npc)
        {
            Npc = npc;
        }

        public async Task ReceiveDamage(int damage, GameEntity? attacker = null)
        {
            if (IsPlayer) await Player!.GetDamage(damage);
            else if (IsMonster) Monster!.GetDamage(damage, attacker);
        }

        public List<MonsterEntity> GetNearestMonsters(byte range)
        {
            List<MonsterEntity> monstersInRange = new List<MonsterEntity>();
            var monsters = IsPlayer ? Player.CurrentMap.MonsterEntities.Where(x => x.MapId == Player.CurrentMap.Template.Id).ToList() :
                IsMonster ? Monster!.Instance.MonsterEntities.Where(x => x.MapId == Monster.MapId).ToList() : null;

            foreach (var monster in monsters.Where(x => Math.Abs(x.CoordX - MapX) <= range && Math.Abs(x.CoordY - MapY) <= range))
            {
                monstersInRange.Add(monster);
            }

            return monstersInRange;
        }

        // Get nearest players

        public List<Player> GetNearestPlayers(byte range)
        {
            List<Player> playersInRange = new List<Player>();
            var players = IsPlayer ? Player.CurrentMap.Players : IsMonster ? Monster.Instance.Players : null;
            foreach (var player in players.Where(x => Math.Abs(x.MapPosX - MapX) <= range && Math.Abs(x.MapPosY - MapY) <= range))
            {
                playersInRange.Add(player);
            }
            return playersInRange;
        }

        public async Task AddBuff(short id)
        {
            var buff = WorldManager.Getbuff(id);
            if (buff == null)
            {
                Log.Warning("A buff is null.");
                return;
            }

            Buffs.Add(buff);

            var buffTiming = buff.DurationMs;
            string buffPacket = $"bf {(IsMonster ? 3 : (IsPlayer ? 1 : 0))} {Id} 0.{buff.BuffId}.{buffTiming} {buff.Level}";
            if (IsMonster)
            {
                await Monster.Instance.Broadcast(buffPacket);
            }
            else if (IsPlayer)
            {
                await Player.CurrentMap.Broadcast(buffPacket);
            }

            var buffBCards = await WorldDbHelper.LoadBuffBCardsByBuffIdAsync(buff.BuffId);
            var bCardShadowAppear = buffBCards?.FirstOrDefault(x => x.Type == BCardType.ShadowAppear && x.SubType == (BCardEffect)51);
            if (bCardShadowAppear != null)
            {
                if (bCardShadowAppear.Type == BCardType.ShadowAppear && bCardShadowAppear.SubType == (BCardEffect)51)
                {
                    if (IsMonster)
                    {
                        await Monster.Instance.Broadcast($"guri 0 1 {Id} {bCardShadowAppear.FirstEffectValue} {bCardShadowAppear.SecondaryEffectValue}");
                    }
                }
            }

            var cantMoveBuff = buffBCards.FirstOrDefault(x => x.Type == BCardType.Movement && x.SubType == (BCardEffect)11 && x.FirstEffectValue == 0);
            var cantAttackBuff = buffBCards.FirstOrDefault(x => x.Type == BCardType.SPECIAL_ATTACK && x.SubType == (BCardEffect)11 && x.FirstEffectValue == 0);


            if (IsMonster)
            {
                await Monster.Instance.Broadcast($"eff 3 {Id} {buff.EffectId}");
            }
            else if (IsPlayer)
            {
                await Player.CurrentMap.Broadcast($"eff 1 {Id} {buff.EffectId}");
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
                                if (IsMonster)
                                {
                                    await Monster.Instance.Broadcast($"guri 0 1 {Id} 0 0"); // Elimina el efecto de sombra
                                }
                            }
                        }
                        Buffs.Remove(buff);
                        if (IsPlayer)
                        {
                            await Player.CurrentMap.Broadcast($"bf 1 {Id} 0.{buff.BuffId}.0 {buff.Level}");
                        }
                        else if (IsMonster)
                        {
                            await Monster.Instance.Broadcast($"bf 3 {Id} 0.{buff.BuffId}.0 {buff.Level}");
                        }
                    }
                });
            }
        }

        public bool HasBuff(short id)
        {
            return Buffs.Any(x => x.BuffId == id);
        }

        public async Task<bool> HasCantMovementBuff()
        {
            foreach(var buff in Buffs)
            {
                var buffBCards = WorldDbHelper.LoadBuffBCardsByBuffIdAsync(buff.BuffId);
                var cantMoveBuff = await buffBCards;
                var _buff = cantMoveBuff.FirstOrDefault(x => x.Type == BCardType.Movement && x.SubType == (BCardEffect)11 && x.FirstEffectValue == 0);
                if (_buff != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> HasCantAttackBuff()
        {
            foreach (var buff in Buffs)
            {
                var buffBCards = WorldDbHelper.LoadBuffBCardsByBuffIdAsync(buff.BuffId);
                var cantAttackBuff = await buffBCards;
                var _buff = cantAttackBuff.FirstOrDefault(x => x.Type == BCardType.SPECIAL_ATTACK && x.SubType == (BCardEffect)11 && x.FirstEffectValue == 0);
                if (_buff != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task RemoveBuff(short id)
        {
            var buff = Buffs.FirstOrDefault(x => x.BuffId == id);
            if (buff != null)
            {
                Buffs.Remove(buff);
                if (IsPlayer)
                {
                    await Player.CurrentMap.Broadcast($"bf 1 {Id} 0.{buff.BuffId}.0 {buff.Level}");
                }
                else
                {
                    await Monster.Instance.Broadcast($"bf 1 {Id} 0.{buff.BuffId}.0 {buff.Level}");
                }
            }
        }
    }
}