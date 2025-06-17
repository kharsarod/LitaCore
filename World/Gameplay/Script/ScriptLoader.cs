using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Entities;
using World.Gameplay.Script.Interfaces;
using World.Gameplay.Script.Objects;
using World.Gameplay.Script.ScriptAttributes;

namespace World.Gameplay.Script
{
    public static class ScriptLoader
    {
        private static readonly Dictionary<Player, List<PlayerScript>> _playerScripts = new();
        private static List<Type> _scriptTypes = new();

        public static void Initialize()
        {
            _scriptTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass &&
                            !x.IsAbstract &&
                            typeof(PlayerScript).IsAssignableFrom(x) &&
                            x.GetCustomAttributes(typeof(ScriptAttribute), false).Any())
                .ToList();

            foreach(var scriptType in _scriptTypes)
            {
                var scriptAttribute = scriptType.GetCustomAttributes(typeof(ScriptAttribute), false).First() as ScriptAttribute;
                Log.Information("Script loaded: {Name}", scriptAttribute.Name);
            }
        }

        public static async Task RegisterAllScripts(Player player)
        {
            if (_playerScripts.ContainsKey(player))
                return;

            var scriptInstances = new List<PlayerScript>();

            foreach (var scriptType in _scriptTypes)
            {
                IScript scriptInstance = null;

                var ctor = scriptType.GetConstructor(new[] { typeof(Player) });
                if (ctor != null)
                {
                    scriptInstance = (IScript)ctor.Invoke(new object[] { player });
                }
                else
                {
                    scriptInstance = (IScript)Activator.CreateInstance(scriptType);
                }

                if (scriptInstance != null)
                {
                    await scriptInstance.Register();

                    scriptInstances.Add((PlayerScript)scriptInstance);
                }

                _playerScripts[player] = scriptInstances;
            }
        }

        public static List<PlayerScript> GetScriptsForPlayer(Player player)
        {
            return _playerScripts.TryGetValue(player, out var scripts) ? scripts : new List<PlayerScript>();
        }
    }
}
