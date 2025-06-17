using Enum.Main.BuffEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.World
{
    public class BuffData
    {
        // No colocar el BuffId como auto incremental.

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short BuffId { get; set; }
        public BuffType BuffType { get; set; }
        public int EffectId { get; set; }

        public byte Level { get; set; }
        public byte ActivationChance { get; set; }

        public int ActivationDelayMs { get; set; }
        public int DurationMs { get; set; }

        public short ExpirationBuffId { get; set; }
        public byte ExpirationBuffChance { get; set; }

        public ICollection<BuffTranslation> Translations { get; set; } = new List<BuffTranslation>();
    }
}
