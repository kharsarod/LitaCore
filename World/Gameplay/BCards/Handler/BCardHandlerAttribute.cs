using Enum.Main.BCardEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Gameplay.BCards.Handler
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class BCardHandlerAttribute : Attribute
    {
        public BCardType Type { get; set; }

        public BCardHandlerAttribute(BCardType type)
        {
            Type = type;
        }
    }
}
