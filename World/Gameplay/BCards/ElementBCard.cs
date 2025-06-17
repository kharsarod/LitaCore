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
    [BCardHandler(BCardType.Element)]
    public class ElementBCard : IBCard
    {
        public BCard BCard { get; set; }

        public ElementBCard(BCard bCard)
        {
            BCard = bCard;
        }

        public async Task Apply(Player player, short skillVNum)
        {

        }
    }
}
