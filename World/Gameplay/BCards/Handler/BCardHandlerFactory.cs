using Database.Item;
using Enum.Main.BCardEnum;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Gameplay.Interfaces;

namespace World.Gameplay.BCards.Handler
{
    public static class BCardHandlerFactory
    {
        private static readonly Dictionary<BCardType, Type> _handlers = new Dictionary<BCardType, Type>();

        private static bool _initialized = false;

        public static void Initialize()
        {
            var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetCustomAttributes(typeof(BCardHandlerAttribute), false).FirstOrDefault() is BCardHandlerAttribute);

            foreach (var type in handlerTypes)
            {
                var attr = type.GetCustomAttributes(typeof(BCardHandlerAttribute), false).FirstOrDefault() as BCardHandlerAttribute;
               
                if (attr != null && !_handlers.ContainsKey(attr.Type))
                {
                    _handlers[attr.Type] = type;
                    Log.Information("Registered BCardHandler: {Type}", attr.Type);
                }
            }
        }

        public static IBCard Create(BCard bCard)
        {
            if (_handlers.TryGetValue(bCard.Type, out var handlerType))
            {
                return (IBCard)Activator.CreateInstance(handlerType, bCard);
            }
            Log.Warning("No BCardHandler found for type: {Type}", bCard.Type);
            return null;
        }
    }
}
