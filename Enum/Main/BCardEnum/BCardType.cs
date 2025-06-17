using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enum.Main.BCardEnum
{
    public enum BCardType
    {
        SPECIAL_ATTACK = 1,
        AttackPower = 3,
        PreventsAndGuaranteed = 16,
        Movement = 19,
        Buff = 25,
        ShadowAppear = 27,
        OldHealthMana = 32,
        HealthMana = 33,
        Element = 7,
        Experience = 44,
        Defenses = 44,
        AdvancedExp = 89
    }

    public enum BCardEffect
    {
        MaximumHPIncreased = 11,
        MaximumHPDecreased = 12,
        MaximumMPIncreased = 21,
        MaximumMPDecreased = 22,
        IncreasesMaximumHP = 41,
        DecreasesMaximumHP = 42,
        IncreasesMaximumMP = 51,
        DecreasesMaximumMP = 52,
        MaximumHPMPIncreased = 31,
        MaximumHPMPDecreased = 32,
    }
}