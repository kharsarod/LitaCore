using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.ServerConf
{
    public class Conf
    {
        public string? ServerName { get; set; }
        public int Port { get; set; }
        public int Port_CH1 { get; set; }
    }

    public class AppSettings
    {
        public Conf Configuration { get; set; }
    }
}
