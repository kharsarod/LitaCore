using Configuration;
using Database.Item;
using Database.MapEntity;
using Database.World;
using Enum.Main.BCardEnum;
using Enum.Main.BuffEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities.Components;
using World.GameWorld;
using World.GameWorld.Objects;
using World.Utils;

namespace World.Entities
{
    public class MonsterEntity : Monster
    {
        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentMana { get; set; }
        public int Dialog { get; set; }
        public int RespawnTime { get; set; }
        public short MapX { get; set; }
        public short MapY { get; set; }
        public short CoordX { get; set; }
        public short CoordY { get; set; }
        public bool IsSitting { get; set; }
        public bool IsInvisible { get; set; }
        public int ChannelId { get; set; }
        public MapInstance Instance { get; set; }
        public MonsterStatComponent Stats { get; set; }
        public GameEntity GameEntity { get; set; }
        public NpcMonster NpcInfo { get; set; }
        public DateTime LastMove { get; set; }
        public bool IsMoving { get; set; }
        private static readonly Random rnd = new Random();
        public GameEntity FirstAttacker { get; set; }
        private readonly Random _rnd = new Random();

        public byte speed { get; set; }

        // Crear un Evento para manejar cuando el monstruo muere
        public event Action<MonsterEntity> OnMonsterDie;

        // Crear un evento para manejar cuando el monstruo es atacado
        public event Action<MonsterEntity, GameEntity> OnMonsterAttacked;
        private Monster Monster { get; set; }
        private IDisposable _moveTimer;


        public MonsterEntity(Monster monster, NpcMonster npc, MapInstance instance)
        {
            MonsterId = monster.MonsterId;
            VNum = monster.VNum;
            MapId = monster.MapId;
            Name = monster.Name;
            MapX = monster.MapX;
            MapY = monster.MapY;
            CoordX = MapX;
            CoordY = MapY;
            Position = monster.Position;
            Stats = new MonsterStatComponent(this);
            NpcInfo = npc;
            this.IsMoving = monster.IsMoving;
            RespawnTime = npc.RespawnTime;
            GameEntity = new GameEntity(this);
            Instance = instance;

            Monster = WorldManager.GetMonsterByVNum(VNum);
        }

        public async Task Initialize()
        {
            await Stats.Initialize();
            
            // Temporal Coords
            CoordX = MapX;
            CoordY = MapY;
        }

        public string GenerateIn()
        {
            var isSitting = IsSitting ? 1 : 0;
            var isInvisible = IsInvisible ? 1 : 0;
            return $"in 3 {VNum} {MonsterId} {MapX} {MapY} {Position} {Stats.HealthPercent()} {Stats.ManaPercent()} {0} 0 0 -1 {(NpcInfo.NoAggresiveIcon ? 1 : 0)} {isSitting} -1 - 0 -1 0 0 0 0 0 0 0 {isInvisible}";
        }

        // Manejar el evento de muerte del monstruo
        public void HandleMonsterDie()
        {
            OnMonsterDie?.Invoke(this);

            DropItem();

            var respawnTime = RespawnTime * 100;

            Observable.Timer(TimeSpan.FromMilliseconds(respawnTime)).Subscribe(async _ => Respawn());

        }

        public void HandleMonsterAttacked(GameEntity attacker)
        {
            OnMonsterAttacked?.Invoke(this, attacker);
            FirstAttacker = attacker;
        }

        // Drop item method
        public async Task DropItem()
        {
            var drops = WorldManager.Drops.Where(d => d.MonsterVNum == VNum).ToList();
            Random rnd = new Random();
            var dropRate = ConfigManager.WorldServerConfig.Rates.DropRate;

            foreach(var drop in drops)
            {
                var chance = rnd.Next(0, 100000);

                if (chance <= drop.Chance * dropRate)
                {
                    short x = (short)rnd.Next(MapX - 2, MapX + 2), y = (short)rnd.Next(MapY - 2, MapY + 2);
                    if (PathFindingMap.IsBlockedZone(rnd.Next(MapX - 2, MapX + 2), rnd.Next(MapY - 2, MapY + 2), Instance.Template.MapGrid))
                    {
                        do
                        {
                            x = (short)rnd.Next(MapX - 2, MapX + 2);
                            y = (short)rnd.Next(MapY - 2, MapY + 2);
                        } while (PathFindingMap.IsBlockedZone(x, y, Instance.Template.MapGrid));
                    }
                    MapItem dropItem = new MapItem(Instance.Template.Id, x, y, drop.VNum, (short)drop.Amount, FirstAttacker, Instance);
                    
                    await Instance.Broadcast($"drop {dropItem.ItemId} {dropItem.Id} {dropItem.X} {dropItem.Y} {dropItem.Amount} 0 {FirstAttacker.Id}");
                    Instance.Items.Add(dropItem);
                }
            }
        }

        private void Respawn()
        {
            if (Stats.CurrentHealth <= 0)
            {
                Stats = new MonsterStatComponent(this);
                Stats.Initialize();
                Stats.CurrentHealth = NpcInfo.MaxHP;
                Stats.CurrentMana = NpcInfo.MaxMP;

                GameEntity = new GameEntity(this);

                Instance.Broadcast(GenerateIn());
                Instance.MonsterEntities.Add(this);
            }
        }

        public void GetDamage(int amount, GameEntity? attacker = null)
        {
            HandleMonsterAttacked(attacker);

            Stats.CurrentHealth -= amount;

            if (Stats.CurrentHealth <= 0)
            {
                if (attacker is not null && attacker.IsPlayer)
                {
                    attacker.Player!.HandleMonsterKilled(this);
                }

                Stats.CurrentHealth = 0;

                Die();
            }
        }

        public async Task Die()
        {
            Stats.CurrentHealth = 0;
            Stats.CurrentMana = 0;
            HandleMonsterDie();
            Instance.MonsterEntities.Remove(this);
            await Instance.Broadcast($"die 3 {MonsterId} 3 {MonsterId}");
        }

        public async Task Move()
        {
            Coords actualCoords = new Coords(CoordX, CoordY);

            Coords wanderCoords = new Coords((short)rnd.Next(MapX - 2, MapX + 2), (short)rnd.Next(MapY - 2, MapY + 2));

            if (await GameEntity!.HasCantMovementBuff())
            {
                return;
            }

            if (PathFindingMap.IsBlockedZone(wanderCoords.MapPosX, wanderCoords.MapPosY, Instance.Template.MapGrid)
                || !PathFindingMap.IsWalkable(wanderCoords.MapPosX, wanderCoords.MapPosY, Instance.Template.MapGrid, Instance.Template.Width, Instance.Template.Height))
            {
                return;
            }

            if (PathFindingMap.IsWalkable(wanderCoords.MapPosX, wanderCoords.MapPosY, Instance.Template.MapGrid, Instance.Template.Width, Instance.Template.Height))
            {
                await Instance.Broadcast($"mv 3 {MonsterId} {wanderCoords.MapPosX} {wanderCoords.MapPosY} {NpcInfo.Speed}");
                CoordX = wanderCoords.MapPosX;
                CoordY = wanderCoords.MapPosY;
            }
        }

    }
}