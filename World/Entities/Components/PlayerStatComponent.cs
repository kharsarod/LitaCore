using Database.Helper;
using Database.Item;
using Database.Player;
using Database.World;
using Enum.Main.BCardEnum;
using Enum.Main.CharacterEnum;
using Enum.Main.ItemEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using World.Gameplay;

namespace World.Entities.Components
{
    public class PlayerStatComponent
    {
        private readonly Player _player;

        public PlayerStatComponent(Player player)
        {
            _player = player;
        }

        public int BaseHealth { get; set; }
        public int CurrentHealth { get; set; }

        public int ExperienceToUp { get; set; }
        public int JobExperienceToUp { get; set; }

        public bool IsAlive()
        {
            if (CurrentHealth > 0)
                return true;
            return false;
        }

        public int BaseMana { get; set; }
        public int CurrentMana { get; set; }

        public int BaseDamage { get; set; }
        public int WeaponMinimumDamage { get; set; }
        public int WeaponMaximumDamage { get; set; }
        public int SecondaryMinimumDamage { get; set; }
        public int SecondaryMaximumDamage { get; set; }
        public int CloseDefense { get; set; }
        public int RangeDefense { get; set; }
        public int MagicDefense { get; set; }
        public int WaterResistance { get; set; }
        public int FireResistance { get; set; }
        public int DarkResistance { get; set; }
        public int LightResistance { get; set; }
        public int BonusDamage { get; set; }

        public async Task<int> MaxHealth()
        {
            return BaseHealth + await GetHealthModifiers();
        }

        public async Task<int> MaxMana()
        {
            return BaseMana + await GetManaModifiers();
        }

        public async Task<byte> Speed()
        {
            return (byte)(_player.BaseSpeed[_player.Class] + await GetMovementSpeedModifiers());
        }

        public async Task<int> HealthPercent()
        {
            var max = await MaxHealth();
            if (CurrentHealth > max)
                CurrentHealth = max;
            return CurrentHealth * 100 / await MaxHealth();
        }

        public async Task<int> ManaPercent()
        {
            return CurrentMana * 100 / await MaxMana();
        }

        public void Initialize()
        {
            loadHPData();
            loadMPData();
            loadEXPData();
            CurrentHealth = _player.Health;
            CurrentMana = _player.Mana;
        }

        public async Task<List<BCard>> GetItemBCards(int itemId)
        {
            var item = WorldManager.GetItem(itemId);
            return await WorldDbHelper.LoadBCardsByItemId(item.Id);
        }

        public async Task<List<BCard>> GetEquippedItemsBCards()
        {
            var items = _player.Inventory.GetEquippedItems();
            var bCards = new List<BCard>();
            foreach (var item in items)
            {
                bCards.AddRange(await GetItemBCards(item.ItemId));
            }
            return bCards;
        }

        public async Task<List<BCard>> GetSkillBCards(short skillId)
        {
            var skill = WorldManager.GetSkill(skillId);
            return await WorldDbHelper.LoadSkillBCardsBySkillVNumAsync(skill.SkillVNum);
        }

        public async Task<int> GetDamageModifier(BCard bCard, short skillId)
        {
            var bEffs = BCardEffectDefinitions.BCardMapping.BCardEffects;
            var fairy = _player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.FAIRY);
            Random rng = new Random();
            int bonus = 0;
            var mainWeapon = _player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.MAIN_WEAPON);

            if (mainWeapon is not null)
            {
                WeaponMinimumDamage = WorldManager.GetItem(mainWeapon.ItemId).DamageMinimum + mainWeapon.MinDmg;
                WeaponMaximumDamage = WorldManager.GetItem(mainWeapon.ItemId).DamageMaximum + mainWeapon.MaxDmg;
            }
            int dmg = rng.Next(WeaponMinimumDamage, WeaponMaximumDamage + 1);
            foreach (var b in await GetEquippedItemsBCards())
            {
                if (bEffs.TryGetValue(b.SubType, out var bCardTypes) && bCardTypes.Contains(b.Type))
                {
                    // create a collection for exclude bcard types.

                    var excludedBCardTypes = new List<BCardType> { BCardType.HealthMana, BCardType.SPECIAL_ATTACK };

                    if (excludedBCardTypes.Contains(b.Type)) continue;

                    switch (b.SubType)
                    {
                        case (BCardEffect)11:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)12:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)21:
                            // Increase bonus with b.firsteffectvalue including % of damage.
                            bonus += (int)(dmg * (b.FirstEffectValue / 100.0));
                            Console.WriteLine($"Damage modifier: {dmg} * {b.FirstEffectValue / 100.0} = {dmg * (b.FirstEffectValue / 100.0)}");
                            break;

                        case (BCardEffect)22:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)31:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)32:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)41:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)42:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)51:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)52:
                            bonus += b.FirstEffectValue;
                            break;
                    }
                }
            }

            foreach (var b in await GetSkillBCards(skillId))
            {
                if (bEffs.TryGetValue(b.SubType, out var bCardTypes) && bCardTypes.Contains(b.Type))
                {
                    // create a collection for exclude bcard types.

                    var excludedBCardTypes = new List<BCardType> { BCardType.HealthMana, BCardType.SPECIAL_ATTACK };
                    if (excludedBCardTypes.Contains(b.Type)) continue;

                    switch (b.SubType)
                    {
                        case (BCardEffect)11:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)12:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)21:
                            bonus += b.FirstEffectValue;
                            if (fairy != null)
                            {
                                // Increase damage bonus by % of fairy level
                                bonus += bCard.FirstEffectValue * fairy.FairyLevel / 100;
                            }
                            break;

                        case (BCardEffect)22:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)31:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)32:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)41:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)42:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)51:
                            bonus += b.FirstEffectValue;
                            break;

                        case (BCardEffect)52:
                            bonus += b.FirstEffectValue;
                            break;
                    }
                }
            }

            foreach (var buff in _player.Buffs.ToList())
            {
                if (buff is null) continue;
                var bCards = await WorldManager.GetBCardsFromBuff(buff.BuffId);
                foreach (var b in bCards.Where(x => x.Type == BCardType.AttackPower || x.Type == (BCardType)7 || x.Type == BCardType.Defenses))
                {
                    if (bEffs.TryGetValue(b.SubType, out var bCardTypes) && bCardTypes.Contains(b.Type))
                    {
                        switch (b.SubType)
                        {
                            case (BCardEffect)11:
                                if (b.IsLevelScaled)
                                {
                                    bonus += _player.Level * b.FirstEffectValue;
                                }
                                else
                                {
                                    bonus += b.FirstEffectValue;
                                }
                                break;
                            case (BCardEffect)12:
                                bonus += b.FirstEffectValue;
                                break;
                            case (BCardEffect)21:
                                if (b.IsLevelScaled)
                                {
                                    bonus += _player.Level * b.FirstEffectValue ;
                                }
                                // Increase bonus with b.firsteffectvalue including % of damage.
                                bonus += (int)(dmg * (b.FirstEffectValue / 100.0));
                                break;
                            case (BCardEffect)22:
                                bonus += b.FirstEffectValue;
                                break;
                            case (BCardEffect)31:
                                bonus += b.FirstEffectValue;
                                break;
                            case (BCardEffect)32:
                                bonus += b.FirstEffectValue;
                                break;
                            case (BCardEffect)41:
                                bonus += b.FirstEffectValue;
                                break;
                            case (BCardEffect)42:
                                bonus += b.FirstEffectValue;
                                break;
                            case (BCardEffect)51:
                                bonus += b.FirstEffectValue;
                                break;
                            case (BCardEffect)52:
                                bonus += b.FirstEffectValue;
                                break;
                        }
                    }
                }
            }

            return dmg + bonus;
        }

        public void loadHPData()
        {
            int[,] _hp;

            _hp = new int[5, 256];

            // Adventurer HP
            for (int i = 1; i < _hp.GetLength(1); i++)
            {
                _hp[(int)ClassId.Adventurer, i] = (int)((1 / 2.0 * i * i) + (31 / 2.0 * i) + 205);
            }

            // Swordsman HP
            for (int i = 0; i < _hp.GetLength(1); i++)
            {
                int j = 16;
                int hp = 946;
                int inc = 85;
                while (j <= i)
                {
                    if (j % 5 == 2)
                    {
                        hp += inc / 2;
                        inc += 2;
                    }
                    else
                    {
                        hp += inc;
                        inc += 4;
                    }
                    ++j;
                }
                _hp[(int)ClassId.Swordsman, i] = hp;
            }

            // Magician HP
            for (int i = 0; i < _hp.GetLength(1); i++)
            {
                _hp[(int)ClassId.Mage, i] = (int)(((((i + 15) * (i + 15)) + i + 15.0) / 2.0) - 465 + 550);
            }

            // Archer HP
            for (int i = 0; i < _hp.GetLength(1); i++)
            {
                int hp = 680;
                int inc = 35;
                int j = 16;
                while (j <= i)
                {
                    hp += inc;
                    ++inc;
                    if (j % 10 == 1 || j % 10 == 5 || j % 10 == 8)
                    {
                        hp += inc;
                        ++inc;
                    }
                    ++j;
                }
                _hp[(int)ClassId.Archer, i] = hp;
            }

            // MARTIAL ARTIST HP
            for (int i = 0; i < _hp.GetLength(1); i++)
            {
                int j = 16;
                int hp = 946;
                int inc = 85;
                while (j <= i)
                {
                    if (j % 5 == 2)
                    {
                        hp += inc / 2;
                        inc += 2;
                    }
                    else
                    {
                        hp += inc;
                        inc += 4;
                    }
                    ++j;
                }
                _hp[(int)ClassId.MartialArtist, i] = hp;
            }

            BaseHealth = _hp[(int)_player.Class, _player.Level];
        }

        public void loadMPData()
        {
            int[,] _mp;
            _mp = new int[5, 257];

            // ADVENTURER MP
            _mp[(int)ClassId.Adventurer, 0] = 60;
            int baseAdventurer = 9;
            for (int i = 1; i < _mp.GetLength(1); i += 4)
            {
                _mp[(int)ClassId.Adventurer, i] = _mp[(int)ClassId.Adventurer, i - 1] + baseAdventurer;
                _mp[(int)ClassId.Adventurer, i + 1] = _mp[(int)ClassId.Adventurer, i] + baseAdventurer;
                _mp[(int)ClassId.Adventurer, i + 2] = _mp[(int)ClassId.Adventurer, i + 1] + baseAdventurer;
                baseAdventurer++;
                _mp[(int)ClassId.Adventurer, i + 3] = _mp[(int)ClassId.Adventurer, i + 2] + baseAdventurer;
                baseAdventurer++;
            }

            // SWORDSMAN MP
            for (int i = 1; i < _mp.GetLength(1) - 1; i++)
            {
                _mp[(int)ClassId.Swordsman, i] = _mp[(int)ClassId.Adventurer, i];
            }

            // ARCHER MP
            for (int i = 0; i < _mp.GetLength(1) - 1; i++)
            {
                _mp[(int)ClassId.Archer, i] = _mp[(int)ClassId.Adventurer, i + 1];
            }

            // MAGICIAN MP
            for (int i = 0; i < _mp.GetLength(1) - 1; i++)
            {
                _mp[(int)ClassId.Mage, i] = 3 * _mp[(int)ClassId.Adventurer, i];
            }

            // MARTIAL ARTIST MP
            for (int i = 1; i < _mp.GetLength(1) - 1; i++)
            {
                _mp[(int)ClassId.MartialArtist, i] = _mp[(int)ClassId.Adventurer, i];
            }

            BaseMana = _mp[(int)_player.Class, _player.Level];
        }

        public void loadEXPData()
        {
            double[] _xpData;
            _xpData = new double[256];
            double[] v = new double[256];
            double variable = 1;
            v[0] = 540;
            v[1] = 960;
            _xpData[0] = 300;
            for (int i = 2; i < v.Length; i++)
            {
                v[i] = v[i - 1] + 420 + (120 * (i - 1));
            }
            for (int i = 1; i < _xpData.Length; i++)
            {
                if (i < 79)
                {
                    switch (i)
                    {
                        case 14:
                            variable = 6 / 3d;
                            break;

                        case 39:
                            variable = 19 / 3d;
                            break;

                        case 59:
                            variable = 70 / 3d;
                            break;
                    }

                    _xpData[i] = Convert.ToInt64(_xpData[i - 1] + (variable * v[i - 1]));
                }
                if (i >= 79)
                {
                    switch (i)
                    {
                        case 79:
                            variable = 5000;
                            break;

                        case 82:
                            variable = 9000;
                            break;

                        case 84:
                            variable = 13000;
                            break;
                    }

                    _xpData[i] = Convert.ToInt64(_xpData[i - 1] + (variable * (i + 2) * (i + 2)));
                }
            }
            ExperienceToUp = (int)_xpData[_player.Level - 1];
        }

        public void loadJobXPData()
        {
            double[] _firstJobXpData;
            double[] _secondjobxpData;

            _firstJobXpData = new double[21];
            _secondjobxpData = new double[256];
            _firstJobXpData[0] = 2200;
            _secondjobxpData[0] = 17600;
            for (int i = 1; i < _firstJobXpData.Length; i++)
            {
                _firstJobXpData[i] = _firstJobXpData[i - 1] + 700;
            }

            for (int i = 1; i < _secondjobxpData.Length; i++)
            {
                int var2 = 400;
                if (i > 3)
                {
                    var2 = 4500;
                }
                if (i > 40)
                {
                    var2 = 15000;
                }
                _secondjobxpData[i] = _secondjobxpData[i - 1] + var2;
            }
            JobExperienceToUp = (_player.Class == ClassId.Adventurer ? (int)_firstJobXpData[_player.JobLevel - 1] : (int)_secondjobxpData[_player.Level - 1]);
        }

        public async Task<int> GetHealthModifiers()
        {
            var bEffs = BCardEffectDefinitions.BCardMapping.BCardEffects;
            int hp = 0;

            var equippedItems = new[]
            {
                 _player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.ARMOR),
                 _player.Inventory.GetEquippedItemFromSlot((int)EquipmentType.MAIN_WEAPON)
            };

            foreach (var item in _player.Inventory.GetEquippedItems())
            {
                if (item == null)
                    continue;

                foreach (var bCard in await GetItemBCards(item.ItemId))
                {
                    if (bEffs.TryGetValue(bCard.SubType, out var bCardTypes) && bCardTypes.Contains(bCard.Type))
                    {
                        // exclude bcard types
                        var excludeTypes = new List<BCardType> { BCardType.AttackPower, BCardType.Element, BCardType.SPECIAL_ATTACK };

                        if (excludeTypes.Contains(bCard.Type))
                            continue;

                        switch (bCard.SubType)
                        {
                            case (BCardEffect)11:
                                hp += bCard.FirstEffectValue;
                                break;

                            case (BCardEffect)12:
                                hp += bCard.FirstEffectValue;
                                break;
                        }
                    }
                }
            }

            foreach(var buff in _player.Buffs.ToList())
            {
                if (buff is null) continue;
                var bCards = await WorldManager.GetBCardsFromBuff(buff.BuffId);
                foreach(var bCard in bCards)
                {
                    if (bEffs.TryGetValue(bCard.SubType, out var bCardTypes))
                    {
                        var includeTypes = new List<BCardType> { BCardType.OldHealthMana };
                        if (!includeTypes.Contains(bCard.Type))
                            continue;

                        switch (bCard.SubType)
                        {
                            case (BCardEffect)41:
                                hp += BaseHealth * bCard.FirstEffectValue / 100;

                                break;

                            case (BCardEffect)42:
                                hp += BaseHealth * bCard.FirstEffectValue / 100;
                                break;
                        }
                    }
                }
            }

            return hp;
        }

        public async Task<int> GetManaModifiers()
        {
            var bEffs = BCardEffectDefinitions.BCardMapping.BCardEffects;
            int mp = 0;

            foreach (var item in _player.Inventory.GetEquippedItems())
            {
                if (item == null)
                    continue;

                foreach (var bCard in await GetItemBCards(item.ItemId))
                {
                    if (bEffs.TryGetValue(bCard.SubType, out var bCardTypes) && bCardTypes.Contains(bCard.Type))
                    {
                        var excludeTypes = new List<BCardType> { BCardType.AttackPower, BCardType.Element, BCardType.SPECIAL_ATTACK };

                        if (excludeTypes.Contains(bCard.Type))
                            continue;

                        switch (bCard.SubType)
                        {
                            case (BCardEffect)21:
                                mp += bCard.FirstEffectValue;
                                break;

                            case (BCardEffect)22:
                                mp += bCard.FirstEffectValue;
                                break;
                        }
                    }
                }

                foreach (var buff in _player.Buffs.ToList())
                {
                    if (buff is null) continue;
                    var bCards = await WorldManager.GetBCardsFromBuff(buff.BuffId);
                    foreach (var bCard in bCards)
                    {
                        if (bEffs.TryGetValue(bCard.SubType, out var bCardTypes) && bCardTypes.Contains(BCardType.HealthMana))
                        {
                            switch (bCard.SubType)
                            {
                                case (BCardEffect)51:
                                    mp += BaseMana * bCard.FirstEffectValue / 100;
                                    break;

                                case (BCardEffect)52:
                                    mp += BaseMana * bCard.FirstEffectValue / 100;
                                    break;
                            }
                        }
                    }
                }
            }

            return mp;
        }

        public async Task<int> GetMovementSpeedModifiers()
        {
            var bEffs = BCardEffectDefinitions.BCardMapping.BCardEffects;
            int bonus = 0;

            foreach (var item in _player.Inventory.GetEquippedItems())
            {
                if (item == null)
                    continue;

                var getSpWithSpeedBonus = WorldManager.Items.FirstOrDefault(x => x.Id == item.ItemId && x.ItemType == ItemType.SPECIALIST);

                if (getSpWithSpeedBonus != null && _player.UsingSpecialist)
                {
                    bonus += getSpWithSpeedBonus.Speed;
                }

                foreach (var bCard in await GetItemBCards(item.ItemId))
                {
                    if (bCard.Type != BCardType.Movement)
                        continue;

                    if (bEffs.TryGetValue(bCard.SubType, out var bCardTypes) && bCardTypes.Contains(BCardType.Movement))
                    {
                        switch (bCard.SubType)
                        {
                            case (BCardEffect)41:
                                bonus += (byte)bCard.FirstEffectValue;
                                break;

                            case (BCardEffect)42:
                                bonus += bCard.FirstEffectValue;
                                break;
                        }
                    }
                }
            }

            foreach (var buff in _player.Buffs.ToList())
            {
                var bCards = await WorldManager.GetBCardsFromBuff(buff.BuffId);

                foreach (var bCard in bCards.Where(x => x.Type == BCardType.Movement))
                {
                    if (bEffs.TryGetValue(bCard.SubType, out var bCardTypes) && bCardTypes.Contains(BCardType.Movement))
                    {
                        switch (bCard.SubType)
                        {
                            case (BCardEffect)41:
                                bonus += (byte)bCard.FirstEffectValue;
                                break;
                            case (BCardEffect)42:
                                bonus += bCard.FirstEffectValue;
                                break;
                        }
                    }
                }
            }
            return bonus;
        }

        public void IncreaseMaxHealth(int amount)
        {
            BaseHealth += amount;
        }
    }
}