using Enum.Main.ChatEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class SelectedTargetHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("ncif", HandleSelectedTarget);
        }

        public static async Task HandleSelectedTarget(ClientSession session, string[] parts)
        {
            var targetType = int.Parse(parts[2]);
            var targetId = int.Parse(parts[3]);

            session.Player.TargetMonsterId = targetId;

            var targetSession = WorldManager.GetPlayerById(targetId);
            var targetMonster = session.Player.CurrentMap.MonsterEntities.Find(x => x.MonsterId == targetId);

            if (targetType == 1 && targetSession is not null)
            {
                var player = targetSession.Player;
                var maxHp = await player.Stats.MaxHealth();
                var maxMp = await player.Stats.MaxMana();
                var buffs = targetSession.Player.Buffs.Aggregate("", (current, buff) => current + $" {buff.BuffId}.{buff.Level}");
                await session.Player.SendPacket($"st 1 {player.Id} {player.Level} {player.HeroLevel} {player.Stats.HealthPercent} {player.Stats.ManaPercent} {player.Stats.CurrentHealth} {player.Stats.CurrentMana} {maxHp} {maxMp} -1 {buffs}");
            }
            else if (targetType == 3 && targetMonster is not null)
            {
                var monster = WorldManager.NpcMonsters.FirstOrDefault(x => x.VNum == targetMonster.VNum);
                if (monster is null) return;

                var buffs = targetMonster.GameEntity.Buffs.Aggregate("", (current, buff) => current + $" {buff.BuffId}.{buff.Level}");
                await session.SendPacket($"st 3 {targetId} {monster.Level} {monster.HeroLevel} {targetMonster.Stats.HealthPercent()} {targetMonster.Stats.ManaPercent()} {targetMonster.Stats.CurrentHealth} {targetMonster.Stats.CurrentMana} {targetMonster.Stats.MaxHealth} {targetMonster.Stats.MaxMana} 25 {buffs}");
            }
            else if (targetType == 2)
            {
                var npc = session.Player.CurrentMap.NpcEntities.FirstOrDefault(x => x.NpcId == targetId);
                if (npc is null) return;
                await session.SendPacket($"st 2 {targetId} {npc.NpcInfo.Level} {npc.NpcInfo.HeroLevel} {npc.Stats.HealthPercent()} {npc.Stats.ManaPercent()} {npc.Stats.CurrentHealth} {npc.Stats.CurrentMana} {npc.Stats.MaxHealth} {npc.Stats.MaxMana} 0");
            }
        }
    }
}