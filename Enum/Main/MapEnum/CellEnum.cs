using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enum.Main.MapEnum
{
    [Flags]
    public enum CellEnum
    {
        WalkDisabled = 0x1,
        AttackDisabledThrough = 0x2,
        MonsterAggroDisable = 0x8,
        PvpDisable = 0x10,
        PetZone = 0x20
    }
}
