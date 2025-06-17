using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.World
{
    public class Map
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Id { get; set; }
        public string? Name { get; set; }

        public int Bgm { get; set; }

        public bool IsShopAllowed { get; set; }

        public byte[] MapGrid { get; set; }

        public byte ExpRate { get; set; }

        public byte GoldRate { get; set; }

        public byte DropRate { get; set; }

        public bool IsPvpAllowed { get; set; }

        public short Height { get; set; }

        public short Width { get; set; }

    }
}
