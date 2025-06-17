using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Server
{
    public class ChannelInfo
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int ChannelPort { get; set; }
        public int MaxPlayers { get; set; }
        public int OnlinePlayers { get; set; }
        public byte ChannelId { get; set; }
        public Status ChannelStatus { get; set; }
        public byte ServerId { get; set; }
        public string PublicAddress { get; set; }
        public bool LocalMode { get; set; }
    }

    public enum Status
    {
        Online,
        Offline
    }
}