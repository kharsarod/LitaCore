using Enum.Main.PlayerEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Player
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte Rank { get; set; }
        public bool IsBanned { get; set; }
        public Language Language { get; set; }
        public DateTime LastLogin { get; set; }
        public string IpAddress { get; set; }
        public bool IsOnline { get; set; }
    }
}