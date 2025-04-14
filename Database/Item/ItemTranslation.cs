using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enum.Main.PlayerEnum;

namespace Database.Item
{
    public class ItemTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Language Language { get; set; }

        [Required]
        public string? Name { get; set; }

        [ForeignKey("Item")]
        public short ItemId { get; set; }
        public virtual Item Item { get; set; }
    }

}
