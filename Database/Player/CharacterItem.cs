using Database.Item;
using Enum.Main.ItemEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Player
{
    public class CharacterItem
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public Character Character { get; set; }

        public int ItemId { get; set; }

        public short Amount { get; set; }
        public InventoryType InventoryType { get; set; }
        public int Slot { get; set; }

        public byte Rarity { get; set; }
        public byte Upgrade { get; set; }
        public bool IsFixed { get; set; }
        public short Ammo { get; set; }
        public short HoldingModel { get; set; }
        public short MaxDmg { get; set; }
        public short MinDmg { get; set; }
        public short HitRate { get; set; }
        public short CritLuckRate { get; set; }
        public short CritRate { get; set; }
        public short CloseDefence { get; set; }
        public short DistDefence { get; set; }
        public short MagicDefence { get; set; }
        public short DefDodge { get; set; }
        public short DistDefDodge { get; set; }
        public byte FairyLevel { get; set; }
        public int FairyMonsterRemaining { get; set; }
        public DateTime TimeRemaining { get; set; }

        public EquipmentType EquipmentSlot { get; set; }
    }
}

// characterId|isMarried ? 5 : 0|isConnected ? 1 : 0|Sylas

// infoi 238 0 0 0 (you're blocked)

// infoi 239 0 0 0 (you're accepted)