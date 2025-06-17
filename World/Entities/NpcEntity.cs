using Database.MapEntity;
using Database.Migrations.WorldDb;
using Database.ShopEntity;
using Database.World;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities.Components;
using World.GameWorld;
using World.Utils;

namespace World.Entities
{
    public class NpcEntity : Npc
    {
        public short VNum { get; set; }
        public short MapId { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public byte Dir { get; set; }
        public bool IsMoving { get; set; }
        public int DialogId { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentMana { get; set; }
        public int RespawnTime { get; set; }
        public List<Shop> Shop { get; set; }
        public int ChannelId { get; set; }
        public NpcMonster NpcInfo { get; set; }
        public GameEntity GameEntity { get; set; }
        public MonsterStatComponent Stats { get; set; }

        public NpcEntity(Npc npc, NpcMonster npcInfo)
        {
            VNum = npc.VNum;
            MapId = npc.MapId;
            Name = npc.Name;
            NpcId = npc.NpcId;
            X = npc.X;
            Y = npc.Y;
            DialogId = npc.DialogId;
            IsMoving = npc.IsMoving;
            Dir = npc.Dir;
            IsMoving = npc.IsMoving;
            NpcInfo = npcInfo;

            GameEntity = new GameEntity(this);
            Stats = new MonsterStatComponent(this);
        }

        public string GenerateIn()
        {
            var name = string.IsNullOrEmpty(Name) ? "-" : Name;
            return $"in 2 {VNum} {NpcId} {X} {Y} {Dir} {Stats.CurrentHealth} {Stats.CurrentMana} {DialogId} {RespawnTime} 0 {-1} {RespawnTime} {0} -1 {name.Replace(' ', '^')} 0 -1 0 0 0 0 0 0 0 0";
        }

        public async Task InitializeMove(MapInstance instance)
        {
            var map = await WorldManager.GetMap(MapId);
            Random rnd = new();
            if (IsMoving)
            {
                Observable.Interval(TimeSpan.FromSeconds(2.3)).Subscribe(async _ =>
                {
                    var nextStep = PathFindingMap.GetNextStep(new Coords(X, Y), new Coords((short)rnd.Next(X - 3, X + 3), (short)rnd.Next(Y - Y + 3)), 1);
                    X = nextStep.MapPosX;
                    Y = nextStep.MapPosY;
                    string packet = $"mv 2 {NpcId} {nextStep.MapPosX} {nextStep.MapPosY} 5";
                    await instance.Broadcast(packet);
                });
            }
        }
    }
}