using AutoMapper.Configuration.Annotations;
using Enum.Main.BCardEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Database.Item
{
    public class BCard
    {
        public int BCardId { get; set; }

        public short? BuffId { get; set; }

        public byte CastType { get; set; }

        public int FirstEffectValue { get; set; }

        public bool IsLevelDivided { get; set; }

        public bool IsLevelScaled { get; set; }

        public short? ItemId { get; set; }

        [NotMapped]
        public short? VNum { get; set; }

        public int SecondaryEffectValue { get; set; }

        public int NpcMonsterVNum { get; set; }

        public short? SkillVNum { get; set; }

        public BCardEffect SubType { get; set; }

        public int ThirdEffectValue { get; set; }

        public BCardType Type { get; set; }
    }
}