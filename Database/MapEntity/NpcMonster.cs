﻿using Enum.Main.MonsterEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.MapEntity
{
    public class NpcMonster
    {
        public int Id { get; set; }

        public short VNum { get; set; }

        public short ItemAmountRequired { get; set; }

        public byte AttackClass { get; set; }

        public byte AttackUpgrade { get; set; }

        public byte BasicArea { get; set; }

        public short BasicCooldown { get; set; }

        public string Name { get; set; }

        public byte BasicRange { get; set; }

        public short BasicSkill { get; set; }

        public bool Catch { get; set; }

        public short CloseDefence { get; set; }

        public short Concentrate { get; set; }

        public byte CriticalChance { get; set; }

        public short CriticalRate { get; set; }

        public short DamageMaximum { get; set; }

        public short DamageMinimum { get; set; }

        public short DarkResistance { get; set; }

        public short DefenceDodge { get; set; }

        public byte DefenceUpgrade { get; set; }

        public short DistanceDefence { get; set; }

        public short DistanceDefenceDodge { get; set; }

        public byte Element { get; set; }

        public short ElementRate { get; set; }

        public short FireResistance { get; set; }

        public byte HeroLevel { get; set; }

        public int HeroXp { get; set; }

        public bool IsHostile { get; set; }

        public int JobXP { get; set; }

        public byte Level { get; set; }

        public short LightResistance { get; set; }

        public short MagicDefence { get; set; }

        public int MaxHP { get; set; }

        public int MaxMP { get; set; }

        public MonsterType MonsterType { get; set; }

        public bool NoAggresiveIcon { get; set; }

        public byte NoticeRange { get; set; }

        public short OriginalVNum { get; set; }

        public byte Race { get; set; }

        public byte RaceType { get; set; }

        public int RespawnTime { get; set; }

        public byte Speed { get; set; }

        public short VNumRequired { get; set; }

        public short WaterResistance { get; set; }

        public int XP { get; set; }
    }
}