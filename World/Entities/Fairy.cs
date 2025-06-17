using Database.Player;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network;

namespace World.Entities
{
    public class Fairy
    {
        private CharacterItem _item;

        public CharacterItem GetFairy(CharacterItem item)
        {
            _item = item;
            return _item;
        }

        public int GetRemainingMonsters()
        {
            return _item.FairyMonsterRemaining;
        }

        public void SetRemainingMonsters()
        {
            var remainingMonsters = 152 * _item.FairyLevel + 50;
            _item.FairyMonsterRemaining = remainingMonsters;
        }
    }
}