using Enum.Main.PlayerEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.World
{
    public class BuffTranslation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Language Language { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
