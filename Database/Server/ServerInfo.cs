using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Server
{
    public class ServerInfo
    {
        public int Id { get; set; }
        public string ServerName { get; set; }
        public bool UnderMaintenance { get; set; }
        public byte ServerId { get; set; }
    }
}