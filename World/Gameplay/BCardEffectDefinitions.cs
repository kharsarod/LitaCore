using Enum.Main.BCardEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Gameplay
{
    public class BCardEffectDefinitions
    {
        public static class BCardMapping
        {
            public static readonly Dictionary<BCardEffect, List<BCardType>> BCardEffects = new()
            {
                { BCardEffect.MaximumHPMPDecreased, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.MaximumHPMPIncreased, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.MaximumHPIncreased, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.MaximumHPDecreased, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.MaximumMPIncreased, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.MaximumMPDecreased, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.IncreasesMaximumHP, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.DecreasesMaximumHP, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.IncreasesMaximumMP, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
                { BCardEffect.DecreasesMaximumMP, new() { BCardType.HealthMana, BCardType.Element, BCardType.AttackPower, BCardType.Defenses, BCardType.Movement, BCardType.Defenses } },
            };
        }
    }
}