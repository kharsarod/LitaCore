using Enum.Main.ItemEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Gameplay.ItemBehaviors.Interfaces;

namespace World.Gameplay.ItemBehaviors
{
    public static class ItemHandlerFactory
    {
        public static IItemHandler GetHandler(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.SPECIAL => new SpecialItemHandler(),
            };
        }
    }
}