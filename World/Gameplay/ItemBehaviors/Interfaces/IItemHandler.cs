using Database.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network;

namespace World.Gameplay.ItemBehaviors.Interfaces
{
    public interface IItemHandler
    {
        Task HandleUse(ClientSession session, Item item);
    }
}