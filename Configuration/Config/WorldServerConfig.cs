using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Config
{
    public class WorldServerConfig
    {
        public RateSettings Rates { get; set; }
        public FeatureSettings Features { get; set; }
        public GameplaySettings Gameplay { get; set; }
    }

    public class RateSettings
    {
        public double ExpRate { get; set; }
        public double GoldRate { get; set; }
        public double DropRate { get; set; }
        public double JobExpRate { get; set; }
    }

    public class FeatureSettings
    {
        public bool PlayVideoOnFirstLogin { get; set; }
        public bool AllowPacketLogging { get; set; }
    }

    public class GameplaySettings
    {
        public byte MaxLevel { get; set; }
        public byte MaxJobLevel { get; set; }

        public byte StartPlayerLevel { get; set; }
        public byte StartPlayerJobLevel { get; set; }

        public short StartPlayerMapID { get; set; }
        public short StartPlayerPositionX { get; set; }
        public short StartPlayerPositionY { get; set; }

        public long StartGold { get; set; }
        public int StartReputation { get; set; }
        public byte StartDignity { get; set; }
    }
}
