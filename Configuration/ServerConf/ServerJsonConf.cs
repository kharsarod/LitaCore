using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.ServerConf
{
    public class ServerJsonConf
    {
        public async Task<AppSettings> ReadConfigAsync()
        {
            string json = await File.ReadAllTextAsync("Config.json");

            return JsonConvert.DeserializeObject<AppSettings>(json);
        }

        public async Task Initialize()
        {
            AppSettings settings = await ReadConfigAsync();
        }
    }
}
