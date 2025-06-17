using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enum.Main.BuffEnum
{
    [Flags]
    public enum BuffType : byte
    {
        Good = 0,
        Neutral = 1,
        Bad = 2,
        All = Good | Neutral | Bad
    }
}
