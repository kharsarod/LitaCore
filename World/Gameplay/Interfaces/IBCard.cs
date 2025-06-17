using Database.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;

namespace World.Gameplay.Interfaces
{
    public interface IBCard
    {
        BCard BCard { get; set; }
        Task Apply(Player player, short skillVNum = -1);
    }
}
