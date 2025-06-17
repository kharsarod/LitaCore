using AutoMapper.Configuration.Annotations;
using Database.Helper;
using Database.Item;
using Database.MapEntity;
using Database.Player;
using Enum.Main.BCardEnum;
using Enum.Main.CharacterEnum;
using Enum.Main.ChatEnum;
using Enum.Main.EntityEnum;
using GameWorld;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.BCards;
using World.Gameplay.BCards.Handler;
using World.Gameplay.Interfaces;
using World.Gameplay.Script;
using World.Network;
using static System.Collections.Specialized.BitVector32;

namespace World.Gameplay
{
    public static class Attack
    {
        public static async Task HandleHit(ClientSession session, int castId, int targetId, Entity userType)
        {
            CharacterSkill getSkill = null;
            if (session.Player.UsingSpecialist)
            {
                getSkill = session.Player.SpecialistSkills.FirstOrDefault(x => x.CastId == castId);
            }
            else
            {
                getSkill = session.Player.Skills.FirstOrDefault(x => x.CastId == castId);
            }
            var _skill = WorldManager.GetSkill(getSkill.VNum);
            var skill = new Skill(_skill);
            

            if (skill is null || getSkill is null) return;

            GameEntity? targetEntity = null;

            if (skill == null)
            {
                Log.Error("A skill is null.");
                await session.Player.GenerateCancel();
                return;
            }

            var bCard = await WorldDbHelper.LoadBCardBySkillVNumAsync(skill.Ski.SkillVNum);
            IBCard handler = null;
            var _bCards = await WorldManager.GetBCardsFromSkill(skill.Ski.SkillVNum);

            var playerEntity = WorldManager.GetPlayerById(targetId);

            if (playerEntity != null)
            {
                targetEntity = playerEntity.Player.GameEntity;
            }
            else
            {
                var monsterEntity = session.Player.CurrentMap.MonsterEntities.FirstOrDefault(x => x.MonsterId == targetId);

                if (monsterEntity != null)
                {
                    targetEntity = monsterEntity.GameEntity;
                }
            }

            if (targetEntity == null)
            {
                Log.Error("TargetEntity is null");
                return;
            }

            var x = targetEntity.MapX;
            var y = targetEntity.MapY;
            var dmgModifier = await session.Player.Stats.GetDamageModifier(bCard, skill.Ski.SkillVNum);
            var percent = await session.Player.Stats.HealthPercent();

            var packet = $"su 1 {session.Player.Id} {(byte)userType} {targetId} {skill.Ski.SkillVNum} {skill.Ski.Cooldown} {skill.Ski.AttackAnimation}" +
                $" {skill.Ski.Effect} {x} {y} 1 {percent} {dmgModifier} 0 {skill.Ski.SkillType - 1}";

            session.Player.TargetEntity = targetEntity;


            if (!skill.CanUse())
            {
                skill.LastUsed = DateTime.Now;
                await session.Player.GenerateCancel();
                return;
            }

              Observable.Timer(TimeSpan.FromMilliseconds(skill.Ski.Cooldown * 100)).Subscribe(async _ =>
              {
                  await session.SendPacket($"sr {skill.Ski.CastId}");
              });


            if (targetEntity.Health > 0 && session.Player.IsInRange(targetEntity.MapX, targetEntity.MapY, skill.Ski.Range))
            {
                await session.Player.CurrentMap.Broadcast($"ct 1 {session.Player.Id} {(byte)userType} {targetId} {skill.Ski.CastAnimation} {skill.Ski.CastEffect} {skill.Ski.SkillVNum}");
                foreach (var bCards in _bCards)
                {
                    handler = BCardHandlerFactory.Create(bCards);
                    handler?.Apply(session.Player, skill.Ski.SkillVNum);
                }
            }
            else
            {
                await session.Player.GenerateCancel(0, session.Player.Id);
                return;
            }

            if (skill.Ski.SkillVNum == 1085) // Volcano Rugido
            {
                await session.Player.TpMove(targetEntity.MapX, targetEntity.MapY);
            }

            if (targetEntity.Health <= 0)
            {
                await session.Player.GenerateCancel();
                return;
            }

            Console.WriteLine($"Casted: {skill.Ski.TargetType} - {skill.Ski.HitType}");

            if (skill.Ski.CastEffect != 0)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(skill.Ski.CastTime * 100);
                    if (skill.Ski.TargetType == 1 && skill.Ski.HitType == 1) // skill de AoE
                    {
                        var onFirstPacketAttack = $"su 1 {session.Player.Id} 1 {session.Player.Id} {skill.Ski.SkillVNum} {skill.Ski.Cooldown} {skill.Ski.AttackAnimation}" +
                            $" {skill.Ski.Effect} 0 0 1 {percent} 0 -2 {skill.Ski.SkillType - 1}";

                        await session.Player.CurrentMap.Broadcast(packet);
                        foreach (var monster in session.Player.GameEntity.GetNearestMonsters(skill.Ski.TargetRange))
                        {

                            packet = $"su 1 {session.Player.Id} 3 {monster.MonsterId} {skill.Ski.SkillVNum} {skill.Ski.Cooldown} {skill.Ski.AttackAnimation}" +
                                $" {skill.Ski.Effect} 0 0 1 0 {dmgModifier} 5 {skill.Ski.SkillType - 1}";

                            session.Player.TargetEntity = monster.GameEntity;

                            foreach (var bCards in _bCards)
                            {
                                handler = BCardHandlerFactory.Create(bCards);
                                handler?.Apply(session.Player, skill.Ski.SkillVNum);
                            }

                            await session.Player.CurrentMap.Broadcast(packet);
                            await monster.GameEntity.ReceiveDamage(dmgModifier, session.Player.GameEntity);
                            if (targetEntity.IsMonster && targetEntity.Health <= 0)
                            {
                                await session.Player.HandleMonsterKilled(monster);
                            }
                        }
                    }
                    else if (skill.Ski.TargetType == 0 && skill.Ski.HitType == 1)
                    {
                        if (!session.Player.IsInRange(targetEntity.MapX, targetEntity.MapY, skill.Ski.Range))
                        {
                            await session.Player.GenerateCancel(); // ??? Esto no funciona como es.
                            return;
                        }
                        else
                        {
                            if (targetEntity is not null)
                            {
                                session.Player.TargetEntity = targetEntity;
                                await session.Player.CurrentMap.Broadcast(packet);
                                await targetEntity.ReceiveDamage(dmgModifier, session.Player.GameEntity);
                            }
                        }
                        foreach (var monster in targetEntity.GetNearestMonsters(skill.Ski.TargetRange)) // Si es el targetEntity principal ignorarlo y seguir con los demás.
                        {
                            session.Player.TargetEntity = monster.GameEntity;
                            if (monster.MonsterId == targetEntity.Id) continue; // Ignorar el targetEntity principal.

                            packet = $"su 1 {session.Player.Id} 3 {monster.MonsterId} {skill.Ski.SkillVNum} {skill.Ski.Cooldown} {skill.Ski.AttackAnimation}" +
                             $" {skill.Ski.Effect} {monster.GameEntity.MapX} {monster.GameEntity.MapY} 1 {percent} {dmgModifier} 0 {skill.Ski.SkillType - 1}";
                            await session.Player.CurrentMap.Broadcast(packet);
                            await monster.GameEntity.ReceiveDamage(dmgModifier, session.Player.GameEntity);
                            if (targetEntity.IsMonster && targetEntity.Health <= 0)
                            {
                                await session.Player.HandleMonsterKilled(monster);
                            }
                        }
                    }
                    else if (skill.Ski.TargetType == 2 && skill.Ski.HitType == 0) // Habilidades hacia el caster.
                    {
                        await session.Player.CurrentMap.Broadcast(packet);
                    }
                    else if (skill.Ski.TargetType == 1 && skill.Ski.HitType == 0) // Habilidades inofensivas (Buffs) hacia otros jugadores o caster.
                    {
                        await session.Player.CurrentMap.Broadcast(packet);
                    }
                    else if (skill.Ski.TargetType == 1 && skill.Ski.HitType == 2) // otro buff?
                    {
                        await session.Player.CurrentMap.Broadcast(packet);
                    }
                    else
                    {
                        await session.Player.CurrentMap.Broadcast(packet);
                        await targetEntity.ReceiveDamage(dmgModifier, session.Player.GameEntity);
                    }
                    foreach (var script in ScriptLoader.GetScriptsForPlayer(session.Player))
                    {
                        await script.OnUseSkill(skill.Ski);
                    }
                });
            }
        }
    }
}