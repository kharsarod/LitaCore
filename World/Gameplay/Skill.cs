using Database.World;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Gameplay.Interfaces;
using World.Network;

namespace World.Gameplay
{
    public class Skill : ISkill
    {
        private short VNum { get; set; }
        public byte Hit { get; set; }
        private ClientSession _session => _session;
        public DateTime LastUsed { get; set; } = DateTime.MinValue;
        public SkillData Ski { get; set; }
        public Skill(SkillData skill)
        {
            Ski = skill;
        }

        public bool CanUse()
        {
            return LastUsed.AddMilliseconds(Ski.Cooldown * 100) < DateTime.Now;
        }
    }
}
