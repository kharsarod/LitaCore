using Enum.Main.PlayerEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.ShopEntity
{
    public class ShopTranslations
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public Language Language { get; set; }
        public string? Name { get; set; }
    }
}