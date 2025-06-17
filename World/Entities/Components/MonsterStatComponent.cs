using Database.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Entities.Components
{
    public class MonsterStatComponent
    {
        private readonly MonsterEntity _monster;
        private readonly NpcEntity _npc;

        public MonsterStatComponent(MonsterEntity monster)
        {
            _monster = monster;
        }

        public MonsterStatComponent(NpcEntity npc)
        {
            _npc = npc;
        }

        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
        public int CurrentHealth { get; set; }
        public int CurrentMana { get; set; }

        public async Task Initialize()
        {
            if (_npc != null)
            {
                MaxHealth = _npc.NpcInfo.MaxHP;
                MaxMana = _npc.NpcInfo.MaxMP;
                CurrentHealth = MaxHealth;
                CurrentMana = MaxMana;
                return;
            }

            if (_monster != null)
            {
                MaxHealth = _monster.NpcInfo.MaxHP;
                MaxMana = _monster.NpcInfo.MaxMP;
                CurrentHealth = MaxHealth;
                CurrentMana = MaxMana;
                return;
            }
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        public int GetExpForPlayer()
        {
            return _monster != null ? _monster.NpcInfo.XP : 0;
        }

        public int GetJExpForPlayer()
        {
            return _monster != null ? _monster.NpcInfo.JobXP : 0;
        }

        public int HealthPercent()
        {
            return CurrentHealth * 100 / MaxHealth;
        }

        public int ManaPercent()
        {
            return CurrentMana * 100 / MaxMana;
        }
    }
}