using Database.Helper;
using Database.Player;
using Enum.Main.ItemEnum;
using GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Helpers
{
    public static class CharacterItemHelper
    {
        public static float GetRarityMultiplier(byte rarity)
        {
            return rarity switch
            {
                0 => 1.0f,
                1 => 1.1f,
                2 => 1.13f,
                3 => 1.15f,
                4 => 1.7f,
                5 => 1.9f,
                6 => 1.95f,
                7 => 1.98f,
                _ => 1.0f
            };
        }

        public static float GetUpgradeMultiplier(byte upgrade)
        {
            return 1.0f + (upgrade * 0.05f);
        }

        public static async Task SetItemStats(CharacterItem item)
        {
            var getItem = WorldManager.GetItem(item.ItemId);
            if (getItem is null) return;

            if (item.Rarity > 0 || item.Upgrade > 0)
            {
                switch (getItem.EquipmentTypeSlot)
                {
                    case EquipmentType.MAIN_WEAPON or EquipmentType.SECONDARY_WEAPON:
                        item.MinDmg = (short)(getItem.DamageMinimum * GetRarityMultiplier(item.Rarity) * GetUpgradeMultiplier(item.Upgrade));
                        item.MaxDmg = (short)(getItem.DamageMaximum * GetRarityMultiplier(item.Rarity) * GetUpgradeMultiplier(item.Upgrade));
                        break;

                    case EquipmentType.ARMOR:
                        item.CloseDefence = (short)(getItem.CloseDefence * GetRarityMultiplier(item.Rarity) * GetUpgradeMultiplier(item.Upgrade));
                        item.MagicDefence = (short)(getItem.MagicDefence * GetRarityMultiplier(item.Rarity) * GetUpgradeMultiplier(item.Upgrade));
                        item.DistDefence = (short)(getItem.DistanceDefence * GetRarityMultiplier(item.Rarity) * GetUpgradeMultiplier(item.Upgrade));
                        break;
                }

                await CharacterDbHelper.UpdateAsync(item);
            }
        }
    }
}
