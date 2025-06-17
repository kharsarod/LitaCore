using Database.Item;
using Enum.Main.BCardEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.BCards.Handler;
using World.Gameplay.Interfaces;

namespace World.Gameplay.BCards
{
    [BCardHandler(BCardType.Experience)]
    public class ExperienceBCard : IBCard
    {
        public BCard BCard { get; set; }

        public ExperienceBCard(BCard bCard)
        {
            BCard = bCard;
        }

        public async Task Apply(Player player, short skillVNum)
        {

        }
    }
}
