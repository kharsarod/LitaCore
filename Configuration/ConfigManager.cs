using Configuration.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class ConfigManager
    {
        public static WorldServerConfig WorldServerConfig { get; set; }
        public static void LoadWorldConfiguration()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("WorldConf.json", optional: false, reloadOnChange: true)
            .Build();

            var worldServerConfig = config.Get<WorldServerConfig>();
            if (worldServerConfig == null)
            {
                throw new Exception("WorldConf.json is not configured correctly or is missing.");
            }
            WorldServerConfig = worldServerConfig;
        }
    }
}
