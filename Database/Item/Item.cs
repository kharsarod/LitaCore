using Enum.Main.ItemEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Item
{
    public class Item
    {
        [Key]
        public short Id { get; set; }

        public byte DefaultUpgrade { get; set; }
        public byte CellonLevel { get; set; }
        public byte RequiredClass { get; set; }
        public short CloseDefence { get; set; }
        public byte Color { get; set; }
        public short Concentrate { get; set; }
        public byte CriticalLuckRate { get; set; }
        public short CriticalRate { get; set; }
        public short DamageMaximum { get; set; }
        public short DamageMinimum { get; set; }
        public byte DarkElement { get; set; }
        public short DarkResistance { get; set; }
        public short DefenceDodge { get; set; }
        public short DistanceDefence { get; set; }
        public short DistanceDefenceDodge { get; set; }
        public short Effect { get; set; }
        public int EffectData { get; set; }
        public byte Element { get; set; }
        public short ElementRate { get; set; }
        public EquipmentType EquipmentSlot { get; set; }
        public byte FireElement { get; set; }
        public short FireResistance { get; set; }
        public byte Height { get; set; }
        public short HitRate { get; set; }
        public short Hp { get; set; }
        public short HpRegeneration { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsColored { get; set; }
        public bool IsConsumable { get; set; }
        public bool IsDroppable { get; set; }
        public bool IsHeroic { get; set; }
        public bool IsHolder { get; set; }
        public bool IsMinilandObject { get; set; }
        public bool IsSoldable { get; set; }
        public bool IsTradable { get; set; }
        public byte ItemSubType { get; set; }
        public ItemType ItemType { get; set; }
        public long ItemValidTime { get; set; }
        public byte LevelJobMinimum { get; set; }
        public byte LevelMinimum { get; set; }
        public byte LightElement { get; set; }
        public short LightResistance { get; set; }
        public short MagicDefence { get; set; }
        public short MaxCellon { get; set; }
        public short MaxCellonLvl { get; set; }
        public short MaxElementRate { get; set; }
        public byte MaximumAmmo { get; set; }
        public int MinilandObjectPoint { get; set; }
        public short MoreHp { get; set; }
        public short MoreMp { get; set; }
        public short Morph { get; set; }
        public short Mp { get; set; }
        public short MpRegeneration { get; set; }
        public long Price { get; set; }
        public long SellToNpcPrice { get; set; }
        public short PvpDefence { get; set; }
        public byte PvpStrength { get; set; }
        public short ReduceOposantResistance { get; set; }
        public byte ReputationMinimum { get; set; }
        public long ReputPrice { get; set; }
        public byte SecondaryElement { get; set; }
        public byte Sex { get; set; }
        public byte Speed { get; set; }
        public byte SpType { get; set; }
        public InventoryType Type { get; set; }
        public short WaitDelay { get; set; }
        public byte WaterElement { get; set; }
        public short WaterResistance { get; set; }
        public byte Width { get; set; }

        public virtual ICollection<ItemTranslation> Translations { get; set; } = new List<ItemTranslation>();
    }

}
