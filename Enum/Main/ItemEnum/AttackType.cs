using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enum.Main.ItemEnum
{
    public enum AttackType
    {
        Melee = 0,
        Ranged = 1,
        Magical = 2,
        Other = 3,
        Charge = 4,
        Dash = 5 // todas las skills que cambian la posición del personaje, puede ser de arrastre.
    }
}
