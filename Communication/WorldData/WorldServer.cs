using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.WorldData
{
    public class WorldServer
    {
        public int ChannelId { get; set; }

        public string EndPoint { get; set; }

        public static int ChannelPort { get; set; }
    }
}