using AutoMapper;
using Database.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;

namespace World.Utils.Configuration
{
    public static class MapperConfig
    {
        public static IMapper Mapper { get; private set; }

        static MapperConfig()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Character, Player>();
            });

            Mapper = config.CreateMapper();
        }
    }
}
