using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.ShopEntity
{
    public class ShopItem
    {
        public int Id { get; set; }
        public short ItemId { get; set; }
        public long Price { get; set; }
        public byte Rarity { get; set; }
        public byte Upgrade { get; set; }
        public byte Slot { get; set; }
        public byte Color { get; set; }
        public byte Window { get; set; }
        public int ShopId { get; set; }
    }
}