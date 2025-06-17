using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.ShopEntity
{
    public class Shop
    {
        public int Id { get; set; }
        public int NpcId { get; set; }
        public byte MenuType { get; set; }
        public byte ShopType { get; set; }
        public int ShopId { get; set; }
    }
}